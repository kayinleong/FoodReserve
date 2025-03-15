using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FoodReserve.OutletPortal.Services.Auth
{
    public class CustomAuthStateProvider(ProtectedSessionStorage sessionStorage) : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var storedTokenResult = await sessionStorage.GetAsync<string>("authToken");
                var storedToken = storedTokenResult.Success ? storedTokenResult.Value : null;

                if (string.IsNullOrEmpty(storedToken))
                {
                    return new AuthenticationState(_anonymous);
                }

                var claimsPrincipal = CreateClaimsPrincipal(storedToken);
                var state = new AuthenticationState(claimsPrincipal);
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return state;
            }
            catch
            {
                var state = new AuthenticationState(_anonymous);
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return state;
            }
        }

        public async Task NotifyAuthenticationStateChangedAsync()
        {
            var authState = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        private static ClaimsPrincipal CreateClaimsPrincipal(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var claimsIdentity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, claims.First(c => c.Type == "id").Value));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, claims.First(c => c.Type == "unique_name").Value));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, claims.First(c => c.Type == "role").Value));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, claims.First(c => c.Type == "email").Value));
            claimsIdentity.AddClaim(new Claim("token", token));
            return new ClaimsPrincipal(claimsIdentity);
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
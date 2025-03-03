using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Authorization;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FoodReserve.AdminPortal.Services.Auth
{
    public class AuthService(
        IHttpClientFactory httpClientFactory,
        NavigationManager navigationManager,
        ProtectedSessionStorage sessionStorage,
        ISnackbar snackbar,
        AuthenticationStateProvider authStateProvider
    )
    {
        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var loginRequest = new UserLoginRequest
                {
                    Username = username,
                    Password = password
                };

                var client = httpClientFactory.CreateClient("FoodReserve.API");
                var response = await client.PostAsJsonAsync("api/auth", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (data != null)
                    {
                        await sessionStorage.SetAsync("authToken", data.Token);
                        if (authStateProvider is CustomAuthStateProvider customProvider)
                        {
                            await customProvider.NotifyAuthenticationStateChangedAsync();
                        }

                        snackbar.Add("Login successful!", Severity.Success);
                        return true;
                    }
                }

                snackbar.Add("Login failed. Please try again.", Severity.Error);
                return false;
            }
            catch (Exception ex)
            {
                snackbar.Add($"An error occurred: {ex.Message}", Severity.Error);
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await sessionStorage.DeleteAsync("authToken");
            if (authStateProvider is CustomAuthStateProvider customProvider)
            {
                await customProvider.NotifyAuthenticationStateChangedAsync();
            }

            navigationManager.NavigateTo("/auth/login");
        }
    }
}
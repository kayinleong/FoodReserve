using FoodReserve.OutletPortal.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace FoodReserve.OutletPortal.Extensions
{
    public static class AuthExtensions
    {
        public static void AddAuthConfig(this IServiceCollection services)
        {
            services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            services.AddAuthorizationCore();
            services.AddCascadingAuthenticationState();
        }
    }
}

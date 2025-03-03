using FoodReserve.AdminPortal.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace FoodReserve.AdminPortal.Extensions
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

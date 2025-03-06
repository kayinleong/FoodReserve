using FoodReserve.AdminPortal.Services;
using FoodReserve.AdminPortal.Services.Auth;
using MudBlazor.Services;

namespace FoodReserve.AdminPortal.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServiceConfig(this IServiceCollection services)
        {
            services.AddMudServices();
            services.AddScoped<AuthService>();
            services.AddScoped<OutletService>();
            services.AddScoped<OutletUserService>();
            services.AddScoped<OutletStaffService>();
        }
    }
}

using FoodReserve.OutletPortal.Services;
using FoodReserve.OutletPortal.Services.Auth;
using MudBlazor.Services;

namespace FoodReserve.OutletPortal.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServiceConfig(this IServiceCollection services)
        {
            services.AddMudServices();
            services.AddScoped<AuthService>();
            services.AddScoped<OutletService>();
            services.AddScoped<CustomerService>();
            services.AddScoped<ReservationService>();
            services.AddScoped<QueueService>();
        }
    }
}

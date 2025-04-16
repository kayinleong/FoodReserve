using FoodReserve.OutletPortal.Services;
using FoodReserve.OutletPortal.Services.Auth;
using MudBlazor.Services;

namespace FoodReserve.OutletPortal.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServiceConfig(this IServiceCollection services)
        {
            services.AddSignalR(e => {
                e.EnableDetailedErrors = true;
                e.MaximumReceiveMessageSize = 102400000;
            });

            services.AddMudServices();
            services.AddScoped<AuthService>();
            services.AddScoped<OutletService>();
            services.AddScoped<StaffService>();
            services.AddScoped<CustomerService>();
            services.AddScoped<ReservationService>();
            services.AddScoped<QueueService>();
        }
    }
}

using FoodReserve.API.Models;
using FoodReserve.API.Services;
using Microsoft.AspNetCore.Identity;

namespace FoodReserve.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServiceConfig(this IServiceCollection services)
        {
            services.AddHostedService<DatabaseWorker>();
            services.AddHostedService<UserWorker>();
            services.AddScoped<UserService>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddSingleton<AuthService>();
        }
    }
}

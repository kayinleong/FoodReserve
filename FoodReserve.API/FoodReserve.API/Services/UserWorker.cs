using FoodReserve.API.Context;
using FoodReserve.API.Models;
using FoodReserve.SharedLibrary.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodReserve.API.Services
{
    public class UserWorker(IServiceProvider serverProvider) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = serverProvider.CreateScope();

            DatabaseContext context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            IPasswordHasher<User> hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            if (await context.Users.AnyAsync(cancellationToken))
            {
                return;
            }

            User[] users = [
                new() {
                    Username = "superuser",
                    Email = "superuser@web.com",
                    PasswordHashed = "",
                    Role = UserRole.Superuser,
                    IsSuspended = false,
                },
                new() {
                    Username = "admin",
                    Email = "admin@web.com",
                    PasswordHashed = "",
                    Role = UserRole.Admin,
                    IsSuspended = false,
                },
                new() {
                    Username = "outlet",
                    Email = "outlet@web.com",
                    PasswordHashed = "",
                    Role = UserRole.Outlet,
                    IsSuspended = false,
                },
            ];

            foreach (var user in users) {
                user.PasswordHashed = hasher.HashPassword(user, "abc123456789");
            }

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

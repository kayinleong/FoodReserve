using FoodReserve.API.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodReserve.API.Services
{
    public class DatabaseWorker(IServiceProvider serverProvider) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = serverProvider.CreateScope();

            DatabaseContext context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            await context.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

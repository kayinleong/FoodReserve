using FoodReserve.API.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodReserve.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static void AddDatabaseConfig(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContextPool<DatabaseContext>(options =>
            {
                string mySqlConnectionString = config.GetConnectionString("MySqlConnection")!;

                options.UseMySql(mySqlConnectionString, ServerVersion.AutoDetect(mySqlConnectionString));
            });
        }
    }
}

namespace FoodReserve.API.Extensions
{
    public static class CorsExtensions
    {
        public static void AddCorsConfig(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("PROD", policyOptions =>
                {
                    policyOptions.WithOrigins("")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

                options.AddPolicy("DEV", policyOptions =>
                {
                    policyOptions.WithOrigins("*", "https://localhost:5000")
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        public static void UseCorsConfig(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("PROD");
            }
            else
            {
                app.UseCors("DEV");
            }
        }
    }
}

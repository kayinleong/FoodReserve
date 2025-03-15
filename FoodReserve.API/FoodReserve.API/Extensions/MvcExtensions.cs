using FoodReserve.API.Policies;
using Microsoft.AspNetCore.Mvc.Razor;

namespace FoodReserve.API.Extensions
{
    public static class MvcExtensions
    {
        public static void AddMvcConfig(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    })
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization()
                    .AddRazorRuntimeCompilation();
            }
            else
            {
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    })
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization();
            }

            services.Configure<RouteOptions>(options => options.AppendTrailingSlash = true);
            services.AddControllers();
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.AddHttpClient();
            services.AddHttpClient("Meta.WhatsApp", option =>
            {
                option.BaseAddress = new Uri(config["Meta.WhatsApp"]!);
            });
        }
    }
}

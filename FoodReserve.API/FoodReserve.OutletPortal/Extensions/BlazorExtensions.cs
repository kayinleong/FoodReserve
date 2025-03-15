namespace FoodReserve.OutletPortal.Extensions
{
    public static class BlazorExtensions
    {
        public static void AddBlazorConfig(this IServiceCollection services, IConfiguration config)
        {
            services.AddLogging();

            services.AddRazorComponents()
                .AddInteractiveServerComponents();

            services.AddHttpClient("FoodReserve.API", option =>
            {
                option.BaseAddress = new Uri(config["API_BASE"]!);
            });
        }
    }
}

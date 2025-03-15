using FoodReserve.OutletPortal.Components;
using FoodReserve.OutletPortal.Extensions;

namespace FoodReserve.OutletPortal;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddBlazorConfig(builder.Configuration);
        builder.Services.AddAuthConfig();
        builder.Services.AddServiceConfig();

        var app = builder.Build();
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        await app.RunAsync();
    }
}

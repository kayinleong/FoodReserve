using FoodReserve.API.Extensions;
using FoodReserve.API.Hubs;
using Microsoft.OpenApi.Models;

namespace FoodReserve.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCorsConfig();
            builder.Services.AddMvcConfig(builder.Configuration, builder.Environment);
            builder.Services.AddResponseCompressionConfig();
            builder.Services.AddAuthenticationConfig(builder.Configuration);
            builder.Services.AddDatabaseConfig(builder.Configuration);
            builder.Services.AddServiceConfig();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "FoodReserve.API Endpoint",
                        Version = "v1.0",
                        Description = "FoodReserve.API Developer API Endpoint"
                    }
                );

                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Here enter jwt with bear format like bearer[space] token",
                    }
                );

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                );
            });

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseResponseCompression();
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();
            app.UseCorsConfig();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                "areaRoute",
                "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapHub<QueueSignalRHub>("/queuehub");

            await app.RunAsync();
        }
    }
}

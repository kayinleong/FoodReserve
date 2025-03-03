using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FoodReserve.API.Extensions
{
    public static class AuthExtensions
    {
        public static void AddAuthenticationConfig(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = config["JWT:Authority"]!;
                options.Audience = config["JWT:Audience"]!;

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TokenValidationParameters.DefaultClockSkew,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = config["JWT:Audience"]!,
                    ValidIssuer = config["JWT:Authority"]!,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:PrivateKey"]!))
                };
            });
        }
    }
}

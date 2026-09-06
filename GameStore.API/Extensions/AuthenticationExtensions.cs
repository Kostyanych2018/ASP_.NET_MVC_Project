using System.Security.Authentication;
using System.Security.Claims;
using GameStore.Application.Common.Constants;
using GameStore.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GameStore.API.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authHost = configuration["AuthServer:Host"];
        var authRealm = configuration["AuthServer:Realm"];

        var authority = $"{authHost}/realms/{authRealm}";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.RequireHttpsMetadata = false;
                var audience = configuration["AuthServer:Audience"];
                options.Audience = audience;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        try
                        {
                            claimsIdentity.AddKeycloakRoles();
                        }
                        catch (AuthenticationException ex)
                        {
                            context.Fail(ex);
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthConstants.AdminPolicy, policy => policy.RequireRole(AuthConstants.AdminRole));
                options.AddPolicy(AuthConstants.UserPolicy, policy => policy.RequireRole(AuthConstants.UserRole));
            }
        );

        return services;
    }
}
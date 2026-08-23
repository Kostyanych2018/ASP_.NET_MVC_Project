using GameStore.Application.Common.Interfaces;
using GameStore.Infrastructure.Data;
using GameStore.Infrastructure.Services;
using GameStore.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnection = configuration.GetConnectionString("PostgreSQL");
        if (!string.IsNullOrWhiteSpace(postgresConnection))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(postgresConnection));
        }

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<DbInitializer>();

        services.Configure<FileStorageSettings>(options =>
        {
            options.DefaultImage = configuration["DefaultGameImage"] ?? "Images/default_game.png";
            options.FolderName = configuration["FolderName"] ?? "Images";
        });

        services.AddScoped<IFileService, FileService>();

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnection; });
        }

        services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 1024 * 10;
            options.DefaultEntryOptions = new HybridCacheEntryOptions()
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(1)
            };
        });

        return services;
    }
}
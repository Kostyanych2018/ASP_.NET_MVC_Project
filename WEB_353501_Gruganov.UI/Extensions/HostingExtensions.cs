using WEB_353501_Gruganov.UI.Services.GameService;

namespace WEB_353501_Gruganov.UI.Extensions;

public static class HostingExtensions
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddScoped<IGenreService, MemoryGenreService>()
            .AddScoped<IGameService, MemoryGameService>();
    }
}
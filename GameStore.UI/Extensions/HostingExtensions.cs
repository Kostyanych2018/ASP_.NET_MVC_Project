using GameStore.UI.HelperClasses;
using GameStore.UI.Services.FileService;
using GameStore.UI.Services.GameService;

namespace GameStore.UI.Extensions;

public static class HostingExtensions
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<KeycloakData>(builder.Configuration.GetSection("Keycloak"))
            .AddSingleton<IFileService, LocalFileService>();
    }
}
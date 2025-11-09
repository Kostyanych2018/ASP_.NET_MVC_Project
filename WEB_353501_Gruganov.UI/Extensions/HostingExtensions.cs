using WEB_353501_Gruganov.UI.HelperClasses;
using WEB_353501_Gruganov.UI.Services.FileService;
using WEB_353501_Gruganov.UI.Services.GameService;

namespace WEB_353501_Gruganov.UI.Extensions;

public static class HostingExtensions
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<KeycloakData>(builder.Configuration.GetSection("Keycloak"))
            .AddSingleton<IFileService, LocalFileService>();
    }
}
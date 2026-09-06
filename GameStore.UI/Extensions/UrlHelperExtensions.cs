using GameStore.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameStore.UI.Extensions;

public static class UrlHelperExtensions
{
    public static string ApiImage(this IUrlHelper urlHelper, string? imagePath)
    {
        var apiSettings = GetApiSettings(urlHelper);
        var baseUrl = apiSettings.BaseUrl.TrimEnd('/');

        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return BuildApiAssetUrl(baseUrl, apiSettings.DefaultGameImage);
        }

        if (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return imagePath;
        }

        return BuildApiAssetUrl(baseUrl, imagePath);
    }

    public static string ApiAvatar(this IUrlHelper urlHelper, string? avatarPath)
    {
        var apiSettings = GetApiSettings(urlHelper);
        var baseUrl = apiSettings.BaseUrl.TrimEnd('/');

        if (string.IsNullOrWhiteSpace(avatarPath))
        {
            return BuildApiAssetUrl(baseUrl, apiSettings.DefaultAvatar);
        }

        if (avatarPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || avatarPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return avatarPath;
        }

        return BuildApiAssetUrl(baseUrl, avatarPath);
    }

    private static ApiSettings GetApiSettings(IUrlHelper urlHelper)
    {
        var httpContext = urlHelper.ActionContext.HttpContext;
        return httpContext.RequestServices.GetRequiredService<IOptions<ApiSettings>>().Value;
    }

    private static string BuildApiAssetUrl(string baseUrl, string relativePath) =>
        $"{baseUrl}/{relativePath.TrimStart('/')}";
}

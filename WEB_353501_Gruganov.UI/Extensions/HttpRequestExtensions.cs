namespace WEB_353501_Gruganov.UI.Extensions;

public static class HttpRequestExtensions
{
    private static string AjaxHeader = "x-requested-with";
    private static string AjaxHeaderValue = "XMLHttpRequest";

    public static bool IsAjaxRequest(this HttpRequest request)
    {
        if (request.Headers.ContainsKey(AjaxHeader)) {
            return request.Headers[AjaxHeader].Equals(AjaxHeaderValue);
        }

        return false;
    }
}
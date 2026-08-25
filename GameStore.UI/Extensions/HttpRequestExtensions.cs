namespace GameStore.UI.Extensions;

public static class HttpRequestExtensions
{
    private static readonly string AjaxHeader = "x-requested-with";
    private static readonly string AjaxHeaderValue = "XMLHttpRequest";

    public static bool IsAjaxRequest(this HttpRequest request)
    {
        if (request.Headers.ContainsKey(AjaxHeader))
        {
            return request.Headers[AjaxHeader].Equals(AjaxHeaderValue);
        }

        return false;
    }
}
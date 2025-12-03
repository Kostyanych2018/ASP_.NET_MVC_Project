namespace WEB_353501_Gruganov.UI.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate requestDelegate, ILogger<RequestLoggingMiddleware> logger)
    {
        _requestDelegate = requestDelegate;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _requestDelegate(context);
        var statusCode = context.Response.StatusCode;
        if (statusCode < 200 || statusCode >= 300) {
            var url = context.Request.Path + context.Request.QueryString;
            _logger.LogInformation("---> request {RequestUrl} returns {StatusCode}", url, statusCode);
        }
    }
}
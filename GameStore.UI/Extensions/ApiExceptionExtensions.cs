using System.Net;
using GameStore.UI.Exceptions;

namespace GameStore.UI.Extensions;

public static class ApiExceptionExtensions
{
    public static void LogApiException(this ILogger logger, ApiException ex, string message, params object?[] args)
    {
        if ((int)ex.StatusCode >= (int)HttpStatusCode.InternalServerError)
        {
            logger.LogError(ex, message, args);
            return;
        }

        logger.LogWarning(ex, message, args);
    }
}

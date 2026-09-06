using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.UI.Exceptions;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public ProblemDetails? ProblemDetails { get; }

    public ApiException(
        HttpStatusCode statusCode,
        ProblemDetails? problemDetails,
        string? message = null)
        : base(message ??
               problemDetails?.Detail ??
               $"API request failed with status code {statusCode}")
    {
        StatusCode = statusCode;
        ProblemDetails = problemDetails;
    }
}
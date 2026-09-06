using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GameStore.API.Extensions;

public static class OpenApiExtensions
{
    private static readonly OpenApiObject ValidationProblemExample = new()
    {
        ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc9110#section-15.5.1"),
        ["title"] = new OpenApiString("Validation Error"),
        ["status"] = new OpenApiInteger(400),
        ["detail"] = new OpenApiString("One or more validation errors occurred."),
        ["instance"] = new OpenApiString("/api/games"),
        ["errors"] = new OpenApiObject
        {
            ["Name"] = new OpenApiArray { new OpenApiString("Game name is required.") },
            ["Price"] = new OpenApiArray { new OpenApiString("Price must be between 1 and 10000.") }
        }
    };

    private static readonly OpenApiObject NotFoundProblemExample = new()
    {
        ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc9110#section-15.5.5"),
        ["title"] = new OpenApiString("Resource Not Found"),
        ["status"] = new OpenApiInteger(404),
        ["detail"] = new OpenApiString("Game with ID 999 was not found."),
        ["instance"] = new OpenApiString("/api/games/999")
    };

    private static readonly OpenApiObject ForbiddenProblemExample = new()
    {
        ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc9110#section-15.5.4"),
        ["title"] = new OpenApiString("Forbidden"),
        ["status"] = new OpenApiInteger(403),
        ["detail"] = new OpenApiString("Access denied. Administrator role is required."),
        ["instance"] = new OpenApiString("/api/games")
    };

    public static OpenApiOptions AddGameStoreDocumentInfo(this OpenApiOptions options)
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info = new OpenApiInfo
            {
                Title = "GameStore API",
                Version = "v1",
                Description = "GameStore ASP .NET Web API"
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token. Example: Bearer eyJhbGciOi...",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            return Task.CompletedTask;
        });

        return options;
    }

    public static OpenApiOptions AddProblemDetailsExamples(this OpenApiOptions options)
    {
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            ApplyExample(operation.Responses, StatusCodes.Status400BadRequest.ToString(), ValidationProblemExample);
            ApplyExample(operation.Responses, StatusCodes.Status404NotFound.ToString(), NotFoundProblemExample);
            ApplyExample(operation.Responses, StatusCodes.Status403Forbidden.ToString(), ForbiddenProblemExample);

            return Task.CompletedTask;
        });

        return options;
    }

    private static void ApplyExample(
        IDictionary<string, OpenApiResponse> responses,
        string statusCode,
        IOpenApiAny example)
    {
        if (!responses.TryGetValue(statusCode, out var response))
        {
            return;
        }

        foreach (var mediaType in response.Content.Values)
        {
            mediaType.Example = example;
        }
    }
}

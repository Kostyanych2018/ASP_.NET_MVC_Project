using System.Text.Json;
using GameStore.UI.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.UI.Extensions;

public static class HttpResponseExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T> ReadAsJsonOrThrowAsync<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        await response.EnsureSuccessOrThrowApiExceptionAsync(cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<T>(DefaultJsonOptions, cancellationToken);
        if (result == null)
        {
            throw new ApiException(response.StatusCode, null, "Failed to deserialize server response payload.");
        }

        return result;
    }

    public static async Task EnsureSuccessOrThrowApiExceptionAsync(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        ValidationProblemDetails? problemDetails = null;
        string? rawContent = null;
        try
        {
            problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(DefaultJsonOptions, cancellationToken);
        }
        catch (JsonException)
        {
            rawContent = await response.Content.ReadAsStringAsync(cancellationToken);
        }

        var message = problemDetails?.Detail
                      ?? problemDetails?.Title
                      ?? (!string.IsNullOrWhiteSpace(rawContent) ? rawContent : $"Запрос к API завершился неудачно с кодом состояния {response.StatusCode} ({response.ReasonPhrase})");

        throw new ApiException(response.StatusCode, problemDetails, message);
    }
}
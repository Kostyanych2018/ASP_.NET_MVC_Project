using GameStore.UI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GameStore.UI.Extensions;

public static class ModelStateExtensions
{
    public static void AddApiException(this ModelStateDictionary modelState, ApiException exception)
    {
        if (exception.ProblemDetails is ValidationProblemDetails validationProblemDetails 
            && validationProblemDetails.Errors.Count > 0)
        {
            foreach (var (key, errorMessages) in validationProblemDetails.Errors)
            {
                foreach (var errorMessage in errorMessages)
                {
                    modelState.AddModelError(key, errorMessage);
                }
            }

            return;
        }

        var generalMessage = exception.ProblemDetails?.Detail ?? exception.ProblemDetails?.Title ?? exception.Message;
        modelState.AddModelError(string.Empty, generalMessage);
    }
}
using GameStore.UI.Exceptions;
using GameStore.UI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameStore.UI.Areas.Admin.Pages;

public abstract class AdminPageModel : PageModel
{
    protected ILogger Logger { get; }

    protected AdminPageModel(ILogger logger)
    {
        Logger = logger;
    }

    protected IActionResult HandleApiExceptionForForm(ApiException ex, string logMessage, params object?[] args)
    {
        Logger.LogApiException(ex, logMessage, args);
        ModelState.AddApiException(ex);
        return Page();
    }

    protected IActionResult HandleApiExceptionNotFound(ApiException ex, string logMessage, params object?[] args)
    {
        Logger.LogApiException(ex, logMessage, args);
        return NotFound();
    }

    protected IActionResult HandleApiExceptionForList(
        ApiException ex,
        string logMessage,
        string userMessage,
        params object?[] args)
    {
        Logger.LogApiException(ex, logMessage, args);
        TempData["ErrorMessage"] = userMessage;
        return Page();
    }
}

using GameStore.UI.Constants;
using GameStore.UI.Exceptions;
using GameStore.UI.Extensions;
using GameStore.UI.Models;
using GameStore.UI.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.UI.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAuthService authService,
        ILogger<AccountController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterUserViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _authService.RegisterUserAsync(model, cancellationToken);
            return RedirectToAction("Index", "Home");
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Registration failed for user {Email}", model.Email);
            ModelState.AddApiException(ex);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for user {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "Registration failed. Please try again later.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task Login(string? returnUrl = null)
    {
        var redirectUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "Home");

        await HttpContext.ChallengeAsync(KeycloakConstants.OpenIdConnectScheme,
            new AuthenticationProperties { RedirectUri = redirectUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(KeycloakConstants.OpenIdConnectScheme,
            new AuthenticationProperties { RedirectUri = Url.Action("Index", "Home") });
    }
}
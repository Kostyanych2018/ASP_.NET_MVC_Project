using GameStore.Application.Common.Constants;
using GameStore.UI.Extensions;
using GameStore.UI.Exceptions;
using GameStore.UI.Models.Cart;
using GameStore.UI.Services.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.UI.Controllers;

[Authorize(Policy = AuthConstants.UserPolicy)]
public class CartController : Controller
{
    private readonly IGameService _gameService;
    private readonly Cart _cart;
    private readonly ILogger<CartController> _logger;

    public CartController(
        IGameService gameService,
        Cart cart,
        ILogger<CartController> logger)
    {
        _gameService = gameService;
        _cart = cart;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(_cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int id, string? returnUrl, CancellationToken cancellationToken)
    {
        try
        {
            var game = await _gameService.GetGameByIdAsync(id, cancellationToken);
            _cart.AddToCart(game);
        }
        catch (ApiException ex)
        {
            _logger.LogApiException(ex, "Failed to add game with Id: {GameId} to cart.", id);
            TempData["ErrorMessage"] = "Failed to add the item to the cart.";
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int id, string? returnUrl)
    {
        _cart.RemoveItem(id);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear(string? returnUrl)
    {
        _cart.ClearAll();

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }
}
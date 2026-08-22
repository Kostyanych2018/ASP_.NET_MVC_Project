using GameStore.UI.Services.GameService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameStore.UI.Extensions;

namespace GameStore.UI.Controllers;

public class CartController : Controller
{
    private readonly IGameService _gameService;
    private readonly Cart _cart;

    public CartController(IGameService gameService, Cart cart)
    {
        _gameService = gameService;
        _cart = cart;
    }

    public IActionResult Index()
    {
        return View(_cart);
    }

    [Authorize]
    public async Task<IActionResult> Add(int id, string returnUrl)
    {
        var response = await _gameService.GetGameByIdAsync(id);
        if (response.Successful) {
            _cart.AddToCart(response.Data!);
        }
        return Redirect(returnUrl);
    }

    public IActionResult Remove(int id, string returnUrl)
    {
        _cart.RemoveItem(id);
        return Redirect(returnUrl);
    }
}
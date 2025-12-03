using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEB_353501_Gruganov.UI.Extensions;
using WEB_353501_Gruganov.UI.Services.GameService;

namespace WEB_353501_Gruganov.UI.Controllers;

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
        if (response.Successfull) {
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
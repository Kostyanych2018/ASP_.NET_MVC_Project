using Microsoft.AspNetCore.Mvc;
namespace GameStore.UI.Views.Shared.Components.Cart;

public class CartViewComponent : ViewComponent
{
    private readonly GameStore.Domain.Models.Cart _cart;

    public CartViewComponent(GameStore.Domain.Models.Cart cart)
    {
        _cart = cart;
    }

    public IViewComponentResult Invoke()
    {
        return View(_cart);
    }
}
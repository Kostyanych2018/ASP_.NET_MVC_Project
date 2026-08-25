using Microsoft.AspNetCore.Mvc;
namespace GameStore.UI.Views.Shared.Components.Cart;

public class CartViewComponent : ViewComponent
{
    private readonly Models.Cart.Cart _cart;

    public CartViewComponent(Models.Cart.Cart cart)
    {
        _cart = cart;
    }

    public IViewComponentResult Invoke()
    {
        return View(_cart);
    }
}
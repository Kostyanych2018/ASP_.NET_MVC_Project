using Microsoft.AspNetCore.Mvc;

namespace WEB_353501_Gruganov.UI.Views.Shared.Components.Cart;

public class CartViewComponent: ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
    
}
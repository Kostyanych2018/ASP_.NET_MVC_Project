using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Views.Shared.Components;

public class CartViewComponent: ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
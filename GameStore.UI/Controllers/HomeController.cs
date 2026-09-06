using Microsoft.AspNetCore.Mvc;

namespace GameStore.UI.Controllers;

public class HomeController: Controller
{
    
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
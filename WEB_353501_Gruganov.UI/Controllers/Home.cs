using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class Home : Controller
{
    // GET
    public IActionResult Index()
    {
        ViewData["Subtitle"] = "Лабораторная работа 2";

        var list = new List<ListDemo>()
        {
            new() { Id = 1, Name = "Item 1" },
            new() { Id = 2, Name = "Item 2" },
            new() { Id = 3, Name = "Item 3" },
        };

        ViewBag.List = new SelectList(list, "Id", "Name");

        return View();
    }
}
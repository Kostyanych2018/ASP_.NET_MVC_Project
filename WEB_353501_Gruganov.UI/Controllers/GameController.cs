using System.Security.AccessControl;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WEB_353501_Gruganov.UI.Extensions;
using WEB_353501_Gruganov.UI.Services.GameService;

namespace WEB_353501_Gruganov.UI.Controllers;

[Route("Catalog")]
public class GameController(IGameService gameService, IGenreService genreService) : Controller
{
    IGameService _gameService = gameService;
    IGenreService _genreService = genreService;

    [Route("{genre?}")]
    [Route("")]
    public async Task<IActionResult> Index(string? genre, int? pageSize = null, int pageNo = 1)
    {

        var genreResponse = await _genreService.GetGenresListAsync();
        if (!genreResponse.Successfull)
            return NotFound(genreResponse.Message);
        
        var gameResponse = await _gameService.GetGamesListAsync(genre, pageNo, pageSize);
        if (!gameResponse.Successfull)
            return NotFound(gameResponse.Message);

        var currentGenre = genreResponse.Data?.FirstOrDefault(g => g.NormalizedName == genre);
        var currentGenreName = currentGenre?.Name ?? "Все категории";

        ViewData["Title"] = "Каталог игр";
        ViewData["Genres"] = genreResponse.Data;
        ViewData["CurrentGenreName"] = currentGenreName;

        if (Request.IsAjaxRequest()) {
            return PartialView("_GameListPartial", gameResponse.Data);
        }
        
        return View(gameResponse.Data);
    }
}
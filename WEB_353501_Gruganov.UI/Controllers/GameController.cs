using System.Security.AccessControl;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WEB_353501_Gruganov.UI.Services.GameService;

namespace WEB_353501_Gruganov.UI.Controllers;

public class GameController(IGameService gameService, IGenreService genreService) : Controller
{
    IGameService _gameService = gameService;
    IGenreService _genreService = genreService;

    public async Task<IActionResult> Index(string? genre, int? pageSize, int pageNo = 1)
    {
        var gameResponse = await _gameService.GetGamesListAsync(genre, pageNo, pageSize);
        if (!gameResponse.Successfull)
            return NotFound(gameResponse.Message);

        var genreResponse = await _genreService.GetGenresListAsync();
        if (!genreResponse.Successfull)
            return NotFound(genreResponse.Message);

        var currentGenre = genreResponse.Data?.FirstOrDefault(g => g.NormalizedName == genre);
        var currentGenreName = currentGenre?.Name ?? "Все категории";

        ViewData["Title"] = "Каталог игр";
        ViewData["Genres"] = genreResponse.Data;
        ViewData["CurrentGenreName"] = currentGenreName;
        return View(gameResponse.Data);
    }
}
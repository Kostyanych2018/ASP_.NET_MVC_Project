using GameStore.Application.Common.Models;
using GameStore.Application.Games.DTOs;
using GameStore.Application.Genres.DTOs;
using GameStore.UI.Extensions;
using GameStore.UI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using GameStore.UI.Models;
using GameStore.UI.Services.Games;
using GameStore.UI.Services.Genres;

namespace GameStore.UI.Controllers;

public class GameController : Controller
{
    private readonly IGameService _gameService;
    private readonly IGenreService _genreService;
    private readonly ILogger<GameController> _logger;

    public GameController(
        IGameService gameService,
        IGenreService genreService,
        ILogger<GameController> logger)
    {
        _gameService = gameService;
        _genreService = genreService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? genre,
        int? pageSize = null,
        int pageNo = 1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var genresTask = _genreService.GetGenresListAsync(cancellationToken);
            var gamesTask = _gameService.GetGamesListAsync(genre, pageSize, pageNo, cancellationToken);

            await Task.WhenAll(genresTask, gamesTask);

            List<GenreDto> genres = await genresTask;
            ListModel<GameDto> games = await gamesTask;

            var currentGenre = genres.FirstOrDefault(g => string.Equals(g.NormalizedName, genre, StringComparison.OrdinalIgnoreCase));
            var currentGenreName = currentGenre?.Name ?? "All categories";

            ViewData["Title"] = "Game Catalog";
            ViewData["Genres"] = genres;
            ViewData["CurrentGenreName"] = currentGenreName;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_GameListPartial", games);
            }

            return View(games);
        }
        catch (ApiException ex)
        {
            _logger.LogApiException(ex, "Failed to load game catalog.");
            return View("Error", new ErrorViewModel { Message = "Failed to load the game catalog." });
        }
    }
}
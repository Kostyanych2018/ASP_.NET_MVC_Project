using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WEB_353501_Gruganov.Domain.Entities;
using WEB_353501_Gruganov.Domain.Models;
using WEB_353501_Gruganov.UI.Controllers;
using WEB_353501_Gruganov.UI.Services.GameService;
using WEB_353501_Gruganov.UI.Services.GenreService;

namespace WEB_353501_Gruganov.Tests;

public class GameControllerTests
{
    private readonly IGameService _gameService;
    private readonly IGenreService _genreService;

    private readonly GameController _controller;

    public GameControllerTests()
    {
        _gameService = Substitute.For<IGameService>();
        _genreService = Substitute.For<IGenreService>();
        _controller = new GameController(_gameService, _genreService);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task Index_ReturnsNotFound_WhenGenreServiceFails()
    {
        var genreReponse = ResponseData<List<Genre>>.Error("Не удалось получить список жанров");
        _genreService.GetGenresListAsync().Returns(genreReponse);

        var result = await _controller.Index(null);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Не удалось получить список жанров", notFoundResult.Value);
    }

    [Fact]
    public async Task Index_ReturnsNotFound_WhenGameServiceFails()
    {
        var genreResponse = ResponseData<List<Genre>>.Success([]);
        _genreService.GetGenresListAsync().Returns(genreResponse);

        var gameResponse = ResponseData<ListModel<Game>>.Error("Не удалось получить список игр");
        _gameService.GetGamesListAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int?>())
            .Returns(gameResponse);

        var result = await _controller.Index(null);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Не удалось получить список игр", notFoundResult.Value);
    }

    [Fact]
    public async Task Index_ReturnsViewWithData_OnSuccess()
    {
        var genres = new List<Genre>
        {
            new() { Name = "Стратегии", NormalizedName = "strategies" },
            new() { Name = "Ролевые игры (RPG)", NormalizedName = "rpg" },
            new() { Name = "Выживание", NormalizedName = "survival" },
            new() { Name = "Шутер", NormalizedName = "shooter" }
        };
        var games = new ListModel<Game>() {
            Items =
            [
                new()
                {
                    Name = "Total War: Warhammer III",
                    Description = "Стратегия в реальном времени",
                    Price = 149.99m,
                    Genre = genres.Find(g => g.NormalizedName.Equals("strategies"))
                },

                new()
                {
                    Name = "Civilization VI",
                    Description = "Пошаговая стратегия о развитии цивилизации",
                    Price = 119.99m,
                    Genre = genres.Find(g => g.NormalizedName.Equals("strategies"))
                },

                new()
                {
                    Name = "Stellaris",
                    Description = "Космическая глобальная стратегия",
                    Price = 109.99m,
                    Genre = genres.Find(g => g.NormalizedName.Equals("strategies"))
                },

                new()
                {
                    Name = "Elden Ring",
                    Description = "Экшн-РПГ с открытым миром",
                    Price = 159.99m,
                    Genre = genres.Find(g => g.NormalizedName.Equals("rpg"))
                },

                new()
                {
                    Name = "Cyberpunk 2077",
                    Description = "Научно-фантастическая РПГ",
                    Price = 144.99m,
                    Genre = genres.Find(g => g.NormalizedName.Equals("rpg"))
                }
            ]
        };
        var genreReponse = ResponseData<List<Genre>>.Success(genres);
        var gameResponse = ResponseData<ListModel<Game>>.Success(games);

        _genreService.GetGenresListAsync().Returns(genreReponse);
        _gameService.GetGamesListAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int?>())
            .Returns(gameResponse);

        var result = await _controller.Index(null);
        var viewResult = Assert.IsType<ViewResult>(result);

        Assert.Equal(games, viewResult.Model);
        Assert.Equal(genres, viewResult.ViewData["Genres"]);
        Assert.Equal("Все категории", viewResult.ViewData["CurrentGenreName"]);
    }

    [Fact]
    public async Task Index_ReturnsViewWithCorrectGenre_WhenGenreIsSpecified()
    {
        var genres = new List<Genre>
        {
            new() { Id = 1, Name = "Экшен", NormalizedName = "action" },
            new() { Id = 2, Name = "Стратегии", NormalizedName = "strategy" }
        };

        var games = new ListModel<Game> { Items = [] };
        var genreReponse = ResponseData<List<Genre>>.Success(genres);
        var gameResponse = ResponseData<ListModel<Game>>.Success(games);
        
        _genreService.GetGenresListAsync().Returns(genreReponse);
        _gameService.GetGamesListAsync("action", Arg.Any<int>(), Arg.Any<int?>())
            .Returns(gameResponse);
        
        var result = await _controller.Index("action");
        
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Экшен", viewResult.ViewData["CurrentGenreName"]);
    }
}
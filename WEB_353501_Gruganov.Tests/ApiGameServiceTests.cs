using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using WEB_353501_Gruganov.API.Data;
using WEB_353501_Gruganov.API.UseCases;
using WEB_353501_Gruganov.Domain.Entities;

namespace WEB_353501_Gruganov.Tests;

public class ApiGameServiceTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiGameServiceTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        SeedDatabse();

        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Scheme = "https",
                Host = new HostString("testhost.com")
            }
        };
        _httpContextAccessor.HttpContext.Returns(httpContext);
    }

    private void SeedDatabse()
    {
        using var context = new AppDbContext(_dbContextOptions);
        context.Database.EnsureCreated();

        var genres = new List<Genre>
        {
            new() { Id = 1, Name = "Экшен", NormalizedName = "action" },
            new() { Id = 2, Name = "Стратегии", NormalizedName = "strategy" }
        };
        context.AddRange(genres);

        var games = new List<Game>();
        for (int i = 1; i <= 5; i++) {
            games.Add(new Game { Id = i, Name = $"Экшен-Игра {i}", Price = 10, GenreId = 1 });
        }

        for (int i = 6; i <= 10; i++) {
            games.Add(new Game { Id = i, Name = $"Стратегическая-Игра {i - 5}", Price = 20, GenreId = 2 });
        }

        context.Games.AddRange(games);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetGamesListAsync_ReturnsFirstPageOfThreeItems()
    {
        await using var context = new AppDbContext(_dbContextOptions);
        var handler = new GetListOfGamesHandler(context, _httpContextAccessor);
        var request = new GetListOfGames(null, 1, 3);

        var result = await handler.Handle(request, CancellationToken.None);


        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Items.Count);
        Assert.Equal(1, result.Data.CurrentPage);
        var totalPages = (int)Math.Ceiling((double)10 / 3);
        Assert.Equal(totalPages, result.Data.TotalPages);
    }

    [Fact]
    public async Task GetGamesListAsync_RequestForSecondPage_ReturnsFirstPageOfThreeItems()
    {
        await using var context = new AppDbContext(_dbContextOptions);
        var handler = new GetListOfGamesHandler(context, _httpContextAccessor);
        var request = new GetListOfGames(null, 2, 3);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Items.Count);
        Assert.Equal(2, result.Data.CurrentPage);
        Assert.Equal("Экшен-Игра 4", result.Data.Items.First().Name);
    }

    [Fact]
    public async Task GetGamesListAsync_RequestWithGenre_ReturnsFilteredItems()
    {
        await using var context = new AppDbContext(_dbContextOptions);
        var handler = new GetListOfGamesHandler(context, _httpContextAccessor);
        var request = new GetListOfGames("strategy", 1, 3);

        var result = await handler.Handle(request, CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Items.Count);
        Assert.All(result.Data.Items, item => Assert.Equal(2, item.GenreId));
        Assert.Equal("Стратегическая-Игра 1", result.Data.Items.First().Name);
    }

    [Fact]
    public async Task GetGamesListAsync_PageSizeGreaterThanMax_ReturnsMaxPageSize()
    {
        await using var context = new AppDbContext(_dbContextOptions);
        var handler = new GetListOfGamesHandler(context, _httpContextAccessor);
        var request = new GetListOfGames(null, 1, 10);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(5, result.Data.Items.Count);
    }

    [Fact]
    public async Task GetGamesListAsync_PageNoGreaterThanTotalPages_ReturnsError()
    {
        await using var context = new AppDbContext(_dbContextOptions);
        var handler = new GetListOfGamesHandler(context, _httpContextAccessor);
        var request = new GetListOfGames(null, 5, 3);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        // int totalPages = (int)Math.Ceiling((double)10 / 3);
        // Assert.Equal(totalPages, result.Data.CurrentPage);
        Assert.False(result.Successfull);
        Assert.Equal("Номер страницы превышает максимальный",result.Message);
    }
}
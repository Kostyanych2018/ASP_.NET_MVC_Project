using GameStore.Application.Games.DTOs;
using GameStore.UI.Constants;
using GameStore.UI.Exceptions;
using GameStore.UI.Extensions;
using GameStore.UI.Services.Games;
using GameStore.UI.Services.Genres;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.UI.Areas.Admin.Pages.Games;

[Authorize(Policy = AuthConstants.AdminPolicy)]
public class EditModel : PageModel
{
    private readonly IGameService _gameService;
    private readonly IGenreService _genreService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        IGameService gameService,
        IGenreService genreService,
        ILogger<EditModel> logger, GameDto game)
    {
        _gameService = gameService;
        _genreService = genreService;
        _logger = logger;
        Game = game;
    }

    [BindProperty] public GameDto Game { get; set; }

    [BindProperty] public IFormFile? Image { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id == null) return NotFound();

        try
        {
            Game = await _gameService.GetGameByIdAsync(id.Value, cancellationToken);
            await LoadGenresSelectListAsync(cancellationToken);
            return Page();
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Игра с Id: {GameId} не найдена для редактирования.", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            await LoadGenresSelectListAsync(cancellationToken);
            return Page();
        }

        try
        {
            await _gameService.UpdateGameAsync(Game.Id, Game, Image, cancellationToken);
            return RedirectToPage("./Index");
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Ошибка при обновлении игры Id: {GameId}.", Game.Id);
            ModelState.AddApiException(ex);
            await LoadGenresSelectListAsync(cancellationToken);
            return Page();
        }
    }

    private async Task LoadGenresSelectListAsync(CancellationToken cancellationToken)
    {
        try
        {
            var genres = await _genreService.GetGenresListAsync(cancellationToken);
            ViewData["Genres"] = new SelectList(genres, "Id", "Name", Game.GenreId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось загрузить категории для редактирования игры.");
            ViewData["Genres"] = new SelectList(Enumerable.Empty<SelectListItem>());
        }
    }
}
using GameStore.Application.Games.DTOs;
using GameStore.UI.Constants;
using GameStore.UI.Exceptions;
using GameStore.UI.Services.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameStore.UI.Areas.Admin.Pages.Games;

[Authorize(Policy = AuthConstants.AdminPolicy)]
public class DeleteModel : PageModel
{
    private readonly IGameService _gameService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(
        IGameService gameService,
        ILogger<DeleteModel> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    [BindProperty] public GameDto Game { get; set; } = new GameDto();

    public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            Game = await _gameService.GetGameByIdAsync(id.Value, cancellationToken);
            return Page();
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Игра с Id: {GameId} не найдена для удаления.", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            await _gameService.DeleteGameAsync(id.Value, cancellationToken);
            return RedirectToPage("./Index");
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Не удалось удалить игру с Id: {GameId}.", id);
            ModelState.AddModelError(string.Empty, "Не удалось удалить игру.");
            return Page();
        }
    }
}
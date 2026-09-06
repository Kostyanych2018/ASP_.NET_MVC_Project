using GameStore.Application.Games.DTOs;
using GameStore.Application.Common;
using GameStore.Application.Common.Constants;
using GameStore.UI.Areas.Admin.Pages;
using GameStore.UI.Exceptions;
using GameStore.UI.Extensions;
using GameStore.UI.Services.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.UI.Areas.Admin.Pages.Games;

[Authorize(Policy = AuthConstants.AdminPolicy)]
public class DeleteModel : AdminPageModel
{
    private readonly IGameService _gameService;

    public DeleteModel(
        IGameService gameService,
        ILogger<DeleteModel> logger) : base(logger)
    {
        _gameService = gameService;
    }

    public GameDto Game { get; set; } = new();

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
            return HandleApiExceptionNotFound(ex, "Game with Id: {GameId} was not found for deletion.", id);
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
            var result = HandleApiExceptionForForm(ex, "Failed to delete game with Id: {GameId}.", id);

            try
            {
                Game = await _gameService.GetGameByIdAsync(id.Value, cancellationToken);
            }
            catch (ApiException reloadEx)
            {
                Logger.LogApiException(reloadEx, "Failed to reload game Id: {GameId} after delete error.", id);
            }

            return result;
        }
    }
}

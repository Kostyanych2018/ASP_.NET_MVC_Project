using GameStore.Application.Games.DTOs;
using GameStore.Application.Common;
using GameStore.Application.Common.Constants;
using GameStore.UI.Areas.Admin.Pages;
using GameStore.UI.Exceptions;
using GameStore.UI.Services.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.UI.Areas.Admin.Pages.Games;

[Authorize(Policy = AuthConstants.AdminPolicy)]
public class DetailsModel : AdminPageModel
{
    private readonly IGameService _gameService;

    public DetailsModel(
        IGameService gameService,
        ILogger<DetailsModel> logger) : base(logger)
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
            return HandleApiExceptionNotFound(ex, "Failed to load game details for Id: {GameId}.", id);
        }
    }
}

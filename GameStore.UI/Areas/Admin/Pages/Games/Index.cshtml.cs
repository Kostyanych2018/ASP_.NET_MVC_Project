using GameStore.Application.Games.DTOs;
using GameStore.Application.Common;
using GameStore.Application.Common.Constants;
using GameStore.Application.Common.Models;
using GameStore.UI.Areas.Admin.Pages;
using GameStore.UI.Exceptions;
using GameStore.UI.Extensions;
using GameStore.UI.Services.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.UI.Areas.Admin.Pages.Games;

[Authorize(Policy = AuthConstants.AdminPolicy)]
public class IndexModel : AdminPageModel
{
    private readonly IGameService _gameService;

    public IndexModel(
        IGameService gameService,
        ILogger<IndexModel> logger) : base(logger)
    {
        _gameService = gameService;
    }

    public ListModel<GameDto> Games { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(
        int pageNo = 1,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Games = await _gameService.GetGamesListAsync(null, pageSize, pageNo, cancellationToken);

            if (Request.IsAjaxRequest())
            {
                return Partial("_GamesTablePartial", this);
            }

            return Page();
        }
        catch (ApiException ex)
        {
            return HandleApiExceptionForList(
                ex,
                "Failed to load games list in admin panel.",
                "Failed to load the games list.");
        }
    }
}

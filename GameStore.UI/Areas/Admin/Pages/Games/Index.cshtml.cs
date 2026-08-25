using GameStore.Application.Games.DTOs;
using GameStore.UI.Constants;
using GameStore.UI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameStore.UI.Extensions;
using GameStore.UI.Services.Games;
using Microsoft.AspNetCore.Authorization;

namespace GameStore.UI.Areas.Admin.Pages.Games
{
    [Authorize(Policy = AuthConstants.AdminPolicy)]
    public class IndexModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IGameService gameService,
            ILogger<IndexModel> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        public ListModel<GameDto> Games { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(
            int pageNo = 1,
            int? pageSize = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Games = await _gameService.GetGamesListAsync(null, pageNo, pageSize, cancellationToken);

                if (Request.IsAjaxRequest())
                {
                    return Partial("_GamesTablePartial", this);
                }

                return Page();
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Ошибка загрузки списка игр в админ-панели.");
                return Page();
            }
        }
    }
}
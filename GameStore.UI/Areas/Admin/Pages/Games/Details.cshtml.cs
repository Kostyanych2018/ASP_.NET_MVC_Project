using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Application.Games.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameStore.UI.Constants;
using GameStore.UI.Exceptions;
using GameStore.UI.Services.Games;
using Microsoft.AspNetCore.Authorization;

namespace GameStore.UI.Areas.Admin.Pages.Games
{
    [Authorize(Policy = AuthConstants.AdminPolicy)]
    public class DetailsModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(
            IGameService gameService,
            ILogger<DetailsModel> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        public GameDto Game { get; set; } = new GameDto();

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
                _logger.LogError(ex, "Ошибка при получении деталей игры Id: {GameId}.", id);
                return NotFound();
            }
        }
    }
}
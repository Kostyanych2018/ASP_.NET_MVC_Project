using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GameStore.Domain.Entities;
using GameStore.UI.Services.GameService;
using GameStore.UI;

namespace GameStore.UI.Areas.Admin.Pages.Games
{
    public class DetailsModel : PageModel
    {
        private readonly IGameService _gameService;

        public DetailsModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        public Game Game { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) {
                return NotFound();
            }

            var game = await _gameService.GetGameByIdAsync(id.Value);

            if (game.Successful) {
                Game = game.Data;

                return Page();
            }

            return NotFound();
        }
    }
}
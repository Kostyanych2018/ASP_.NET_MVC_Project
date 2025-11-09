using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.Domain.Entities;
using WEB_353501_Gruganov.UI;
using WEB_353501_Gruganov.UI.Services.GameService;

namespace WEB_353501_Gruganov.UI.Areas.Admin.Pages.Games
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

            if (game.Successfull) {
                Game = game.Data;

                return Page();
            }

            return NotFound();
        }
    }
}
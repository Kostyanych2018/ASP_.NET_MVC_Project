using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.Domain.Entities;
using WEB_353501_Gruganov.UI;
using WEB_353501_Gruganov.UI.Services.GameService;

namespace WEB_353501_Gruganov.UI.Areas.Admin.Pages.Games
{
    public class EditModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly IGenreService _genreService;

        public EditModel(IGameService gameService, IGenreService genreService)
        {
            _gameService = gameService;
            _genreService = genreService;
        }

        [BindProperty] public Game Game { get; set; }

        [BindProperty] public IFormFile? Image { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) {
                return NotFound();
            }

            var game = await _gameService.GetGameByIdAsync(id.Value);

            if (game.Successfull) {
                Game = game.Data!;

                var genres = await _genreService.GetGenresListAsync();
                ViewData["Genres"] = new SelectList(genres.Data, "Id", "Name");
                return Page();
            }

            return NotFound();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) {
                var genres = await _genreService.GetGenresListAsync();
                ViewData["Genres"] = new SelectList(genres.Data, "Id", "Name");
                return Page();
            }

            await _gameService.UpdateGameAsync(Game.Id, Game, Image);

            return RedirectToPage("./Index");
        }
    }
}
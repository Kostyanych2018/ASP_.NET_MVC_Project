using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_353501_Gruganov.UI.Services.GameService;

namespace WEB_353501_Gruganov.UI.Areas.Admin.Pages.Games
{
    public class CreateModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly IGenreService _genreService;

        public CreateModel(IGameService gameService,IGenreService genreService)
        {
            _gameService = gameService;
            _genreService = genreService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var genres = await _genreService.GetGenresListAsync();
            ViewData["Genres"] = new SelectList(genres.Data, "Id", "Name");
            return Page();
        }

        [BindProperty] public Game Game { get; set; }
        [BindProperty] public IFormFile? Image { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) {
                return Page();
            }

            await _gameService.CreateGameAsync(Game, Image);
            
            return RedirectToPage("./Index");
            }
    }
}
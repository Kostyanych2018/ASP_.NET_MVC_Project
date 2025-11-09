using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.UI.Services.GameService;

namespace WEB_353501_Gruganov.UI.Areas.Admin.Pages.Games
{
    public class DeleteModel : PageModel
    {
        private readonly IGameService _gameService;

        public DeleteModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        [BindProperty]
        public Game Game { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _gameService.GetGameByIdAsync(id.Value);

            if (response.Successfull)
            {
                Game = response.Data!;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _gameService.DeleteGameAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
}

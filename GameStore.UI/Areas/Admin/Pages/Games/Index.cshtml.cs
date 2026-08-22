using GameStore.UI.Services.GameService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameStore.UI.Extensions;

namespace GameStore.UI.Areas.Admin.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly IGameService _gameService;

        public IndexModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        public ListModel<Game> Games { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageNo = 1, int? pageSize = null)
        {
            var response = await _gameService.GetGamesListAsync(null,pageNo, pageSize);
            if (response is { Successful: true, Data: not null }) {
                Games = response.Data;
            }

            if (Request.IsAjaxRequest()) {
                return Partial("_GamesTablePartial", this);
            }
            
            return Page();
        }
    }
}
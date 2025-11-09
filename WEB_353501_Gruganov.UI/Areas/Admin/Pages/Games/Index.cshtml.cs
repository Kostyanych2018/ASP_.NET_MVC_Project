using Microsoft.AspNetCore.Mvc.RazorPages;
using WEB_353501_Gruganov.UI.Services.GameService;

namespace WEB_353501_Gruganov.UI.Areas.Admin.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly IGameService _gameService;

        public IndexModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        public ListModel<Game> Games { get; set; }

        public async Task OnGetAsync(int pageNo = 1, int? pageSize = null)
        {
            var response = await _gameService.GetGamesListAsync(null,pageNo, pageSize);
            if (response is { Successfull: true, Data: not null }) {
                Games = response.Data;
            }
        }
    }
}
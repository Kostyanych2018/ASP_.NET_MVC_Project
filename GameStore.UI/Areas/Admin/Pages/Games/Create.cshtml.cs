using GameStore.Application.Games.DTOs;
using GameStore.UI.Constants;
using GameStore.UI.Exceptions;
using GameStore.UI.Extensions;
using GameStore.UI.Services.Games;
using GameStore.UI.Services.Genres;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.UI.Areas.Admin.Pages.Games
{
    [Authorize(Policy = AuthConstants.AdminPolicy)]
    public class CreateModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly IGenreService _genreService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            IGameService gameService,
            IGenreService genreService,
            ILogger<CreateModel> logger,
            GameDto game)
        {
            _gameService = gameService;
            _genreService = genreService;
            _logger = logger;
            Game = game;
        }

        [BindProperty] public GameDto  Game { get; set; }
        [BindProperty] public IFormFile? Image { get; set; }

        public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken=default)
        {
            await LoadGenresSelectListAsync(cancellationToken);
            return Page();
        }


        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                await LoadGenresSelectListAsync(cancellationToken);
                return Page();
            }

            try
            {
                await _gameService.CreateGameAsync(Game, Image, cancellationToken);
                return RedirectToPage("./Index");
            }
            catch (ApiException ex)
            {
                _logger.LogWarning(ex, "Ошибка валидации/создания игры через API.");
                ModelState.AddApiException(ex);
                await LoadGenresSelectListAsync(cancellationToken);
                return Page();
            }
        }
        
        private async Task LoadGenresSelectListAsync(CancellationToken cancellationToken)
        {
            try
            {
                var genres = await _genreService.GetGenresListAsync(cancellationToken);
                ViewData["Genres"] = new SelectList(genres, nameof(GameDto.Id), "Name");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось загрузить категории для формы создания игры.");
                ViewData["Genres"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }
    }
}
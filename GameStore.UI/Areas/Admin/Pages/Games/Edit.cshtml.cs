using System.Globalization;
using GameStore.Application.Common;
using GameStore.Application.Common.Constants;
using GameStore.Application.Games.DTOs;
using GameStore.Application.Genres.DTOs;
using GameStore.UI.Areas.Admin.Pages;
using GameStore.UI.Exceptions;
using GameStore.UI.Services.Games;
using GameStore.UI.Services.Genres;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.UI.Areas.Admin.Pages.Games;

[Authorize(Policy = AuthConstants.AdminPolicy)]
public class EditModel : AdminPageModel
{
    private readonly IGameService _gameService;
    private readonly IGenreService _genreService;

    public EditModel(
        IGameService gameService,
        IGenreService genreService,
        ILogger<EditModel> logger) : base(logger)
    {
        _gameService = gameService;
        _genreService = genreService;
    }

    [BindProperty] public int Id { get; set; }
    [BindProperty] public string Name { get; set; } = string.Empty;
    [BindProperty] public string? Description { get; set; }
    [BindProperty] public string Price { get; set; } = string.Empty;
    [BindProperty] public int GenreId { get; set; }
    [BindProperty] public string? CurrentImage { get; set; }
    [BindProperty] public IFormFile? Image { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var game = await _gameService.GetGameByIdAsync(id.Value, cancellationToken);
            Id = game.Id;
            Name = game.Name;
            Description = game.Description;
            Price = game.Price.ToString(CultureInfo.InvariantCulture);
            GenreId = game.GenreId;
            CurrentImage = game.Image;

            await LoadGenresSelectListAsync(cancellationToken);
            return Page();
        }
        catch (ApiException ex)
        {
            return HandleApiExceptionNotFound(ex, "Game with Id: {GameId} was not found for editing.", id);
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!PriceParser.TryParse(Price, out var parsedPrice))
        {
            ModelState.AddModelError(nameof(Price), "Please enter a valid price (e.g. 149.99 or 149,99).");
            await LoadGenresSelectListAsync(cancellationToken);
            return Page();
        }

        try
        {
            var gameDto = new GameDto
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Price = parsedPrice,
                GenreId = GenreId,
                Image = CurrentImage
            };

            await _gameService.UpdateGameAsync(Id, gameDto, Image, cancellationToken);
            return RedirectToPage("./Index");
        }
        catch (ApiException ex)
        {
            await LoadGenresSelectListAsync(cancellationToken);
            return HandleApiExceptionForForm(ex, "Failed to update game Id: {GameId}.", Id);
        }
    }

    private async Task LoadGenresSelectListAsync(CancellationToken cancellationToken)
    {
        try
        {
            var genres = await _genreService.GetGenresListAsync(cancellationToken);
            ViewData["Genres"] = new SelectList(genres, nameof(GenreDto.Id), "Name", GenreId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load genres for edit game form.");
            ViewData["Genres"] = new SelectList(Enumerable.Empty<SelectListItem>());
        }
    }
}

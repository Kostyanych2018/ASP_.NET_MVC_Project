using FluentValidation;

namespace GameStore.Application.Games.Commands.UpdateGame;

public class UpdateGameCommandValidator: AbstractValidator<UpdateGameCommand>
{
    public UpdateGameCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage(GameConstants.ErrorMessages.IdRequired);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(GameConstants.ErrorMessages.NameRequired)
            .Length(GameConstants.MinNameLength, GameConstants.MaxNameLength)
            .WithMessage(string.Format(
                GameConstants.ErrorMessages.NameLength,
                GameConstants.MinNameLength,
                GameConstants.MaxNameLength));

        RuleFor(x => x.Description)
            .MaximumLength(GameConstants.MaxDescriptionLength)
            .WithMessage(string.Format(
                GameConstants.ErrorMessages.DescriptionLength,
                GameConstants.MaxDescriptionLength));

        RuleFor(x => x.Price)
            .InclusiveBetween(GameConstants.MinPrice, GameConstants.MaxPrice)
            .WithMessage(string.Format(
                GameConstants.ErrorMessages.PriceRange,
                GameConstants.MinPrice,
                GameConstants.MaxPrice));

        RuleFor(x => x.GenreId)
            .GreaterThan(0)
            .WithMessage(GameConstants.ErrorMessages.GenreRequired);

        When(x => x.ImageStream != null && !string.IsNullOrWhiteSpace(x.ImageFileName), () =>
        {
            RuleFor(x => x.ImageFileName)
                .Must(HaveValidImageExtension)
                .WithMessage(string.Format(
                    GameConstants.ErrorMessages.InvalidImageFormat,
                    string.Join(", ", GameConstants.AllowedImageExtensions)));
        });
    }

    private bool HaveValidImageExtension(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return GameConstants.AllowedImageExtensions.Contains(extension);
    }
}

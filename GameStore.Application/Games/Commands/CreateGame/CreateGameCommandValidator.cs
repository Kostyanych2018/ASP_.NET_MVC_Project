using FluentValidation;

namespace GameStore.Application.Games.Commands.CreateGame;

public class CreateGameCommandValidator: AbstractValidator<CreateGameCommand>
{
    public CreateGameCommandValidator()
    {
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
                .Must(GameConstants.IsAllowedImageExtension)
                .WithMessage(string.Format(
                    GameConstants.ErrorMessages.InvalidImageFormat,
                    string.Join(", ", GameConstants.AllowedImageExtensions)));

            RuleFor(x => x.ImageFileSize)
                .NotNull()
                .LessThanOrEqualTo(GameConstants.MaxImageFileSizeBytes)
                .WithMessage(string.Format(
                    GameConstants.ErrorMessages.InvalidImageSize,
                    GameConstants.MaxImageFileSizeBytes / (1024 * 1024)));
        });
    }
}

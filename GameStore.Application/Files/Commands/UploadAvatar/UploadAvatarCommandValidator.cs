using FluentValidation;
using GameStore.Application.Games;

namespace GameStore.Application.Files.Commands.UploadAvatar;

public class UploadAvatarCommandValidator : AbstractValidator<UploadAvatarCommand>
{
    public UploadAvatarCommandValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage(GameConstants.ErrorMessages.FileRequired);

        When(x => x.FileStream != null, () =>
        {
            RuleFor(x => x.FileName)
                .NotEmpty()
                .Must(GameConstants.IsAllowedImageExtension)
                .WithMessage(string.Format(
                    GameConstants.ErrorMessages.InvalidImageFormat,
                    string.Join(", ", GameConstants.AllowedImageExtensions)));

            RuleFor(x => x.FileSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(GameConstants.MaxImageFileSizeBytes)
                .WithMessage(string.Format(
                    GameConstants.ErrorMessages.InvalidImageSize,
                    GameConstants.MaxImageFileSizeBytes / (1024 * 1024)));
        });
    }
}

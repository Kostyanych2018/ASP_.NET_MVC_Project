using FluentValidation;

namespace GameStore.Application.Games.Commands.DeleteGame;

public class DeleteGameCommandValidator: AbstractValidator<DeleteGameCommand>
{
    public DeleteGameCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage(GameConstants.ErrorMessages.IdRequired);
    }
}
using FluentValidation;

namespace GameStore.Application.Games.Queries.GetGameById;

public class GetGameByIdQueryValidator: AbstractValidator<GetGameByIdQuery>
{
    public GetGameByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage(GameConstants.ErrorMessages.IdRequired);
    }
}
using FluentValidation;
using GameStore.Application.Games.Queries.GetGameById;

namespace GameStore.Application.Games.Queries.GetGamesWithPagination;

public class GetGamesWithPaginationQueryValidator : AbstractValidator<GetGamesWithPaginationQuery>
{
    public GetGamesWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNo)
            .GreaterThanOrEqualTo(1)
            .WithMessage(GameConstants.ErrorMessages.InvalidPageNumber);
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, GameConstants.MaxPageSize)
            .WithMessage(string.Format(
                GameConstants.ErrorMessages.InvalidPageSize,
                GameConstants.MaxPageSize));
    }
}
using GameStore.Application.Common.Interfaces;
using GameStore.Application.Games.DTOs;
using GameStore.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Games.Queries.GetGameById;

public class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQuery, ResponseData<GameDto>>
{
    private readonly IApplicationDbContext _context;

    public GetGameByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseData<GameDto>> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var gameDto = await _context.Games
            .AsNoTracking()
            .Where(g => g.Id == request.Id)
            .Select(g => new GameDto()
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Price = g.Price,
                GenreId = g.GenreId,
                GenreName = g.Genre == null ? null : g.Genre.Name,
                Image = g.Image,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (gameDto == null)
        {
            return ResponseData<GameDto>.Error(
                string.Format(GameConstants.ErrorMessages.GameNotFound, request.Id));
        }

        return ResponseData<GameDto>.Success(gameDto);
    }
}
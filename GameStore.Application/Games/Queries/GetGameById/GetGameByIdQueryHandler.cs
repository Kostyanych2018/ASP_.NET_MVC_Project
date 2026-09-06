using GameStore.Application.Common.Constants;
using GameStore.Application.Common.Exceptions;
using GameStore.Application.Common.Interfaces;
using GameStore.Application.Games.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Games.Queries.GetGameById;

public class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQuery, GameDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public GetGameByIdQueryHandler(IApplicationDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cache = cacheService;
    }

    public async Task<GameDto> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheConstants.GameByIdKey(request.Id);
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                var gameDto = await _context.Games
                    .AsNoTracking()
                    .Where(g => g.Id == request.Id)
                    .Select(g => new GameDto
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description,
                        Price = g.Price,
                        GenreId = g.GenreId,
                        GenreName = g.Genre.Name,
                        Image = g.Image
                    })
                    .FirstOrDefaultAsync(ct);
                if (gameDto == null)
                {
                    throw new NotFoundException(string.Format(GameConstants.ErrorMessages.GameNotFound, request.Id));
                }

                return gameDto;
            },
            [CacheConstants.GamesTag],
            cancellationToken: cancellationToken);
    }
}
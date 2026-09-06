using GameStore.Application.Common.Constants;
using GameStore.Application.Common.Exceptions;
using GameStore.Application.Common.Interfaces;
using GameStore.Application.Common.Models;
using GameStore.Application.Games.DTOs;
using GameStore.Application.Games;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Games.Queries.GetGamesWithPagination;

public class GetGamesWithPaginationQueryHandler : IRequestHandler<GetGamesWithPaginationQuery, ListModel<GameDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;
    public GetGamesWithPaginationQueryHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<ListModel<GameDto>> Handle(GetGamesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheConstants.GamesListKey(request.GenreNormalizedName, request.PageNo, request.PageSize);
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                var query = _context.Games.AsNoTracking();
                if (!string.IsNullOrWhiteSpace(request.GenreNormalizedName))
                {
                    query = query.Where(g => g.Genre.NormalizedName == request.GenreNormalizedName);
                }
                var totalItems = await query.CountAsync(ct);
                var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)request.PageSize);
                if (request.PageNo > totalPages && totalItems > 0)
                {
                    throw new NotFoundException(GameConstants.ErrorMessages.PageNumberExceeded);
                }
           
                var items = await query
                    .OrderBy(g => g.Id)
                    .Skip((request.PageNo - 1) * request.PageSize)
                    .Take(request.PageSize)
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
                    .ToListAsync(ct);
                return new ListModel<GameDto>
                {
                    Items = items,
                    CurrentPage = request.PageNo,
                    TotalPages = totalPages
                };
            },
            [CacheConstants.GamesTag], 
            cancellationToken: cancellationToken);
    }
}
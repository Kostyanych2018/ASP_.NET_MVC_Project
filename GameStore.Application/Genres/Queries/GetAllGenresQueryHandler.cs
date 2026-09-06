using GameStore.Application.Common.Constants;
using GameStore.Application.Common.Interfaces;
using GameStore.Application.Genres.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Genres.Queries;

public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, List<GenreDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public GetAllGenresQueryHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<GenreDto>> Handle(
        GetAllGenresQuery request,
        CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            CacheConstants.AllGenresKey,
            async ct =>
            {
                return await _context.Genres
                    .AsNoTracking()
                    .Select(g => new GenreDto()
                    {
                        Id = g.Id,
                        Name = g.Name,
                        NormalizedName = g.NormalizedName,
                    })
                    .ToListAsync(ct);
            },
            [CacheConstants.GenresTag],
            cancellationToken);
    }
}
using GameStore.Application.Common.Interfaces;
using GameStore.Application.Genres.DTOs;
using GameStore.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Genres.Queries;

public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, ResponseData<List<GenreDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllGenresQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseData<List<GenreDto>>> Handle(
        GetAllGenresQuery request,
        CancellationToken cancellationToken)
    {
        var genres = await _context.Genres
            .AsNoTracking()
            .Select(g => new GenreDto()
            {
                Id = g.Id,
                Name = g.Name,
                NormalizedName = g.NormalizedName,
            })
            .ToListAsync(cancellationToken);

        return ResponseData<List<GenreDto>>.Success(genres);
    }
}
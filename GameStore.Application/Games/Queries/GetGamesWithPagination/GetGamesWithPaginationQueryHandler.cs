using GameStore.Application.Common.Interfaces;
using GameStore.Application.Games.DTOs;
using GameStore.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ValidationException = GameStore.Application.Common.Exceptions.ValidationException;

namespace GameStore.Application.Games.Queries.GetGamesWithPagination;

public class GetGamesWithPaginationQueryHandler : IRequestHandler<GetGamesWithPaginationQuery, ListModel<GameDto>>
{
    private readonly IApplicationDbContext _context;

    public GetGamesWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ListModel<GameDto>> Handle(GetGamesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Games.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.GenreNormalizedName))
        {
            query = query.Where(g => g.Genre != null && g.Genre.NormalizedName == request.GenreNormalizedName);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)request.PageSize);
        if (request.PageNo > totalPages && totalItems > 0)
        {
            throw new ValidationException(nameof(request.PageNo), GameConstants.ErrorMessages.PageNumberExceeded);
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
                GenreName = g.Genre != null ? g.Genre.Name : null,
                Image = g.Image
            })
            .ToListAsync(cancellationToken);

        var listModel = new ListModel<GameDto>
        {
            Items = items,
            CurrentPage = request.PageNo,
            TotalPages = totalPages
        };

        return listModel;
    }
}
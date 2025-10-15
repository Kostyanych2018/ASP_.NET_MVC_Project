using MediatR;
using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.API.Data;
using WEB_353501_Gruganov.Domain.Entities;
using WEB_353501_Gruganov.Domain.Models;
using HostingEnvironmentExtensions = Microsoft.Extensions.Hosting.HostingEnvironmentExtensions;

namespace WEB_353501_Gruganov.API.Use_Cases;

public sealed record GetListOfGames(
    string? genreNormalizedName,
    int pageNo = 1,
    int pageSize = 3) : IRequest<ResponseData<ListModel<Game>>>;

public class GetListOfGamesHandler(AppDbContext context) : IRequestHandler<GetListOfGames, ResponseData<ListModel<Game>>>
{
    private readonly int _maxPageSize = 20;

    public async Task<ResponseData<ListModel<Game>>> Handle(GetListOfGames request, CancellationToken cancellationToken)
    {
        int pageSize = Math.Min(_maxPageSize, request.pageSize);
        pageSize = Math.Max(1, pageSize);

        var query = context.Games.AsQueryable();

        if (!string.IsNullOrEmpty(request.genreNormalizedName)) {
            query = query.Where(g => g.Genre.NormalizedName == request.genreNormalizedName);
        }

        int totalItems = await query.CountAsync(cancellationToken);
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        int pageNo = Math.Max(1, Math.Min(totalPages, request.pageNo));
        var items = await query
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var listModel = new ListModel<Game>
        {
            Items = items,
            CurrentPage = pageNo,
            TotalPages = totalPages
        };
        return ResponseData<ListModel<Game>>.Success(listModel);
    }
}
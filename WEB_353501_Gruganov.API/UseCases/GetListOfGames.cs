using MediatR;
using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.API.Data;
using WEB_353501_Gruganov.Domain.Entities;
using WEB_353501_Gruganov.Domain.Models;

namespace WEB_353501_Gruganov.API.UseCases;

public sealed record GetListOfGames(
    string? genreNormalizedName,
    int pageNo = 1,
    int pageSize = 3) : IRequest<ResponseData<ListModel<Game>>>;

public class GetListOfGamesHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetListOfGames, ResponseData<ListModel<Game>>>
{
    private readonly int _maxPageSize = 5;

    public async Task<ResponseData<ListModel<Game>>> Handle(GetListOfGames request, CancellationToken cancellationToken)
    {
        int pageSize = Math.Min(_maxPageSize, request.pageSize);
        pageSize = Math.Max(1, pageSize);

        var query = context.Games.AsQueryable();
        query = query.Include(g=>g.Genre);

        if (!string.IsNullOrEmpty(request.genreNormalizedName)) {
            query = query.Where(g => g.Genre.NormalizedName == request.genreNormalizedName);
        }

        int totalItems = await query.CountAsync(cancellationToken);
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        // int pageNo = Math.Max(1, Math.Min(totalPages, request.pageNo));
        if (request.pageNo > totalPages) {
            return ResponseData<ListModel<Game>>.Error("Номер страницы превышает максимальный");
        }
        var games = await query
            .Skip((request.pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null) {
            return ResponseData<ListModel<Game>>.Error("Не удалось получить HTTP-контекст");
        }
        var scheme = httpContext.Request.Scheme;
        var host = httpContext.Request.Host.Value;

        foreach (var game in games) {
            if (!string.IsNullOrEmpty(game.Image) && !game.Image.StartsWith("http")) {
                game.Image = $"{scheme}://{host}/{game.Image}";
            }
        }

        var listModel = new ListModel<Game>
        {
            Items = games,
            CurrentPage = request.pageNo,
            TotalPages = totalPages
        };
        return ResponseData<ListModel<Game>>.Success(listModel);
    }
}
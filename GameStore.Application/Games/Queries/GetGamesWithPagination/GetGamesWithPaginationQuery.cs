using GameStore.Application.Common.Models;
using GameStore.Application.Games.DTOs;
using MediatR;

namespace GameStore.Application.Games.Queries.GetGamesWithPagination;

public class GetGamesWithPaginationQuery : IRequest<ListModel<GameDto>>
{
    public string? GenreNormalizedName { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    
    public GetGamesWithPaginationQuery(
        string? genreNormalizedName,
        int pageNo = GameConstants.DefaultPageNumber,
        int pageSize = GameConstants.DefaultPageSize)
    {
        GenreNormalizedName = genreNormalizedName;
        PageNo = pageNo;
        PageSize = pageSize;
    }
}
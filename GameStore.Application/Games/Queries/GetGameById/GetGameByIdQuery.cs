using GameStore.Application.Games.DTOs;
using MediatR;

namespace GameStore.Application.Games.Queries.GetGameById;

public class GetGameByIdQuery: IRequest<GameDto>
{
    public int Id { get; set; }

    public GetGameByIdQuery(int id)
    {
        Id = id;
    }
}
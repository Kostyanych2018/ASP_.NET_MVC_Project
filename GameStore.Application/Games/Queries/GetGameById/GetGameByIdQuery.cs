using GameStore.Application.Games.DTOs;
using GameStore.Domain.Models;
using MediatR;

namespace GameStore.Application.Games.Queries.GetGameById;

public class GetGameByIdQuery: IRequest<ResponseData<GameDto>>
{
    public int Id { get; set; }

    public GetGameByIdQuery(int id)
    {
        Id = id;
    }
}
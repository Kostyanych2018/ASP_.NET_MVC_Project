using GameStore.Domain.Models;
using MediatR;

namespace GameStore.Application.Games.Commands.DeleteGame;

public class DeleteGameCommand: IRequest<ResponseData<bool>>
{
    public int Id { get; set; }
    
    public DeleteGameCommand(int id)
    {
        Id = id;
    }
}
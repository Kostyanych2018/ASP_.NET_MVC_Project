using GameStore.Domain.Models;
using MediatR;

namespace GameStore.Application.Games.Commands.DeleteGame;

public class DeleteGameCommand: IRequest<bool>
{
    public int Id { get; set; }
    
    public DeleteGameCommand(int id)
    {
        Id = id;
    }
}
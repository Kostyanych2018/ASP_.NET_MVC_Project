using GameStore.Application.Games.DTOs;
using MediatR;

namespace GameStore.Application.Games.Commands.CreateGame;

public class CreateGameCommand: IRequest<GameDto>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int GenreId { get; set; }
    public Stream? ImageStream { get; set; }
    public string? ImageFileName { get; set; }
    
    public CreateGameCommand(
        string name,
        string? description,
        decimal price,
        int genreId,
        Stream? imageStream = null,
        string? imageFileName = null)
    {
        Name = name;
        Description = description;
        Price = price;
        GenreId = genreId;
        ImageStream = imageStream;
        ImageFileName = imageFileName;
    }
}
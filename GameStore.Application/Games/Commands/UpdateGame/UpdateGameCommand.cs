using GameStore.Application.Games.DTOs;
using GameStore.Domain.Models;
using MediatR;

namespace GameStore.Application.Games.Commands.UpdateGame;

public class UpdateGameCommand: IRequest<GameDto>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int GenreId { get; set; }
    public Stream? ImageStream { get; set; }
    public string? ImageFileName { get; set; }
    
    public UpdateGameCommand(
        int id,
        string name,
        string? description,
        decimal price,
        int genreId,
        Stream? imageStream = null,
        string? imageFileName = null)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        GenreId = genreId;
        ImageStream = imageStream;
        ImageFileName = imageFileName;
    }
}
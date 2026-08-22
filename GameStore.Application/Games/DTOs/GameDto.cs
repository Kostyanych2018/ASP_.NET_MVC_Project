namespace GameStore.Application.Games.DTOs;

public class GameDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public int GenreId { get; set; }
    public string? GenreName { get; set; }
}
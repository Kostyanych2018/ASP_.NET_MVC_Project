namespace GameStore.Domain.Entities;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}
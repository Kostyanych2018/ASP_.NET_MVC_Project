namespace GameStore.Application.Genres.DTOs;

public class GenreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
}
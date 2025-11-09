namespace WEB_353501_Gruganov.Domain.Entities;

public class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string NormalizedName { get; set; } = null!;
}
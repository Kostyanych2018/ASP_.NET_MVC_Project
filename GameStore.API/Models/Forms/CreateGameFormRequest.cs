namespace GameStore.API.Models.Forms;

public class CreateGameFormRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Price { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public IFormFile? File { get; set; }
}

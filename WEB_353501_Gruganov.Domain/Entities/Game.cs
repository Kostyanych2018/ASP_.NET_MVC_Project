using System.ComponentModel.DataAnnotations;

namespace WEB_353501_Gruganov.Domain.Entities;

public class Game
{
    public int Id { get; set; }
    
    [Display(Name = "Название")]
    public string Name { get; set; }
    
    [Display(Name = "Описание")]
    public string? Description { get; set; }
    [Display(Name = "Жанр")]
    public int GenreId { get; set; }
    
    public Genre? Genre { get; set; }
    
    [Display(Name = "Цена")]
    public decimal Price { get; set; }
    
    [Display(Name = "Изображение")]
    public string? Image { get; set; }
    // public ImageMimeType { get; set; }
}
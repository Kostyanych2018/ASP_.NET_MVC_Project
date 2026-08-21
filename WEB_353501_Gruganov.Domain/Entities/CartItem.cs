namespace WEB_353501_Gruganov.Domain.Entities;

public class CartItem
{
    public Game Game { get; set; } = null!;
    public int Count { get; set; }
}
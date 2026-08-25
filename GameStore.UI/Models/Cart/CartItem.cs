namespace GameStore.UI.Models.Cart;

public class CartItem
{
    public int GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => Price * Quantity;
}
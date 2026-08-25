using GameStore.Application.Games.DTOs;

namespace GameStore.UI.Models.Cart;

public class Cart
{
    public Dictionary<int, CartItem> Items { get; set; } = new();

    public virtual int TotalItems => Items.Values.Sum(item => item.Quantity);

    public virtual decimal TotalPrice => Items.Values.Sum(item => item.TotalPrice);

    public virtual void AddToCart(GameDto game)
    {
        if (Items.TryGetValue(game.Id, out var existingItem))
        {
            existingItem.Quantity++;
            return;
        }

        Items.Add(game.Id, new CartItem
        {
            GameId = game.Id,
            GameName = game.Name,
            Price = game.Price,
            Image = game.Image,
            Quantity = 1
        });
    }

    public virtual void RemoveItem(int id)
    {
        Items.Remove(id);
    }

    public virtual void ClearAll()
    {
        Items.Clear();
    }
}
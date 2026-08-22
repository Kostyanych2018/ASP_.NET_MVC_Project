using GameStore.Domain.Entities;

namespace GameStore.Domain.Models;

public class Cart
{
    public Dictionary<int, CartItem> CartItems { get; set; } = [];

    public virtual void AddToCart(Game game)
    {
        if (CartItems.TryGetValue(game.Id, out var item))
        {
            ++item.Count;
        }
        else
        {
            CartItems.Add(game.Id, new CartItem { Count = 1, Game = game });
        }
    }

    public virtual void RemoveItem(int id)
    {
        if (!CartItems.TryGetValue(id, out var item))
        {
            return;
        }

        --item.Count;

        if (item.Count <= 0)
        {
            CartItems.Remove(id);
        }
    }

    public virtual void ClearAll()
    {
        CartItems.Clear();
    }

    public int Count => CartItems.Sum(item => item.Value.Count);

    public decimal TotalPrice => CartItems.Sum(item => item.Value.Count * item.Value.Game.Price);
}
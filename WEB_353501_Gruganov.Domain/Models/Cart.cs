using WEB_353501_Gruganov.Domain.Entities;

namespace WEB_353501_Gruganov.Domain.Models;

public class Cart
{
    public Dictionary<int, CartItem> CartItems { get; set; } = [];

    public virtual void AddToCart(Game game)
    {
        if (CartItems.ContainsKey(game.Id)) {
            ++CartItems[game.Id].Count;
        }
        else {
            CartItems.Add(game.Id, new CartItem() { Count = 1, Game = game });
        }
    }

    public virtual void RemoveItem(int id)
    {
        if (CartItems.ContainsKey(id)) {
            --CartItems[id].Count;
            if (CartItems[id].Count <= 0) {
                CartItems.Remove(id);
            }
        }
    }

    public virtual void ClearAll()
    {
        CartItems.Clear();
    }

    public int Count => CartItems.Sum(item => item.Value.Count);

    public decimal TotalPrice => CartItems.Sum(item => item.Value.Count * item.Value.Game.Price);
}
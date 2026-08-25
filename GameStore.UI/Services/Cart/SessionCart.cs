using System.Text.Json.Serialization;
using GameStore.Application.Games.DTOs;
using GameStore.UI.Extensions;

namespace GameStore.UI.Services.Cart;

public class SessionCart : Models.Cart.Cart
{
    private const string CartKey = "GameStoreCartSession";
    [JsonIgnore] private ISession? _session;

    public static Models.Cart.Cart GetCart(IServiceProvider serviceProvider)
    {
        var session = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Session;
        var cart = session?.Get<SessionCart>(CartKey) ?? new SessionCart();
        cart._session = session;
        return cart;
    }

    public override void AddToCart(GameDto  game)
    {
        base.AddToCart(game);
        _session?.Set(CartKey, this);
    }

    public override void RemoveItem(int id)
    {
        base.RemoveItem(id);
        _session?.Set(CartKey, this);
    }

    public override void ClearAll()
    {
        base.ClearAll();
        _session?.Set(CartKey, this);
    }
}
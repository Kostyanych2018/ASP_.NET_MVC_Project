using System.Text.Json.Serialization;
using WEB_353501_Gruganov.UI.Extensions;

namespace WEB_353501_Gruganov.UI.Services.CartService;

public class SessionCart : Cart
{
    public const string CartKey = "Cart";
    [JsonIgnore] public ISession Session { get; set; }

    public static Cart GetCart(IServiceProvider serviceProvider)
    {
        var session = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Session;
        var cart = session?.Get<SessionCart>(CartKey) ?? new SessionCart();
        cart.Session = session;
        return cart;
    }

    public override void AddToCart(Game game)
    {
        base.AddToCart(game);
        Session?.Set(CartKey, this);
    }

    public override void RemoveItem(int id)
    {
        base.RemoveItem(id);
        Session?.Set(CartKey, this);
    }

    public override void ClearAll()
    {
        base.ClearAll();
        Session?.Set(CartKey, this);
    }
}
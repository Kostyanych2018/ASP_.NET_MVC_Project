namespace GameStore.UI.Settings;

public class KeycloakSettings
{
    public string Host { get; set; } = null!;
    public string Realm { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
}
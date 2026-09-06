namespace GameStore.UI.Models.Auth;

public class KeycloakCreateUserModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public bool EmailVerified { get; set; } = true;
    public Dictionary<string, string>? Attributes { get; set; }
    public List<KeycloakUserCredential>? Credentials { get; set; }
}

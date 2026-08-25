using GameStore.UI.Constants;

namespace GameStore.UI.Models.Auth;

public class KeycloakUserCredential
{
    public string Type { get; set; } = AuthConstants.PasswordCredentialType;
    public bool Temporary { get; set; } = false;
    public string Value { get; set; } = string.Empty;
}
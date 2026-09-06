namespace GameStore.Infrastructure.Settings;

public class FileStorageSettings
{
    public string BasePath { get; set; } = string.Empty;
    public string FolderName { get; set; } = "Images";
    public string DefaultImage { get; set; } = "Images/default_game.png";
    public string AvatarsSubFolder { get; set; } = "avatars";
    public string DefaultAvatar { get; set; } = "Images/avatars/avatar.png";
}

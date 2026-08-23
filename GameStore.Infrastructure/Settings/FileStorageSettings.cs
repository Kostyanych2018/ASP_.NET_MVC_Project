namespace GameStore.Infrastructure.Settings;

public class FileStorageSettings
{
    public string BasePath { get; set; }
    public string DefaultImage { get; set; }
    public string FolderName { get; set; }
    
    public FileStorageSettings(string basePath, string defaultImage, string folderName)
    {
        BasePath = basePath;
        DefaultImage = defaultImage;
        FolderName = folderName;
    }
}
using GameStore.Application.Common.Interfaces;
using GameStore.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameStore.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly FileStorageSettings _settings;
    private readonly ILogger<FileService> _logger;

    public FileService(
        IOptions<FileStorageSettings> settings,
        ILogger<FileService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public Task<string> SaveFileAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        return SaveToFolderAsync(stream, fileName, subFolder: null, cancellationToken);
    }
        

    public Task<string> SaveAvatarAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        return SaveToFolderAsync(stream, fileName, _settings.AvatarsSubFolder, cancellationToken);
    }

    public Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        if (IsProtectedImage(fileName))
        {
            _logger.LogInformation("Skipped deletion of protected image: {FileName}", fileName);
            return Task.CompletedTask;
        }

        var rootDirectory = GetRootDirectory();
        var fullPhysicalPath = Path.Combine(rootDirectory, fileName);

        if (File.Exists(fullPhysicalPath))
        {
            File.Delete(fullPhysicalPath);
            _logger.LogInformation("File deleted: {PhysicalPath}", fullPhysicalPath);
        }
        else
        {
            _logger.LogWarning("Attempted to delete file that does not exist: {PhysicalPath}", fullPhysicalPath);
        }

        return Task.CompletedTask;
    }

    private async Task<string> SaveToFolderAsync(
        Stream stream,
        string fileName,
        string? subFolder,
        CancellationToken cancellationToken)
    {
        var rootDirectory = GetRootDirectory();

        var targetFolder = string.IsNullOrWhiteSpace(subFolder)
            ? Path.Combine(rootDirectory, _settings.FolderName)
            : Path.Combine(rootDirectory, _settings.FolderName, subFolder);

        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        var fileExtension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var fullPhysicalPath = Path.Combine(targetFolder, uniqueFileName);

        await using (var fileStream = new FileStream(fullPhysicalPath, FileMode.Create, FileAccess.Write))
        {
            await stream.CopyToAsync(fileStream, cancellationToken);
        }

        _logger.LogInformation("File successfully written to disk at {PhysicalPath}", fullPhysicalPath);

        return string.IsNullOrWhiteSpace(subFolder)
            ? $"{_settings.FolderName}/{uniqueFileName}"
            : $"{_settings.FolderName}/{subFolder}/{uniqueFileName}";
    }

    private string GetRootDirectory() =>
        string.IsNullOrWhiteSpace(_settings.BasePath)
            ? Directory.GetCurrentDirectory()
            : _settings.BasePath;

    private bool IsProtectedImage(string fileName) =>
        string.Equals(fileName, _settings.DefaultImage, StringComparison.OrdinalIgnoreCase)
        || string.Equals(fileName, _settings.DefaultAvatar, StringComparison.OrdinalIgnoreCase);
}

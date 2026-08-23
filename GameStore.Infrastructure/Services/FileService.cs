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

    public async Task<string> SaveFileAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        var rootDirectory = string.IsNullOrWhiteSpace(_settings.BasePath) 
            ? Directory.GetCurrentDirectory()
            : _settings.BasePath;

        var targetFolder = Path.Combine(rootDirectory, _settings.FolderName);

        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        var fileExtension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var fullPhysicalPath = Path.Combine(targetFolder, uniqueFileName);

        using (var fileStream = new FileStream(fullPhysicalPath, FileMode.Create, FileAccess.Write))
        {
            await stream.CopyToAsync(fileStream, cancellationToken);
        }

        _logger.LogInformation("File successfully written to disk at {PhysicalPath}", fullPhysicalPath);

        return $"{_settings.FolderName}/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        if (string.Equals(fileName, _settings.DefaultImage, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Skipped deletion of the default image: {FileName}", fileName);
            return Task.CompletedTask;
        }

        var rootDirectory = string.IsNullOrWhiteSpace(_settings.BasePath)
            ? Directory.GetCurrentDirectory()
            : _settings.BasePath;

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
}

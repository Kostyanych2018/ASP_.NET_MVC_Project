namespace GameStore.Application.Common.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
}
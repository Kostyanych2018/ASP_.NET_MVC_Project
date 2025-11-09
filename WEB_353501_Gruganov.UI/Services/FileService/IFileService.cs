namespace WEB_353501_Gruganov.UI.Services.FileService;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file);
}
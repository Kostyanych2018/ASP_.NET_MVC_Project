namespace WEB_353501_Gruganov.UI.Services.FileService;

public class LocalFileService(IWebHostEnvironment environment) : IFileService
{
    public async Task<string> SaveFileAsync(IFormFile file)
    {
        var imagesPath = Path.Combine(environment.WebRootPath, "Images");
        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";

        var filePath = Path.Combine(imagesPath, fileName);

        await using (var fileStream = new FileStream(filePath, FileMode.Create)) {
            await file.CopyToAsync(fileStream);
        }

        var imageUrl = Path.Combine(imagesPath, fileName);
        return imageUrl;
    }
}
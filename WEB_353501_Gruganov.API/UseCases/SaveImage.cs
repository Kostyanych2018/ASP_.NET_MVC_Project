using MediatR;

namespace WEB_353501_Gruganov.API.UseCases;

public sealed record SaveImage(IFormFile file) : IRequest<string>;


public class SaveImageHandler(
    IWebHostEnvironment env,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<SaveImage, string>
{
    public async Task<string> Handle(SaveImage request, CancellationToken cancellationToken)
    {
        var imagesPath = Path.Combine(env.WebRootPath, "Images");
        var fileExtension = Path.GetExtension(request.file.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        
        var filePath = Path.Combine(imagesPath, fileName);

        await using (var fileStream = new FileStream(filePath, FileMode.Create)) {
            await request.file.CopyToAsync(fileStream,cancellationToken);
        }
        
        // var httpContext = httpContextAccessor.HttpContext;
        // if (httpContext == null) {
        //     throw new Exception("HttpContext is null");
        // }
        //
        // var requestScheme = httpContext.Request.Scheme;
        // var requestHost = httpContext.Request.Host.Value;
        var imageUrl = $"Images/{fileName}";
        return imageUrl;
    }
}
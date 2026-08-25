using GameStore.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController: ControllerBase
{
    private readonly IFileService _fileService;
    private const string AvatarsFolder = "avatars";

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<string>> UploadAvatar(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Файл не выбран.");
        }

        using var stream = file.OpenReadStream();
        var relativePath = await _fileService.SaveFileAsync(stream, file.FileName, cancellationToken);

        return Ok(relativePath);
    }
}
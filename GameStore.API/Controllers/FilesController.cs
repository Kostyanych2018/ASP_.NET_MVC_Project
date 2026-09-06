using GameStore.API.Models.Forms;
using GameStore.Application.Files.Commands.UploadAvatar;
using GameStore.Application.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

public class FilesController : BaseApiController
{
    [HttpPost("avatar")]
    [Authorize]
    [RequestSizeLimit(GameConstants.MaxImageFileSizeBytes)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<string>> UploadAvatar(
        [FromForm] UploadAvatarFormRequest request,
        CancellationToken cancellationToken)
    {
        Stream? fileStream = null;
        try
        {
            if (request.File != null && request.File.Length > 0)
            {
                fileStream = request.File.OpenReadStream();
            }

            var command = new UploadAvatarCommand(
                fileStream,
                request.File?.FileName ?? string.Empty,
                request.File?.Length ?? 0);
            fileStream = null;

            var relativePath = await Sender.Send(command, cancellationToken);
            return Ok(relativePath);
        }
        finally
        {
            fileStream?.Dispose();
        }
    }
}

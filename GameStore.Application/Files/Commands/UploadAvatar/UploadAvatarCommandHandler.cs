using GameStore.Application.Common.Interfaces;
using MediatR;

namespace GameStore.Application.Files.Commands.UploadAvatar;

public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, string>
{
    private readonly IFileService _fileService;

    public UploadAvatarCommandHandler(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<string> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _fileService.SaveAvatarAsync(
                request.FileStream!,
                request.FileName,
                cancellationToken);
        }
        finally
        {
            request.FileStream?.Dispose();
        }
    }
}

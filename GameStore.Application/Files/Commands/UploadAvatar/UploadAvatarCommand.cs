using MediatR;

namespace GameStore.Application.Files.Commands.UploadAvatar;

public class UploadAvatarCommand : IRequest<string>
{
    public Stream? FileStream { get; }
    public string FileName { get; }
    public long FileSize { get; }

    public UploadAvatarCommand(Stream? fileStream, string fileName, long fileSize)
    {
        FileStream = fileStream;
        FileName = fileName;
        FileSize = fileSize;
    }
}

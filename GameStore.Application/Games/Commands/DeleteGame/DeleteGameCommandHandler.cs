using GameStore.Application.Common.Interfaces;
using GameStore.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Games.Commands.DeleteGame;

public class DeleteGameCommandHandler: IRequestHandler<DeleteGameCommand, ResponseData<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public DeleteGameCommandHandler(
        IApplicationDbContext context,
        IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }
    
    public async Task<ResponseData<bool>> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (game == null)
        {
            return ResponseData<bool>
                .Error(string.Format(GameConstants.ErrorMessages.GameNotFound, request.Id));
        }
        
        if (!string.IsNullOrEmpty(game.Image))
        {
            await _fileService.DeleteFileAsync(game.Image, cancellationToken);
        }
        
        _context.Games.Remove(game);
        await _context.SaveChangesAsync(cancellationToken);

        return ResponseData<bool>.Success(true);
    }
}
using GameStore.Application.Common;
using GameStore.Application.Common.Exceptions;
using GameStore.Application.Common.Extensions;
using GameStore.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GameStore.Application.Common.Constants;
namespace GameStore.Application.Games.Commands.DeleteGame;

public class DeleteGameCommandHandler: IRequestHandler<DeleteGameCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly IUserContext _userContext;
    private readonly ICacheService _cache;
    public DeleteGameCommandHandler(
        IApplicationDbContext context,
        IFileService fileService,
        IUserContext userContext,
        ICacheService cache)
    {
        _context = context;
        _fileService = fileService;
        _userContext = userContext;
        _cache = cache;
    }
    
    public async Task<bool> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        _userContext.EnsureAdmin();
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (game == null)
        {
            throw new NotFoundException(string.Format(GameConstants.ErrorMessages.GameNotFound, request.Id));
        }
        
        if (!string.IsNullOrEmpty(game.Image))
        {
            await _fileService.DeleteFileAsync(game.Image, cancellationToken);
        }
        
        _context.Games.Remove(game);
        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByTagAsync(CacheConstants.GamesTag, cancellationToken);

        return true;
    }
}
using GameStore.Application.Common;
using GameStore.Application.Common.Exceptions;
using GameStore.Application.Common.Extensions;
using GameStore.Application.Common.Interfaces;
using GameStore.Application.Games.DTOs;
using GameStore.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GameStore.Application.Common.Constants;

namespace GameStore.Application.Games.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, GameDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly IUserContext _userContext;
    private readonly ICacheService _cache;
    public CreateGameCommandHandler(
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

    public async Task<GameDto> Handle(
        CreateGameCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _userContext.EnsureAdmin();
            var genre = await _context.Genres
                .FirstOrDefaultAsync(g => g.Id == request.GenreId, cancellationToken);

            if (genre == null)
            {
                throw new NotFoundException(string.Format(GameConstants.ErrorMessages.GenreNotFound, request.GenreId));
            }

            string? imagePath = null;
            if (request.ImageStream != null && !string.IsNullOrWhiteSpace(request.ImageFileName))
            {
                imagePath = await _fileService.SaveFileAsync(
                    request.ImageStream,
                    request.ImageFileName,
                    cancellationToken);
            }

            var game = new Game
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                GenreId = request.GenreId,
                Image = imagePath
            };

            await _context.Games.AddAsync(game, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByTagAsync(CacheConstants.GamesTag, cancellationToken);


            return new GameDto()
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description,
                Price = game.Price,
                GenreId = game.GenreId,
                GenreName = genre.Name,
                Image = game.Image
            };
        }
        finally
        {
            request.ImageStream?.Dispose();
        }
    }
}
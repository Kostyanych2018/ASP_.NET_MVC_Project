using GameStore.Application.Common.Exceptions;
using GameStore.Application.Common.Interfaces;
using GameStore.Application.Games.DTOs;
using GameStore.Domain.Entities;
using GameStore.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Games.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, GameDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public CreateGameCommandHandler(
        IApplicationDbContext context,
        IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<GameDto> Handle(
        CreateGameCommand request,
        CancellationToken cancellationToken)
    {
        var genre = await _context.Genres
            .FirstOrDefaultAsync(g => g.Id == request.GenreId, cancellationToken);

        if (genre == null)
        {
            throw new NotFoundException(string.Format(GameConstants.ErrorMessages.GenreNotFound, request.GenreId));
        }

        string? imagePath = null;
        if (request.ImageStream != null && !string.IsNullOrWhiteSpace(request.ImageFileName))
        {
            using (request.ImageStream)
            {
                imagePath = await _fileService.SaveFileAsync(
                    request.ImageStream,
                    request.ImageFileName,
                    cancellationToken);
            }
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

        var gameDto = new GameDto()
        {
            Id = game.Id,
            Name = game.Name,
            Description = game.Description,
            Price = game.Price,
            GenreId = game.GenreId,
            GenreName = genre.Name,
            Image = game.Image
        };

        return gameDto;
    }
}
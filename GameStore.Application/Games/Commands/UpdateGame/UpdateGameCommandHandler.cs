using GameStore.Application.Common.Interfaces;
using GameStore.Application.Games.DTOs;
using GameStore.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Games.Commands.UpdateGame;

public class UpdateGameCommandHandler : IRequestHandler<UpdateGameCommand, ResponseData<GameDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public UpdateGameCommandHandler(
        IApplicationDbContext context,
        IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<ResponseData<GameDto>> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (game == null)
        {
            return ResponseData<GameDto>
                .Error(string.Format(GameConstants.ErrorMessages.GameNotFound, request.Id));
        }

        var genre = await _context.Genres
            .FirstOrDefaultAsync(g => g.Id == request.GenreId, cancellationToken);

        if (genre == null)
        {
            return ResponseData<GameDto>
                .Error(string.Format(GameConstants.ErrorMessages.GenreNotFound, request.GenreId));
        }

        if (request.ImageStream != null && !string.IsNullOrWhiteSpace(request.ImageFileName))
        {
            if (!string.IsNullOrWhiteSpace(request.ImageFileName))
            {
                await _fileService.DeleteFileAsync(request.ImageFileName,cancellationToken);
            }
            
            game.Image = await _fileService.SaveFileAsync(
                request.ImageStream,
                request.ImageFileName,
                cancellationToken);
        }
        
        game.Name = request.Name;
        game.Description = request.Description;
        game.Price = request.Price;
        game.GenreId = request.GenreId;
        
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
        
        return ResponseData<GameDto>.Success(gameDto);
    }
}
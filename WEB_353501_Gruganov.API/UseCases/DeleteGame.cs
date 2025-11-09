using MediatR;
using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.API.Data;

namespace WEB_353501_Gruganov.API.UseCases;

public sealed record DeleteGame(int id) : IRequest<IResult>;

public class DeleteGameHandler(AppDbContext context, IWebHostEnvironment env)
    : IRequestHandler<DeleteGame, IResult>
{
    public async Task<IResult> Handle(DeleteGame request, CancellationToken cancellationToken)
    {
        var gameToDelete = await context.Games.FirstOrDefaultAsync(g => g.Id == request.id, cancellationToken);
        if (gameToDelete == null)
        {
            return Results.NotFound($"Игра с ID {request.id} не найдена.");
        }
        if (!string.IsNullOrEmpty(gameToDelete.Image) && !gameToDelete.Image.Contains("default"))
        {
            var fullPath = Path.Combine(env.WebRootPath, gameToDelete.Image.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        context.Games.Remove(gameToDelete);
        await context.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}
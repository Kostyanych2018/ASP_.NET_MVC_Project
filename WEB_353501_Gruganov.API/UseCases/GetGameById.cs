using MediatR;
using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.API.Data;
using WEB_353501_Gruganov.Domain.Entities;
using WEB_353501_Gruganov.Domain.Models;

namespace WEB_353501_Gruganov.API.UseCases;

public sealed record GetGameById(int id) : IRequest<ResponseData<Game>>;

public class GetGameByIdHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetGameById, ResponseData<Game>>
{
    public async Task<ResponseData<Game>> Handle(GetGameById request, CancellationToken cancellationToken)
    {
        var game = await context.Games
            .Include(g => g.Genre)
            .FirstOrDefaultAsync(g => g.Id == request.id, cancellationToken);
        
        if (game == null) {
            return ResponseData<Game>.Error($"Игра с ID {request.id} не найдена.");
        }
        
        var httpContext  = httpContextAccessor.HttpContext;
        if (httpContext != null) {
            var scheme = httpContext.Request.Scheme;
            var host = httpContext.Request.Host.Value;
            game.Image = $"{scheme}://{host}/{game.Image}";
        }

        return ResponseData<Game>.Success(game);
    }
}
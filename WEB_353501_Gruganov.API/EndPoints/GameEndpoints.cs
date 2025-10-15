using MediatR;
using Microsoft.AspNetCore.Mvc;
using WEB_353501_Gruganov.API.Use_Cases;
using WEB_353501_Gruganov.Domain.Entities;

namespace WEB_353501_Gruganov.API.EndPoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/games")
            .WithTags(nameof(Game))
            .WithOpenApi();

        group.MapGet("/{genreNormalizedName?}", async (
                IMediator mediator,
                string? genreNormalizedName,
                [FromQuery] int pageNo = 1,
                [FromQuery] int pageSize = 3) =>
            {
                var query = new GetListOfGames(genreNormalizedName, pageNo, pageSize);
                var response = await mediator.Send(query);
                return Results.Ok(response); 
            })
            .WithName("GetListOfGames")
            .WithOpenApi();
    }
}
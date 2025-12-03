using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using WEB_353501_Gruganov.API.UseCases;
using WEB_353501_Gruganov.Domain.Entities;

namespace WEB_353501_Gruganov.API.EndPoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/games")
            .WithTags(nameof(Game))
            .WithOpenApi()
            .DisableAntiforgery()
            .RequireAuthorization("admin");

        group.MapGet("/{genreNormalizedName?}", async (IMediator mediator,
                HybridCache cache,
                string? genreNormalizedName,
                [FromQuery] int pageNo = 1,
                [FromQuery] int pageSize = 3) =>
            {
                var cacheKey = $"games_{genreNormalizedName}_{pageNo}_{pageSize}";
                var query = new GetListOfGames(genreNormalizedName, pageNo, pageSize);
                var response = await cache.GetOrCreateAsync(cacheKey, async token => await mediator.Send(query, token),
                    new HybridCacheEntryOptions()
                    {
                        Expiration = TimeSpan.FromMinutes(3),
                        LocalCacheExpiration = TimeSpan.FromSeconds(35)
                    });

                return Results.Ok(response);
            })
            .WithName("GetListOfGames")
            .WithOpenApi()
            .AllowAnonymous();

        group.MapGet("/{id:int}", async (IMediator mediator, int id) =>
            {
                var query = new GetGameById(id);
                var response = await mediator.Send(query);
                return response.Successfull
                    ? Results.Ok(response)
                    : Results.NotFound(response);
            })
            .WithName("GetGameById")
            .WithOpenApi();


        group.MapPost("/", async (IMediator mediator, HttpContext context) =>
            {
                var command = new CreateGame(context.Request);
                var response = await mediator.Send(command);
                return response;
            })
            .WithName("CreateGame")
            .WithOpenApi();

        group.MapPut("/{id:int}", async (IMediator mediator, HttpContext context) =>
            {
                var command = new UpdateGame(context);
                return await mediator.Send(command);
            })
            .WithName("UpdateGame")
            .WithOpenApi();

        group.MapDelete("/{id:int}", async (IMediator mediator, int id) =>
            {
                var command = new DeleteGame(id);
                return await mediator.Send(command);
            })
            .WithName("DeleteGame")
            .WithOpenApi();
    }
}
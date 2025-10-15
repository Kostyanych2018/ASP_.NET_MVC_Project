using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.API.Data;
using WEB_353501_Gruganov.Domain.Entities;

namespace WEB_353501_Gruganov.API.EndPoints;

public static class GenreEndpoints
{
    public static void MapGenreEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/genres")
            .WithTags(nameof(Genre))
            .WithOpenApi();

        group.MapGet("/", async (AppDbContext db) =>
            {
                var genres = await db.Genres.ToListAsync();
                return Results.Ok(genres);
            })
            .WithName("GetGenres");

        group.MapGet("/{id}", async (int id, AppDbContext db) =>
            {
                var genre = await db.Genres.FindAsync(id);

                return genre is null
                    ? Results.NotFound()
                    : Results.Ok(genre);
            })
            .WithName("GetGenreById");
    }
}
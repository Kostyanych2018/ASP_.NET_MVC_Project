using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.API.Data;
using WEB_353501_Gruganov.Domain.Entities;

namespace WEB_353501_Gruganov.API.UseCases;

public sealed record UpdateGame(HttpContext context) : IRequest<IResult>;

public class UpdateGameHandler(AppDbContext context, IMediator mediator, IWebHostEnvironment env) : IRequestHandler<UpdateGame, IResult>
{
    public async Task<IResult> Handle(UpdateGame updateGameRequest, CancellationToken cancellationToken)
    {
        var request = updateGameRequest.context.Request;
        if (!request.RouteValues.TryGetValue("id", out var idObj) || !int.TryParse(idObj?.ToString(), out var id)) {
            return Results.BadRequest("Неверный или отсутствующий ID.");
        }

        var form = await request.ReadFormAsync(cancellationToken);
        var gameJson = form["game"];
        if (string.IsNullOrEmpty(gameJson)) {
            return Results.BadRequest("Отсутствуют данные об игре (game).");
        }

        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        Game? game;
        try {
            game = JsonSerializer.Deserialize<Game>(gameJson, options);
            if (game == null) {
                return Results.BadRequest("Не удалось десериализовать JSON-данные.");
            }
        }
        catch (JsonException ex) {
            return Results.BadRequest($"Ошибка в формате JSON: {ex.Message}");
        }

        var gameToUpdate = await context.Games.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        if (gameToUpdate == null) {
            return Results.NotFound($"Игра с ID {id} не найдена");
        }

        gameToUpdate.Name = game.Name;
        gameToUpdate.Description = game.Description;
        gameToUpdate.Price = game.Price;
        gameToUpdate.GenreId = game.GenreId;


        var file = form.Files.GetFile("Image");
        if (file != null) {
            var oldImagePath = gameToUpdate.Image;
            var newImageUrl = await mediator.Send(new SaveImage(file), cancellationToken);
            gameToUpdate.Image = newImageUrl;

            if (!string.IsNullOrEmpty(oldImagePath) && !oldImagePath.Contains("default")) {
                var systemPath = oldImagePath.Replace('/', Path.DirectorySeparatorChar);
                var fullPath = Path.Combine(env.WebRootPath, systemPath.TrimStart('/'));
                if (File.Exists(fullPath)) {
                    File.Delete(fullPath);
                }
            }
        }

        context.Games.Update(gameToUpdate);
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(gameToUpdate);
    }
}
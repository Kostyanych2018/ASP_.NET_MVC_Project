using System.Text.Json;
using MediatR;
using WEB_353501_Gruganov.API.Data;
using WEB_353501_Gruganov.Domain.Entities;

namespace WEB_353501_Gruganov.API.UseCases;

public sealed record CreateGame(HttpRequest Request) : IRequest<IResult>;

public class CreateGameHandler(AppDbContext context, IMediator mediator, IConfiguration config)
    : IRequestHandler<CreateGame, IResult>
{
    public async Task<IResult> Handle(CreateGame createGameRequest, CancellationToken cancellationToken)
    {
        var request = createGameRequest.Request;
        var form = await request.ReadFormAsync(cancellationToken);

        var gameJson = form["game"];
        if (string.IsNullOrEmpty(gameJson)) {
            return Results.BadRequest("Отсутствует обязательная часть 'game' с JSON-данными.");
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

        var file = form.Files.GetFile("Image");
        string? imageUrl;
        if (file != null) {
            imageUrl = await mediator.Send(new SaveImage(file), cancellationToken);
        }
        else {
            imageUrl = config.GetValue<string>("DefaultGameImage");
        }

        game.Image = imageUrl;


        await context.Games.AddAsync(game, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Results.Created($"api/Game/{game.Id}", game);
    }
}
using GameStore.API.Models.Forms;
using GameStore.Application.Common;
using GameStore.Application.Games.Commands.CreateGame;
using GameStore.Application.Games.Commands.UpdateGame;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Mapping;

public static class GameFormMapper
{
    private const string InvalidPriceMessage = "Enter a valid price (e.g. 149.99 or 149,99).";

    public static GameFormMapResult<CreateGameCommand> ToCreateCommand(CreateGameFormRequest request)
    {
        if (!PriceParser.TryParse(request.Price, out var price))
        {
            return new GameFormMapResult<CreateGameCommand> { ValidationError = InvalidPriceValidationError() };
        }

        var (imageStream, fileName, fileSize) = OpenImage(request.File);

        return new GameFormMapResult<CreateGameCommand>
        {
            Command = new CreateGameCommand(
                request.Name,
                request.Description,
                price,
                request.GenreId,
                imageStream,
                fileName,
                fileSize)
        };
    }

    public static GameFormMapResult<UpdateGameCommand> ToUpdateCommand(int id, UpdateGameFormRequest request)
    {
        if (!PriceParser.TryParse(request.Price, out var price))
        {
            return new GameFormMapResult<UpdateGameCommand>
            {
                ValidationError = InvalidPriceValidationError()
            };
        }

        var (imageStream, fileName, fileSize) = OpenImage(request.File);

        return new GameFormMapResult<UpdateGameCommand>
        {
            Command = new UpdateGameCommand(
                id,
                request.Name,
                request.Description,
                price,
                request.GenreId,
                imageStream,
                fileName,
                fileSize)
        };
    }

    private static (Stream? ImageStream, string? FileName, long? FileSize) OpenImage(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return (null, null, null);
        }

        return (file.OpenReadStream(), file.FileName, file.Length);
    }

    private static ValidationProblemDetails InvalidPriceValidationError() =>
        new(new Dictionary<string, string[]>
        {
            ["Price"] = [InvalidPriceMessage]
        });
}
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Mapping;

public sealed class GameFormMapResult<TCommand> where TCommand : class
{
    public TCommand? Command { get; set; }
    public ValidationProblemDetails? ValidationError { get; set; }

    public bool IsSuccess => ValidationError == null && Command != null;
}
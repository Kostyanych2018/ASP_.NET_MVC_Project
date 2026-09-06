using GameStore.API.Mapping;
using GameStore.API.Models.Forms;
using GameStore.Application.Common.Constants;
using GameStore.Application.Common.Models;
using GameStore.Application.Games;
using GameStore.Application.Games.Commands.DeleteGame;
using GameStore.Application.Games.DTOs;
using GameStore.Application.Games.Queries.GetGameById;
using GameStore.Application.Games.Queries.GetGamesWithPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

public class GamesController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(ListModel<GameDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ListModel<GameDto>>> GetGames(
        [FromQuery] string? genre,
        [FromQuery] int pageNo = 1,
        [FromQuery] int pageSize = 3,
        CancellationToken cancellationToken = default)
    {
        var query = new GetGamesWithPaginationQuery(genre, pageNo, pageSize);
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetGameByIdQuery(id);
        var result = await Sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AuthConstants.AdminPolicy)]
    [RequestSizeLimit(GameConstants.MaxImageFileSizeBytes)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GameDto>> Create(
        [FromForm] CreateGameFormRequest request,
        CancellationToken cancellationToken)
    {
        var mapResult = GameFormMapper.ToCreateCommand(request);
        if (!mapResult.IsSuccess)
        {
            return ValidationProblem(mapResult.ValidationError!);
        }

        Stream? imageStream = mapResult.Command!.ImageStream;
        try
        {
            var result = await Sender.Send(mapResult.Command, cancellationToken);
            imageStream = null;
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        finally
        {
            imageStream?.Dispose();
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthConstants.AdminPolicy)]
    [RequestSizeLimit(GameConstants.MaxImageFileSizeBytes)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GameDto>> Update(
        int id,
        [FromForm] UpdateGameFormRequest request,
        CancellationToken cancellationToken)
    {
        var mapResult = GameFormMapper.ToUpdateCommand(id, request);
        if (!mapResult.IsSuccess)
        {
            return ValidationProblem(mapResult.ValidationError!);
        }

        Stream? imageStream = mapResult.Command!.ImageStream;
        try
        {
            var result = await Sender.Send(mapResult.Command, cancellationToken);
            imageStream = null;
            return Ok(result);
        }
        finally
        {
            imageStream?.Dispose();
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthConstants.AdminPolicy)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<bool>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteGameCommand(id);
        var result = await Sender.Send(command, cancellationToken);

        return Ok(result);
    }
}

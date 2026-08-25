using GameStore.Application.Games.Commands.CreateGame;
using GameStore.Application.Games.Commands.DeleteGame;
using GameStore.Application.Games.Commands.UpdateGame;
using GameStore.Application.Games.DTOs;
using GameStore.Application.Games.Queries.GetGameById;
using GameStore.Application.Games.Queries.GetGamesWithPagination;
using GameStore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

public class GamesController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(ListModel<GameDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ListModel<GameDto>>> GetGames(
        string? genre,
        int pageNo = 1,
        int pageSize = 3,
        CancellationToken cancellationToken = default)
    {
        var query = new GetGamesWithPaginationQuery(genre, pageNo, pageSize);
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetGameByIdQuery(id);
        var result = await Sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameDto>> Create(
        string name,
        string? description,
        decimal price,
        int genreId,
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        Stream? imageStream = null;
        string? fileName = null;

        if (file != null && file.Length > 0)
        {
            imageStream = file.OpenReadStream();
            fileName = file.FileName;
        }

        var command = new CreateGameCommand(
            name,
            description,
            price,
            genreId,
            imageStream,
            fileName);

        var result = await Sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> Update(
        int id,
        string name,
        string? description,
        decimal price,
        int genreId,
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        Stream? imageStream = null;
        string? fileName = null;

        if (file != null && file.Length > 0)
        {
            imageStream = file.OpenReadStream();
            fileName = file.FileName;
        }

        var command = new UpdateGameCommand(
            id,
            name,
            description,
            price,
            genreId,
            imageStream,
            fileName);

        var result = await Sender.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteGameCommand(id);
        var result = await Sender.Send(command, cancellationToken);

        return Ok(result);
    }
}
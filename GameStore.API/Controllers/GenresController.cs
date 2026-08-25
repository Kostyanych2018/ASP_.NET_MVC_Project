using GameStore.Application.Genres.DTOs;
using GameStore.Application.Genres.Queries;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

public class GenresController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<GenreDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GenreDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetAllGenresQuery(), cancellationToken);
        return Ok(result);
    }
}
using GameStore.Application.Genres.DTOs;
using GameStore.Domain.Models;
using MediatR;

namespace GameStore.Application.Genres.Queries;

public class GetAllGenresQuery : IRequest<ResponseData<List<GenreDto>>>
{
}
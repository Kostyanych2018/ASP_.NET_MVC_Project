using GameStore.Application.Genres.DTOs;
using MediatR;

namespace GameStore.Application.Genres.Queries;

public class GetAllGenresQuery : IRequest<List<GenreDto>>
{
}
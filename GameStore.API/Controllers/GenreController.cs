using GameStore.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Domain.Entities;

namespace GameStore.API.Controllers;

[ApiController] 
[Route("api/[controller]")]
public class GenreController: ControllerBase
{
    private readonly AppDbContext _context;
    public GenreController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
    {
        return await _context.Genres.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Genre>> GetGenre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null) {
            return NotFound();
        }
        return genre;
    }
}
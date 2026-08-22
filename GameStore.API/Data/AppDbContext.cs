using Microsoft.EntityFrameworkCore;
using GameStore.Domain.Entities;

namespace GameStore.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Game> Games { get; set; }
    public DbSet<Genre> Genres  { get; set; }
}
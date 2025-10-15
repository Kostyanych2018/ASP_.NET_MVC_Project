using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.Domain.Entities;

namespace WEB_353501_Gruganov.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Game> Games { get; set; }
    public DbSet<Genre> Genres  { get; set; }
}
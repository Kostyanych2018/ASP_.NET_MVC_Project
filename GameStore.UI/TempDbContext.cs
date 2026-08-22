using Microsoft.EntityFrameworkCore;

namespace GameStore.UI;

public class TempDbContext: DbContext
{
    public DbSet<Game> Games { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite();
    }
}
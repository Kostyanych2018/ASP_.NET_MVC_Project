using Microsoft.EntityFrameworkCore;

namespace WEB_353501_Gruganov.UI;

public class TempDbContext: DbContext
{
    public DbSet<Game> Games { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite();
    }
}
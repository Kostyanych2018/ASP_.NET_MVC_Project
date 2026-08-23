using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Description)
            .HasMaxLength(1000);

        builder.Property(g => g.Price)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(g => g.Image)
            .HasMaxLength(500);

        builder.HasOne(g => g.Genre)
            .WithMany()
            .HasForeignKey(g => g.GenreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
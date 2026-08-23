using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Data.Configurations;

public class GenreConfiguration: IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genres");
        
        builder.HasKey(g => g.Id);
        
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(g => g.NormalizedName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(g => g.NormalizedName)
            .IsUnique();
    }
}
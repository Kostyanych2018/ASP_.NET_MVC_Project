using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Game> Games { get; }
    DbSet<Genre> Genres { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
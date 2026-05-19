using Microsoft.EntityFrameworkCore;
using Connecions.Api.Models;

namespace Connecions.Api.Data;

public class ConnectionsContext(DbContextOptions<ConnectionsContext> options) : DbContext(options)
{
    public DbSet<Puzzle> Puzzles => Set<Puzzle>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Word> Words => Set<Word>();
    public DbSet<DailyPuzzle> DailyPuzzle => Set<DailyPuzzle>();
    public DbSet<GameState> GameState => Set<GameState>();

    protected override void OnModelCreating(ModelBuilder ModelBuilder)
    {
        ModelBuilder.Entity<DailyPuzzle>()
            .HasIndex(dp => dp.Date)
            .IsUnique();
    }
}

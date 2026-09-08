using Microsoft.EntityFrameworkCore;
using Connecions.Api.Models;

namespace Connecions.Api.Data;

public class ConnectionsContext(DbContextOptions<ConnectionsContext> options) : DbContext(options)
{
    public DbSet<Puzzle> Puzzles => Set<Puzzle>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Word> Words => Set<Word>();
    public DbSet<DailyPuzzle> DailyPuzzles => Set<DailyPuzzle>();
    public DbSet<GameState> GameStates => Set<GameState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DailyPuzzle>()
            .HasIndex(dp => dp.Date)
            .IsUnique();

        modelBuilder.Entity<DailyPuzzle>()
            .Property(dp => dp.Date)
            .HasConversion(
                date => date.ToString("yyyy-MM-dd"),
                value => DateOnly.Parse(value));

        modelBuilder.Entity<Puzzle>()
            .Property(p => p.LastUsed)
            .HasConversion(
                date => date.HasValue
                    ? date.Value.ToString("yyyy-MM-dd")
                    : null,
                value => value == null
                    ? null
                    : DateOnly.Parse(value));
    }
}

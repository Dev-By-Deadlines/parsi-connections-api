using Microsoft.EntityFrameworkCore;
using Connecions.Api.Models;

namespace QuoteVault.Api.Data;

public class ConnectionsContext(DbContextOptions<ConnectionsContext> options) : DbContext(options)
{
    public DbSet<Puzzle> Puzzles => Set<Puzzle>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Difficulty> Difficulties => Set<Difficulty>();
}

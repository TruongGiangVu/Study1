using GenApi.Models;

using Microsoft.EntityFrameworkCore;

namespace GenApi.Repositories;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Anime> Anime { get; set; } = default!;
}

using Microsoft.EntityFrameworkCore;
using Models.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        //dotnet ef migrations add InitialCreate
        //dotnet ef database update


    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LobbyGameData>().HasKey(e => e.Id);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<LobbyGameData> LobbyGameData { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Player> Players { get; set; }

}
using Backend.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();

            // UNIQUE INDEX on Email
            entity.HasIndex(e => e.Email).IsUnique();

            // Index on LastLoginTime for sorting
            entity.HasIndex(e => e.LastLoginTime);
        });
    }
}

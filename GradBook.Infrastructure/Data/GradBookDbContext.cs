using GradBook.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GradBook.Infrastructure.Data;

public class GradBookDbContext : DbContext
{
    public GradBookDbContext(DbContextOptions<GradBookDbContext> options) : base(options) { }

    public DbSet<Graduate> Graduates { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Memory> Memories { get; set; }
    public DbSet<Visitor> Visitors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Graduate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(300);
            entity.Property(e => e.Bio).HasMaxLength(2000);
            entity.Property(e => e.MainImageUrl).HasMaxLength(500);

            // Seed data
            entity.HasData(new Graduate
            {
                Id = 1,
                FullName = "Eng. Mohammed Abu-Issa",
                Title = "Bachelor of Engineering",
                Bio = "A journey of dedication, perseverance, and passion. Today marks the beginning of a new chapter — one filled with endless possibilities and the promise of great achievements.",
                MainImageUrl = "mohammad.jpg",
                GraduationDate = new DateTime(2026, 7, 22)
            });
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SenderName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.ReactionType).HasMaxLength(10);
        });

        modelBuilder.Entity<Memory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
        });
    }
}

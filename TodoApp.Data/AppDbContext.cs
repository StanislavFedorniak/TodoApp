using Microsoft.EntityFrameworkCore;
using TodoApp.Core.Entities;

namespace TodoApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TodoTask> Tasks { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<TodoTask>(entity =>
        {
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.PasswordHash)
                .IsRequired();
        });
    }
}
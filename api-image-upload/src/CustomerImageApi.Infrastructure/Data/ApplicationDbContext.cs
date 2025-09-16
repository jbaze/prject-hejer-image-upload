using CustomerImageApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerImageApi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<CustomerImage> CustomerImages { get; set; }
    public DbSet<LeadImage> LeadImages { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.EstimatedTime).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

            entity.HasMany(e => e.Images)
                  .WithOne(e => e.Customer)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Source).HasMaxLength(200);
            entity.Property(e => e.EstimatedTime).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

            entity.HasMany(e => e.Images)
                  .WithOne(e => e.Lead)
                  .HasForeignKey(e => e.LeadId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CustomerImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Base64Data).IsRequired();
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.ContentType).HasMaxLength(100);
        });

        modelBuilder.Entity<LeadImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Base64Data).IsRequired();
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.ContentType).HasMaxLength(100);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
            
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Seed default administrator user
        SeedDefaultUser(modelBuilder);
    }

    private static void SeedDefaultUser(ModelBuilder modelBuilder)
    {
        // For now, keep the simple password for the seed data
        // This will be updated when proper password hashing is implemented
        var defaultPassword = "Administrator1!";
        
        var defaultUser = new User
        {
            Id = 1,
            Email = "administrator@localhost",
            PasswordHash = defaultPassword, // Using plain text for now to match the current authentication
            FirstName = "System",
            LastName = "Administrator",
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = null
        };

        modelBuilder.Entity<User>().HasData(defaultUser);
    }
}
using ECommerce.Domain.Entities;
using ECommerce.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<CheckoutArchive> CheckoutArchives => Set<CheckoutArchive>();

    // AJOUT Identity
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PendingCheckout> PendingCheckouts => Set<PendingCheckout>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Product>()
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Product>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
            .HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId);

        modelBuilder.Entity<CheckoutArchive>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

        modelBuilder.Entity<CheckoutArchive>()
            .Property(x => x.AmountPaid)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PaymentMethod>().HasData(
            new PaymentMethod
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Credit Card"
            }
        );

        modelBuilder.Entity<RefreshToken>()
            .Property(x => x.UserId)
            .IsRequired();

        // Config RefreshToken
        modelBuilder.Entity<RefreshToken>()
            .Property(x => x.Token)
            .IsRequired();

        modelBuilder.Entity<CheckoutArchive>()
            .HasIndex(x => new { x.StripeSessionId, x.ProductId })
            .IsUnique()
            .HasFilter("[StripeSessionId] IS NOT NULL");

        modelBuilder.Entity<PendingCheckout>()
            .Property(x => x.CartJson)
            .IsRequired();

        modelBuilder.Entity<PendingCheckout>()
            .HasIndex(x => x.StripeSessionId)
            .IsUnique()
            .HasFilter("[StripeSessionId] IS NOT NULL");
    }
}

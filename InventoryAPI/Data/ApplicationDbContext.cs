using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InventoryAPI.Models;

namespace InventoryAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);  // This is CRITICAL - it creates Identity tables

            // Add indexes for Product table
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasDatabaseName("IX_Products_CategoryId");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Products_Name");

                entity.HasIndex(e => e.Price)
                    .HasDatabaseName("IX_Products_Price");

                entity.HasIndex(e => e.Quantity)
                    .HasDatabaseName("IX_Products_Quantity");

                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_Products_CreatedAt");
            });


            // Add indexes for faster queries
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_Users_Email");
                entity.HasIndex(e => e.NormalizedEmail).HasDatabaseName("IX_Users_NormalizedEmail");
                entity.HasIndex(e => e.UserName).HasDatabaseName("IX_Users_UserName");
            });

            // Rename Identity tables
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");


            // Seed Roles
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories" },
                new Category { Id = 2, Name = "Clothing", Description = "Apparel and fashion items" },
                new Category { Id = 3, Name = "Books", Description = "Books and publications" }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, Quantity = 10, CategoryId = 1 },
                new Product { Id = 2, Name = "T-Shirt", Description = "Cotton t-shirt", Price = 19.99m, Quantity = 50, CategoryId = 2 },
                new Product { Id = 3, Name = "Programming Book", Description = "Learn C#", Price = 49.99m, Quantity = 30, CategoryId = 3 }
            );
        }
    }
}
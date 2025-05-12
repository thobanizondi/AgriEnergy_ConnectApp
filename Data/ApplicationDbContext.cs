using Microsoft.EntityFrameworkCore;
using AgriEnergy_ConnectApp.Models;

namespace AgriEnergy_ConnectApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Inside ApplicationDbContext class
      //  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       // {
           // optionsBuilder.UseLazyLoadingProxies(); // Ensure the required package is installed
       // }

        // Define DbSet properties for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                    .HasOne<Farmer>() // No navigation property in Product
                    .WithMany(f => f.Products) // The Farmer entity still has a collection of Products
                    .HasForeignKey(p => p.FarmerId) // Foreign key in Product
                    .OnDelete(DeleteBehavior.Cascade) // Cascade delete when a Farmer is deleted
                    .IsRequired(); // Marks FarmerId as required


            // Configure the Price property in Product for SQL precision
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired(); // Ensures Price is always provided

            // Set default value for the CreatedAt property in Farmer
            modelBuilder.Entity<Farmer>()
                .Property(f => f.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()") // Automatically set UTC timestamp
                .IsRequired(); // Ensures CreatedAt cannot be null

            // Configure other relationships or constraints as needed
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email) // Example: Ensures unique Email for Users
                .IsUnique(); // Adds a unique constraint on the Email column
        }
    }
}

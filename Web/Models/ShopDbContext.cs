using System.Linq;
using Bogus;
using Bogus.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LightDbContext.Models
{
    public class ShopDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite("Data Source=example.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var ids = 1;
            var stock = new Faker<Item>()
                .RuleFor(m => m.Id, f => ids++)
                .RuleFor(m => m.Name, f => f.Commerce.ProductName())
                .RuleFor(m => m.Category, f => f.Commerce.Categories(1).First())
                .RuleFor(m => m.Price, f => f.Commerce.Price(1).First());

            // generate 1000 items
            modelBuilder
                .Entity<Item>()
                .HasData(stock.GenerateBetween(1000, 1000));
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
    
    public sealed class LightShopDbContext : ShopDbContext
    {
        public LightShopDbContext()
        {
            // light sessions only
            // this will improve performance with no tracking
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // leave the heavy lifting to the base class
        }
    }

}
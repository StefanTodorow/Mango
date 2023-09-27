using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 1,
                Name = "Breakfast Sandwich",
                Price = 3.99,
                Description = "Sandwich with eggs, ham and light sauce.",
                ImageUrl = "https://picography.co/wp-content/uploads/2023/07/picography-food-sandwich-breakfast-cook-768x673.jpg",
                CategoryName = "Breakfast"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 2,
                Name = "Tacos",
                Price = 11.99,
                Description = "Tacos with cheese, corn, chicken, lettuce, green pepper.",
                ImageUrl = "https://picography.co/wp-content/uploads/2022/06/picography-plated-tacos-768x432.jpg",
                CategoryName = "Appetizer"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 3,
                Name = "Scallops",
                Price = 14.99,
                Description = "Scallops made in a pan.",
                ImageUrl = "https://picography.co/wp-content/uploads/2021/01/picography-seared-scallops-in-pan-768x513.jpg",
                CategoryName = "Appetizer"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 4,
                Name = "Waffles",
                Price = 8.99,
                Description = "Waffles with different kind of berries and strawberry topping.",
                ImageUrl = "https://picography.co/wp-content/uploads/2019/07/picography-breakfast-flatlay-with-fruit-and-waffles-768x1015.jpg",
                CategoryName = "Dessert"
            });
        }

    }
}

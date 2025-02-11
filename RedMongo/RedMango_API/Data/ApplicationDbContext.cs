using Microsoft.EntityFrameworkCore;
using RedMango_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using static System.Net.Mime.MediaTypeNames;

namespace RedMango_API.Data
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderDetails>().HasOne(x => x.MenuItem).
                WithMany().HasForeignKey(x => x.MenuItemId).
                OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuItem>().HasData(
               new MenuItem
               {
                   Id = 1,
                   Name = "Spring Roll",
                   Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                   Image = "https://res.cloudinary.com/dhqg2iovy/image/upload/v1734881235/spring_roll_zvidgp.jpg",
                   Price = 7.99,
                   Category = "Appetizer",
                   SpecialTag = ""
               }, new MenuItem
               {
                   Id = 2,
                   Name = "Idli",
                   Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                   Image = "https://res.cloudinary.com/dhqg2iovy/image/upload/v1734881235/sweet_rolls_rvgq2r.jpg",
                   Price = 8.99,
                   Category = "Appetizer",
                   SpecialTag = ""
               }, new MenuItem
               {
                   Id = 3,
                   Name = "Panu Puri",
                   Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                   Image = "https://res.cloudinary.com/dhqg2iovy/image/upload/v1734881235/sweet_rolls_rvgq2r.jpg",
                   Price = 8.99,
                   Category = "Appetizer",
                   SpecialTag = "Best Seller"
               }, new MenuItem
               {
                   Id = 4,
                   Name = "Hakka Noodles",
                   Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                   Image = "https://res.cloudinary.com/dhqg2iovy/image/upload/v1734881235/sweet_rolls_rvgq2r.jpg",
                   Price = 10.99,
                   Category = "Entrée",
                   SpecialTag = ""
               }, new MenuItem
               {
                   Id = 5,
                   Name = "Malai Kofta",
                   Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                   Image = "https://res.cloudinary.com/dhqg2iovy/image/upload/v1734881235/sweet_rolls_rvgq2r.jpg",
                   Price = 12.99,
                   Category = "Entrée",
                   SpecialTag = "Top Rated"
               }, new MenuItem
               {
                   Id = 6,
                   Name = "Paneer Pizza",
                   Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                   Image = "https://res.cloudinary.com/dhqg2iovy/image/upload/v1734881235/sweet_rolls_rvgq2r.jpg",
                   Price = 11.99,
                   Category = "Entrée",
                   SpecialTag = ""
               }, new MenuItem
               {
                   Id = 7,
                   Name = "Paneer Tikka",
                   Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                   Image = "https://res.cloudinary.com/dhqg2iovy/image/upload/v1734881235/sweet_rolls_rvgq2r.jpg",
                   Price = 13.99,
                   Category = "Entrée",
                   SpecialTag = "Chef's Special"
               }, new MenuItem
               {
                   Id = 8,
                   Name = "Carrot Love",
                   Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                   Image = "https://res.cloudinary.com/dhqg2iovy/image/upload/v1734881235/sweet_rolls_rvgq2r.jpg",
                   Price = 4.99,
                   Category = "Dessert",
                   SpecialTag = ""
               }, new MenuItem
               {
                   Id = 9,
                   Name = "Rasmalai",
                   Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                   Image = "https://res.cloudinary.com/dhqg2iovy/image/upload/v1734881235/sweet_rolls_rvgq2r.jpg",
                   Price = 4.99,
                   Category = "Dessert",
                   SpecialTag = "Chef's Special"
               }, new MenuItem
               {
                   Id = 10,
                   Name = "Sweet Rolls",
                   Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                   Image = "https://res.cloudinary.com/dhqg2iovy/image/upload/v1734881235/sweet_rolls_rvgq2r.jpg",
                   Price = 3.99,
                   Category = "Dessert",
                   SpecialTag = "Top Rated"
               }
        );
    }
    }
}

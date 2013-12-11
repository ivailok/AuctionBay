using System;
using System.Data.Entity;
using System.Linq;
using AuctionBay.DataModels;

namespace AuctionBay.DataLayer
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProfileImage> ProfileImages { get; set; }

        public DbSet<Offer> Offers { get; set; }

        public DataContext()
            : base("AuctionBay")
        {
        }
    }
}

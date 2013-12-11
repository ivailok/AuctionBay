using System;
using System.Linq;
using AuctionBay.DataModels;
using System.Data.Entity;

namespace AuctionBayWebApi.Repositories
{
    public class DbProductsRepository : DbRepository<Product>
    {
        public DbProductsRepository(DbContext context)
            : base(context)
        {
        }

        public Product GetById(int productId)
        {
            return this.Get(productId);
        }
    }
}
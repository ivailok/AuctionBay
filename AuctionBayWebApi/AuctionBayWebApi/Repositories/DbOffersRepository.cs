using System.Data.Entity;
using AuctionBay.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuctionBayWebApi.Repositories
{
    public class DbOffersRepository : DbRepository<Offer>
    {
        public DbOffersRepository(DbContext context)
            : base(context)
        {
        }
    }
}
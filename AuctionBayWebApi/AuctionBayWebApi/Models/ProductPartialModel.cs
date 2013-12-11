using System;
using System.Linq;

namespace AuctionBayWebApi.Models
{
    public class ProductPartialModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Auctioneer { get; set; }

        public TimeSpan BiddingTimeLeft { get; set; }

        public decimal CurrentPrice { get; set; }

        public string ImageLocation { get; set; }
    }
}
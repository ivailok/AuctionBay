using System;
using System.Linq;

namespace AuctionBay.Model
{
    public class ProductPartialModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Auctioneer { get; set; }

        public string ImageLocation { get; set; }

        public TimeSpan BiddingTimeLeft { get; set; }

        public decimal CurrentPrice { get; set; }
    }
}

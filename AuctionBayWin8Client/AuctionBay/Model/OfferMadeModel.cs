using System;
using System.Linq;

namespace AuctionBay.Model
{
    public class OfferMadeModel
    {
        public int Id { get; set; }

        public string Auctioneer { get; set; }

        public string Product { get; set; }

        public decimal Price { get; set; }

        public string AuctioneerPhone { get; set; }

        public string AuctioneerEmail { get; set; }
    }
}

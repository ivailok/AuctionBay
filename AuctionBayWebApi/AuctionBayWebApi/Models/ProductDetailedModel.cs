using System;
using System.Collections.Generic;
using System.Linq;

namespace AuctionBayWebApi.Models
{
    public class ProductDetailedModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal CurrentPrice { get; set; }

        public string CurrentBidder { get; set; }

        public string BidingType { get; set; }

        public string Auctioneer { get; set; }

        public int AuctioneerId { get; set; }

        public IEnumerable<string> Images { get; set; }

        public TimeSpan BiddingTimeLeft { get; set; }
    }
}
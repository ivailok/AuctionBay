using System;
using System.Collections.Generic;
using System.Linq;

namespace AuctionBayWebApi.Models
{
    public class MyProductDetailedModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal CurrentPrice { get; set; }

        public string CurrentBidder { get; set; }

        public string BidingType { get; set; }

        public bool IsTimeOver { get; set; }

        public bool IsSold { get; set; }

        public bool IsActive { get; set; }

        public bool AreNegotiationsActive { get; set; }

        public OfferModel Offer { get; set; }

        public IEnumerable<string> Images { get; set; }

        public TimeSpan BiddingTimeLeft { get; set; }
    }
}
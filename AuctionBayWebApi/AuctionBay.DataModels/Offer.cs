using System;
using System.Linq;

namespace AuctionBay.DataModels
{
    public class Offer
    {
        public int OfferId { get; set; }

        public virtual User Buyer { get; set; }

        public virtual Product Product { get; set; }

        public bool IsBuyerFullfilled { get; set; }

        public bool IsAuctioneerFullfilled { get; set; }

        public OfferStatus Status { get; set; }
    }
}

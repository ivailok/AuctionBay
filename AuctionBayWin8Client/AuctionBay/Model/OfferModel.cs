using System;
using System.Linq;

namespace AuctionBay.Model
{
    public class OfferModel
    {
        public int Id { get; set; }

        public string Buyer { get; set; }

        public string Product { get; set; }

        public decimal Price { get; set; }

        public string BuyerPhone { get; set; }

        public string BuyerEmail { get; set; }
    }
}

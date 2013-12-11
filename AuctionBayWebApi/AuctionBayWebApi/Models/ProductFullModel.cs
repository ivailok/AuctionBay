using System;
using System.Linq;

namespace AuctionBayWebApi.Models
{
    public class ProductFullModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal StartingPrice { get; set; }

        public string BidingType { get; set; }  
    }
}
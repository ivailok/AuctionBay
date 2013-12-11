using AuctionBay.DataModels;
using System;
using System.Linq;

namespace AuctionBayWebApi.Models
{
    public class OfferMadeModel
    {
        public int Id { get; set; }

        public string AuctioneerName { get; set; }

        public string ProductName { get; set; }

        public int AuctioneerId { get; set; }

        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public bool IsBuyerFulfilled { get; set; }

        public bool IsAuctioneerFulfilled { get; set; }

        public OfferStatus Status { get; set; }
    }
}
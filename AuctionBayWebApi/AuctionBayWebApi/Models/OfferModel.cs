using System;
using System.Linq;
using AuctionBay.DataModels;

namespace AuctionBayWebApi.Models
{
    public class OfferModel
    {
        public int Id { get; set; }

        public string BuyerName { get; set; }

        public string ProductName { get; set; }

        public int BuyerId { get; set; }

        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public bool IsBuyerFullfilled { get; set; }

        public bool IsAuctioneerFullfilled { get; set; }

        public OfferStatus Status { get; set; }
    }
}
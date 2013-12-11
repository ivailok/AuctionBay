using System;
using System.Linq;
using AuctionBayWebApi.Models;
using AuctionBay.DataModels;

namespace AuctionBayWebApi.Mappers
{
    public class OffersMapper
    {
        public static OfferMadeModel ToOfferMadeModel(Offer offerEntity)
        {
            return new OfferMadeModel()
            {
                Id = offerEntity.OfferId,
                AuctioneerName = offerEntity.Product.Auctioneer.Nickname,
                Price = offerEntity.Product.CurrentPrice,
                ProductName = offerEntity.Product.Title,
                AuctioneerId = offerEntity.Product.Auctioneer.Id,
                ProductId = offerEntity.Product.ProductId,
                IsAuctioneerFulfilled = offerEntity.IsAuctioneerFullfilled,
                IsBuyerFulfilled = offerEntity.IsBuyerFullfilled,
                Status = offerEntity.Status
            };
        }

        public static OfferModel ToOfferModel(Offer offerEntity)
        {
            return new OfferModel()
            {
                Id = offerEntity.OfferId,
                BuyerName = offerEntity.Buyer.Nickname,
                Price = offerEntity.Product.CurrentPrice,
                ProductName = offerEntity.Product.Title,
                BuyerId = offerEntity.Buyer.Id,
                ProductId = offerEntity.Product.ProductId,
                IsAuctioneerFullfilled = offerEntity.IsAuctioneerFullfilled,
                IsBuyerFullfilled = offerEntity.IsBuyerFullfilled,
                Status = offerEntity.Status
            };
        }
    }
}
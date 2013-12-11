using AuctionBay.DataModels;
using AuctionBayWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuctionBayWebApi.Mappers
{
    public class ProductsMapper
    {
        public static ProductPartialModel ToProductPartialModel(Product entity)
        {
            string firstPicLoc = null;
            var firstImage = entity.ProductImages.FirstOrDefault();
            if (firstImage != null)
            {
                firstPicLoc = firstImage.ImagePublicLocation;
            }

            return new ProductPartialModel()
            {
                Id = entity.ProductId,
                Title = entity.Title,
                Auctioneer = entity.Auctioneer.Nickname,
                ImageLocation = firstPicLoc,
                BiddingTimeLeft = new TimeSpan(48, 0, 0).Subtract((DateTime.Now - entity.TimeCreated)),
                CurrentPrice = entity.CurrentPrice
            };
        }

        public static MyProductPartialModel ToMyProductPartialModel(Product entity)
        {
            string firstPicLoc = null;
            var firstImage = entity.ProductImages.FirstOrDefault();
            if (firstImage != null)
            {
                firstPicLoc = firstImage.ImagePublicLocation;
            }

            MyProductPartialModel model =  new MyProductPartialModel()
            {
                Id = entity.ProductId,
                Title = entity.Title,
                Auctioneer = entity.Auctioneer.Nickname,
                ImageLocation = firstPicLoc,
                BiddingTimeLeft = new TimeSpan(48, 0, 0).Subtract((DateTime.Now - entity.TimeCreated)),
                CurrentPrice = entity.CurrentPrice,
            };

            if (model.BiddingTimeLeft.Ticks < 0)
            {
                model.IsTimeOver = true;
            }

            switch (entity.Status)
            {
                case StatusList.Active:
                    model.IsActive = true;
                    break;
                case StatusList.Negotiating:
                    model.AreNegotiationsActive = true;
                    break;
                case StatusList.Sold:
                    model.IsSold = true;
                    break;
            }

            return model;
        }

        public static Product ToProductEntity(ProductFullModel product, User user)
        {
            Product productEntity = new Product()
            {
                Title = product.Title,
                Description = product.Description,
                Auctioneer = user,
                CurrentPrice = product.StartingPrice,
                TimeCreated = DateTime.Now,
                BidingType = product.BidingType,
                Status = StatusList.Active
            };

            return productEntity;
        }

        public static ProductDetailedModel ToProductDetailedModel(Product productEntity)
        {
            ProductDetailedModel product = new ProductDetailedModel()
            {
                Title = productEntity.Title,
                Auctioneer = productEntity.Auctioneer.Nickname,
                AuctioneerId = productEntity.Auctioneer.Id,
                BiddingTimeLeft = new TimeSpan(48, 0, 0).Subtract((DateTime.Now - productEntity.TimeCreated)),
                CurrentPrice = productEntity.CurrentPrice,
                CurrentBidder = productEntity.CurrentBidder,
                BidingType = productEntity.BidingType,
                Description = productEntity.Description,
            };

            ICollection<string> images = new List<string>();
            foreach (var image in productEntity.ProductImages)
            {
                images.Add(image.ImagePublicLocation);
            }
            product.Images = images;

            return product;
        }

        public static MyProductDetailedModel ToMyProductDetailedModel(Product productEntity, Offer offerEntity)
        {
            MyProductDetailedModel product = new MyProductDetailedModel()
            {
                Title = productEntity.Title,
                BiddingTimeLeft = new TimeSpan(48, 0, 0).Subtract((DateTime.Now - productEntity.TimeCreated)),
                CurrentPrice = productEntity.CurrentPrice,
                CurrentBidder = productEntity.CurrentBidder,
                BidingType = productEntity.BidingType,
                Description = productEntity.Description,
                Offer = offerEntity == null ? null : OffersMapper.ToOfferModel(offerEntity)
            };

            if (product.BiddingTimeLeft.Ticks < 0)
            {
                product.IsTimeOver = true;
            }

            switch (productEntity.Status)
            {
                case StatusList.Active:
                    product.IsActive = true;
                    break;
                case StatusList.Negotiating:
                    product.AreNegotiationsActive = true;
                    break;
                case StatusList.Sold:
                    product.IsSold = true;
                    break;
            }

            ICollection<string> images = new List<string>();
            foreach (var image in productEntity.ProductImages)
            {
                images.Add(image.ImagePublicLocation);
            }
            product.Images = images;

            return product;
        }
    }
}
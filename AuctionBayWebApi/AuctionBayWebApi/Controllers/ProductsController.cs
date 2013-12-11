using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using AuctionBay.DataModels;
using AuctionBayWebApi.Attributes;
using AuctionBayWebApi.Mappers;
using AuctionBayWebApi.Models;
using AuctionBayWebApi.Repositories;
using System.Web.Http.ValueProviders;
using System.Web;

namespace AuctionBayWebApi.Controllers
{
    public class ProductsController : BaseController
    {
        private DbUsersRepository usersRepository;
        private DbProductsRepository productsRepository;
        private DbOffersRepository offersRepository;
        private DropboxUploader uploader;

        public ProductsController(DbUsersRepository usersRepository, DbProductsRepository productsRepository, DropboxUploader uploader, DbOffersRepository offersRepository)
        {
            this.usersRepository = usersRepository;
            this.productsRepository = productsRepository;
            this.offersRepository = offersRepository;
            this.uploader = uploader;
        }

        [HttpGet, ActionName("all")]
        public HttpResponseMessage GetProducts(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]string sessionKey)
        {
            HttpResponseMessage responseMsg = this.PerformOperationAndHandleExceptions(
               () =>
               {
                   User user = null;
                   try
                   {
                       user = this.usersRepository.GetBySessionKey(sessionKey);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session key provided!");
                   }

                   //&& product.Auctioneer.Nickname != user.Nickname

                   IEnumerable<Product> productEntities = this.productsRepository.GetAll().OrderBy(x => x.TimeCreated).AsEnumerable();
                   foreach (var entity in productEntities)
                   {
                       var firstPic = entity.ProductImages.FirstOrDefault();
                       if (firstPic != null)
                       {
                           firstPic.ImagePublicLocation = this.uploader.RenewImageLocation(firstPic.ImageFolder, firstPic.ImageName);
                       }
                   }

                   bool userLocationAll = false;
                   if (user.Location == "All")
                   {
                       userLocationAll = true;
                   }

                   IEnumerable<ProductPartialModel> products =
                       from product in productEntities
                       where (DateTime.Now - product.TimeCreated).TotalHours < 48 && product.Status == StatusList.Active && product.Auctioneer.Nickname != user.Nickname && (userLocationAll || product.Auctioneer.Location == user.Location)
                       select ProductsMapper.ToProductPartialModel(product);

                   HttpResponseMessage response = this.Request.CreateResponse<IEnumerable<ProductPartialModel>>(HttpStatusCode.OK, products);
                   return response;
               });

            return responseMsg;
        }

        [HttpGet, ActionName("my")]
        public HttpResponseMessage GetMyProducts(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]string sessionKey)
        {
            HttpResponseMessage responseMsg = this.PerformOperationAndHandleExceptions(
               () =>
               {
                   User user = null;
                   try
                   {
                       user = this.usersRepository.GetBySessionKey(sessionKey);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session key provided!");
                   }

                   IEnumerable<Product> productEntities = this.productsRepository.GetAll().OrderBy(x => x.TimeCreated).AsEnumerable();
                   foreach (var entity in productEntities)
                   {
                       var firstPic = entity.ProductImages.FirstOrDefault();
                       if (firstPic != null)
                       {
                           firstPic.ImagePublicLocation = this.uploader.RenewImageLocation(firstPic.ImageFolder, firstPic.ImageName);
                       }
                   }

                   IEnumerable<MyProductPartialModel> products =
                       from product in productEntities
                       where product.Auctioneer.Nickname == user.Nickname
                       select ProductsMapper.ToMyProductPartialModel(product);

                   HttpResponseMessage response = this.Request.CreateResponse<IEnumerable<MyProductPartialModel>>(HttpStatusCode.OK, products);
                   return response;
               });

            return responseMsg;
        }

        [HttpPost, ActionName("add")]
        public HttpResponseMessage AddProduct(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]string sessionKey,
            [FromBody]ProductFullModel product)
        {
            HttpResponseMessage responseMsg = this.PerformOperationAndHandleExceptions(
               () =>
               {
                   User user = null;
                   try
                   {
                       user = this.usersRepository.GetBySessionKey(sessionKey);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session key provided!");
                   }

                   Product productEntity = ProductsMapper.ToProductEntity(product, user);
                   this.productsRepository.Add(productEntity);

                   HttpResponseMessage response = this.Request.CreateResponse<int>(HttpStatusCode.Created, productEntity.ProductId);
                   return response;
               });

            return responseMsg;
        }

        [HttpPost, ActionName("uploadimages")]
        public HttpResponseMessage UploadImages(int productId, string sessionKey)
        {
            HttpResponseMessage responseMsg = this.PerformOperationAndHandleExceptions(
               () =>
               {
                   User user = null;
                   try
                   {
                       user = this.usersRepository.GetBySessionKey(sessionKey);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session key provided!");
                   }

                   Product product = null;
                   try
                   {
                       product = this.productsRepository.GetById(productId);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid product id provided!");
                   }

                   var files = HttpContext.Current.Request.Files;

                   string currentDir = Path.GetTempPath();
                   for (int i = 0; i < files.Count; i++)
                   {
                       var filePath = currentDir + files[i].FileName;
                       files[i].SaveAs(filePath);

                       ProductImage image = this.uploader.UploadProductImage(filePath);

                       product.ProductImages.Add(image);
                       this.productsRepository.Update(productId, product);

                       //File.Delete(filePath);
                   }

                   HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.Created);
                   return response;
               });

            return responseMsg;
        }

        [HttpGet, ActionName("view")]
        public HttpResponseMessage ViewProduct(int productId,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]string sessionKey)
        {
            HttpResponseMessage responseMsg = this.PerformOperationAndHandleExceptions(
               () =>
               {
                   User user = null;
                   try
                   {
                       user = this.usersRepository.GetBySessionKey(sessionKey);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session key provided!");
                   }

                   Product product = null;
                   try
                   {
                       product = this.productsRepository.GetById(productId);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid product id provided!");
                   }

                   foreach (var image in product.ProductImages)
                   {
                       image.ImagePublicLocation = this.uploader.RenewImageLocation(image.ImageFolder, image.ImageName);
                   }
                   this.productsRepository.Update(productId, product);
                   ProductDetailedModel detailedProduct = ProductsMapper.ToProductDetailedModel(product);

                   HttpResponseMessage response = this.Request.CreateResponse<ProductDetailedModel>(HttpStatusCode.OK, detailedProduct);
                   return response;
               });

            return responseMsg;
        }

        [HttpGet, ActionName("viewmy")]
        public HttpResponseMessage ViewMyProduct(int productId,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]string sessionKey)
        {
            HttpResponseMessage responseMsg = this.PerformOperationAndHandleExceptions(
               () =>
               {
                   User user = null;
                   try
                   {
                       user = this.usersRepository.GetBySessionKey(sessionKey);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session key provided!");
                   }

                   Product product = null;
                   try
                   {
                       product = this.productsRepository.GetById(productId);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid product id provided!");
                   }

                   foreach (var image in product.ProductImages)
                   {
                       image.ImagePublicLocation = this.uploader.RenewImageLocation(image.ImageFolder, image.ImageName);
                   }
                   this.productsRepository.Update(productId, product);

                   Offer offer = this.offersRepository.GetAll().Where(x => x.Product.ProductId == productId && (x.Status != OfferStatus.Closed && x.Status != OfferStatus.Cancelled)).FirstOrDefault();

                   MyProductDetailedModel detailedProduct = ProductsMapper.ToMyProductDetailedModel(product, offer);

                   HttpResponseMessage response = this.Request.CreateResponse<MyProductDetailedModel>(HttpStatusCode.OK, detailedProduct);
                   return response;
               });

            return responseMsg;
        }

        [HttpGet, ActionName("renew")]
        public HttpResponseMessage RenewProduct(int productId,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]string sessionKey)
        {
            HttpResponseMessage responseMsg = this.PerformOperationAndHandleExceptions(
               () =>
               {
                   User user = null;
                   try
                   {
                       user = this.usersRepository.GetBySessionKey(sessionKey);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session key provided!");
                   }

                   Product product = null;
                   try
                   {
                       product = this.productsRepository.GetById(productId);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid product id provided!");
                   }

                   foreach (var image in product.ProductImages)
                   {
                       image.ImagePublicLocation = this.uploader.RenewImageLocation(image.ImageFolder, image.ImageName);
                   }
                   product.TimeCreated = DateTime.Now;
                   this.productsRepository.Update(productId, product);

                   HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, new { TimeLeft = new TimeSpan(48, 0, 0).Subtract((DateTime.Now - product.TimeCreated)) });
                   return response;
               });

            return responseMsg;
        }

        [HttpPut, ActionName("bid")]
        public HttpResponseMessage BidForProduct(int productId, string value,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]string sessionKey)
        {
            HttpResponseMessage responseMsg = this.PerformOperationAndHandleExceptions(
               () =>
               {
                   User user = null;
                   try
                   {
                       user = this.usersRepository.GetBySessionKey(sessionKey);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session key provided!");
                   }

                   Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(1026);

                   decimal bidValue;
                   if (!decimal.TryParse(value, out bidValue))
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid bid value provided!");
                   }

                   Product product = null;
                   try
                   {
                       product = this.productsRepository.GetById(productId);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid product id provided!");
                   }

                   lock (product)
                   {
                       product.CurrentPrice += bidValue;
                       product.CurrentBidder = user.Nickname;
                       this.productsRepository.Update(product.ProductId, product);
                   }
                   HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
                   return response;
               });

            return responseMsg;
        }

        [HttpPut, ActionName("buyout")]
        public HttpResponseMessage BuyProduct(int productId,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]string sessionKey)
        {
            HttpResponseMessage responseMsg = this.PerformOperationAndHandleExceptions(
               () =>
               {
                   User user = null;
                   try
                   {
                       user = this.usersRepository.GetBySessionKey(sessionKey);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session key provided!");
                   }

                   Product product = null;
                   try
                   {
                       product = this.productsRepository.GetById(productId);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid product id provided!");
                   }

                   lock (product)
                   {
                       this.offersRepository.Add(new Offer()
                       {
                           Buyer = user,
                           Product = product,
                           Status = OfferStatus.Initialized,
                           IsAuctioneerFullfilled = false,
                           IsBuyerFullfilled = false
                       });

                       product.Status = StatusList.Negotiating;
                       this.productsRepository.Update(product.ProductId, product);
                   }
                   HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
                   return response;
               });

            return responseMsg;
        }
    }
}

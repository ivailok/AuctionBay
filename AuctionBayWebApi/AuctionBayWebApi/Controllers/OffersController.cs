using AuctionBay.DataModels;
using AuctionBayWebApi.Attributes;
using AuctionBayWebApi.Mappers;
using AuctionBayWebApi.Models;
using AuctionBayWebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ValueProviders;

namespace AuctionBayWebApi.Controllers
{
    public class OffersController : BaseController
    {
        private DbOffersRepository offersRepository;
        private DbUsersRepository usersRepository;
        private DbProductsRepository productsRepository;

        public OffersController(DbOffersRepository offersRepository, DbUsersRepository usersRepository, DbProductsRepository productsRepository)
        {
            this.usersRepository = usersRepository;
            this.productsRepository = productsRepository;
            this.offersRepository = offersRepository;
        }

        [HttpGet, ActionName("made")]
        public HttpResponseMessage GetMadeOffers(
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

                   IEnumerable<Offer> offerEntities = this.offersRepository.GetAll().Where(x => x.Buyer.Nickname == user.Nickname).AsEnumerable();

                   IEnumerable<OfferMadeModel> offers =
                       from offer in offerEntities
                       select OffersMapper.ToOfferMadeModel(offer);

                   HttpResponseMessage response = this.Request.CreateResponse<IEnumerable<OfferMadeModel>>(HttpStatusCode.OK, offers);
                   return response;
               });

            return responseMsg;
        }

        [HttpPut, ActionName("cancel")]
        public HttpResponseMessage CancelDeal(int offerId,
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

                   Offer offer = null;
                   try
                   {
                       offer = this.offersRepository.GetAll().Where(x => x.OfferId == offerId).FirstOrDefault();
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid offer provided!");
                   }

                   offer.Product.Status = StatusList.Active;
                   offer.Product.TimeCreated = DateTime.Now;

                   if (offer.Buyer.Nickname == user.Nickname)
                   {
                       offer.Status = OfferStatus.Cancelled;
                   }
                   else
                   {
                       offer.Status = OfferStatus.Closed;
                   }

                   this.offersRepository.Update(offer.OfferId, offer);

                   HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
                   return response;
               });

            return responseMsg; 
        }
        
        [HttpPut, ActionName("finalize")]
        public HttpResponseMessage FinalizeDeal(int offerId,
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

                   Offer offer = null;
                   try
                   {
                       offer = this.offersRepository.GetAll().Where(x => x.OfferId == offerId).FirstOrDefault();
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid offer provided!");
                   }

                   if (offer.Buyer.Nickname == user.Nickname)
                   {
                       offer.IsBuyerFullfilled = true;
                   }
                   else
                   {
                       offer.IsAuctioneerFullfilled = true;
                       offer.Product.Status = StatusList.Sold;
                   }

                   if (offer.IsAuctioneerFullfilled && offer.IsBuyerFullfilled)
                   {
                       offer.Product.Auctioneer.Reputation += 25;
                       offer.Buyer.Reputation += 10;
                       offer.Status = OfferStatus.Successful;
                   }

                   this.offersRepository.Update(offerId, offer);

                   HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
                   return response;
               });

            return responseMsg;
        }
    }
}

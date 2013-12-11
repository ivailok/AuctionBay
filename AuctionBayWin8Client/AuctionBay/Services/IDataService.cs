using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionBay.Model;
using Windows.Storage;
using Windows.Networking.BackgroundTransfer;

namespace AuctionBay.Services
{
    public interface IDataService
    {
        Task<LoggedModel> Login(LoginModel loginModel);

        Task<LoggedModel> Register(RegisterModel registerModel);

        Task Logout(string sessionKey);


        Task UploadProfileImage(IStorageFile file, Progress<UploadOperation> progress, string sessionKey);

        Task UploadImages(IEnumerable<IStorageFile> files, Progress<UploadOperation> progress, string sessionKey, int productId);


        Task<ProfileModel> GetProfile(string sessionKey);

        Task UpdateProfile(string sessionKey, ProfileModel profile);


        Task<IEnumerable<ProductPartialModel>> GetProducts(string sessionKey);

        Task<IEnumerable<ProductPartialModel>> GetMyProducts(string sessionKey);

        Task<int> AddProduct(ProductFullModel product, string sessionKey);

        Task<ProductDetailedModel> ViewProduct(string sessionKey, int productId);

        Task Bid(string sessionKey, int productId, string value);

        Task Buyout(string sessionKey, int productId);


        Task<IEnumerable<OfferMadeModel>> GetMyOffers(string sessionKey);

        Task<IEnumerable<OfferModel>> GetReceivedOffers(string sessionKey);
    }
}

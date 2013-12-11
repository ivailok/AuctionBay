using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AuctionBay.Model;
using Newtonsoft.Json;
using System.Net;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using System.Threading;

namespace AuctionBay.Services
{
    public class ExternalDataService : IDataService
    {
        private const string LocalBaseUrl = "http://localhost:63716/api/";
        private const string RemoteBaseUrl = "http://auctionbay.apphb.com/api/";

        public string CurrentBaseUrl { get; private set; }

        public ExternalDataService()
        {
            this.CurrentBaseUrl = RemoteBaseUrl;
        }

        // user operations
        public Task Logout(string sessionKey)
        {
            IDictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(new KeyValuePair<string,string>("X-sessionKey", sessionKey));
            return HttpRequester.Put<LoginModel>(this.CurrentBaseUrl + "users/logout", null, headers);
        }

        public async Task<LoggedModel> Login(LoginModel loginModel)
        {
            try
            {
                HttpResponseMessage response = await HttpRequester.Post<LoginModel>(this.CurrentBaseUrl + "users/login", loginModel);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    string contentAsString = await response.Content.ReadAsStringAsync();
                    LoggedModel loggedData = await JsonConvert.DeserializeObjectAsync<LoggedModel>(contentAsString);
                    return loggedData;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        public async Task<LoggedModel> Register(RegisterModel registerModel)
        {
            try
            {
                HttpResponseMessage response = await HttpRequester.Post<RegisterModel>(this.CurrentBaseUrl + "users/register", registerModel);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    string contentAsString = await response.Content.ReadAsStringAsync();
                    LoggedModel loggedData = await JsonConvert.DeserializeObjectAsync<LoggedModel>(contentAsString);
                    return loggedData;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }


        // profile operations
        public async Task<ProfileModel> GetProfile(string sessionKey)
        {
            try
            {
                IDictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add(new KeyValuePair<string, string>("X-sessionKey", sessionKey));

                HttpResponseMessage response = await HttpRequester.Get(this.CurrentBaseUrl + "users/getprofile", headers);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string contentAsString = await response.Content.ReadAsStringAsync();
                    ProfileModel profileData = await JsonConvert.DeserializeObjectAsync<ProfileModel>(contentAsString);
                    return profileData;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        public async Task UpdateProfile(string sessionKey, ProfileModel profile)
        {
            try
            {
                IDictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add(new KeyValuePair<string, string>("X-sessionKey", sessionKey));

                HttpResponseMessage response = await HttpRequester.Put<ProfileModel>(this.CurrentBaseUrl + "users/updateprofile", profile, headers);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }


        // image uploading
        public async Task UploadProfileImage(IStorageFile file, Progress<UploadOperation> progress, string sessionKey)
        {
            IList<IStorageFile> files = new List<IStorageFile>();
            files.Add(file);
            await this.UploadProfileImages(files, progress, sessionKey);
        }

        private async Task UploadProfileImages(IEnumerable<IStorageFile> files, Progress<UploadOperation> progress, string sessionKey)
        {
            try
            {
                var fileParts = new List<BackgroundTransferContentPart>();
                foreach (var file in files)
                {
                    var part = new BackgroundTransferContentPart();
                    part.SetFile(file);
                    fileParts.Add(part);
                }

                BackgroundUploader uploader = new BackgroundUploader();
                var uploadOperation =
                    await uploader.CreateUploadAsync(
                        new Uri(this.CurrentBaseUrl + "users/uploadimage?sessionKey=" + sessionKey, UriKind.Absolute),
                        fileParts);


                await uploadOperation.StartAsync()
                                     .AsTask(CancellationToken.None, progress);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        public async Task UploadImages(IEnumerable<IStorageFile> files, Progress<UploadOperation> progress, string sessionKey, int productId)
        {
            try
            {
                var fileParts = new List<BackgroundTransferContentPart>();
                foreach (var file in files)
                {
                    var part = new BackgroundTransferContentPart();
                    part.SetFile(file);
                    fileParts.Add(part);
                }

                BackgroundUploader uploader = new BackgroundUploader();
                var uploadOperation =
                    await uploader.CreateUploadAsync(
                        new Uri(this.CurrentBaseUrl + "products/uploadimages?productId=" + productId + "&sessionKey=" + sessionKey, UriKind.Absolute),
                        fileParts);


                await uploadOperation.StartAsync()
                                     .AsTask(CancellationToken.None, progress);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }


        // product operations
        public async Task<IEnumerable<ProductPartialModel>> GetProducts(string sessionKey)
        {
            try
            {
                IDictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add(new KeyValuePair<string, string>("X-sessionKey", sessionKey));

                HttpResponseMessage response = await HttpRequester.Get(this.CurrentBaseUrl + "products/all", headers);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string contentAsString = await response.Content.ReadAsStringAsync();
                    IEnumerable<ProductPartialModel> products = await JsonConvert.DeserializeObjectAsync<IEnumerable<ProductPartialModel>>(contentAsString);
                    return products;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        public async Task<IEnumerable<ProductPartialModel>> GetMyProducts(string sessionKey)
        {
            try
            {
                IDictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add(new KeyValuePair<string, string>("X-sessionKey", sessionKey));

                HttpResponseMessage response = await HttpRequester.Get(this.CurrentBaseUrl + "products/my", headers);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string contentAsString = await response.Content.ReadAsStringAsync();
                    IEnumerable<ProductPartialModel> products = await JsonConvert.DeserializeObjectAsync<IEnumerable<ProductPartialModel>>(contentAsString);
                    return products;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        public async Task<int> AddProduct(ProductFullModel product, string sessionKey)
        {
            try
            {
                IDictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add(new KeyValuePair<string, string>("X-sessionKey", sessionKey));

                HttpResponseMessage response = await HttpRequester.Post<ProductFullModel>(this.CurrentBaseUrl + "products/add", product, headers);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    string contentAsString = await response.Content.ReadAsStringAsync();
                    int productId = await JsonConvert.DeserializeObjectAsync<int>(contentAsString);
                    return productId;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        public async Task<ProductDetailedModel> ViewProduct(string sessionKey, int productId)
        {
            try
            {
                IDictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add(new KeyValuePair<string, string>("X-sessionKey", sessionKey));

                HttpResponseMessage response = await HttpRequester.Get(this.CurrentBaseUrl + "products/view?productId=" + productId, headers);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string contentAsString = await response.Content.ReadAsStringAsync();
                    ProductDetailedModel product = await JsonConvert.DeserializeObjectAsync<ProductDetailedModel>(contentAsString);
                    return product;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        public async Task Bid(string sessionKey, int productId, string value)
        {
            try
            {
                IDictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add(new KeyValuePair<string, string>("X-sessionKey", sessionKey));

                HttpResponseMessage response = await HttpRequester.Put<ProfileModel>(this.CurrentBaseUrl + "products/bid?productId=" + productId + "&value=" + value, null, headers);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        public async Task Buyout(string sessionKey, int productId)
        {
            try
            {
                IDictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add(new KeyValuePair<string, string>("X-sessionKey", sessionKey));

                HttpResponseMessage response = await HttpRequester.Put<ProfileModel>(this.CurrentBaseUrl + "products/buyout?productId=" + productId, null, headers);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }


        // offer operations
        public async Task<IEnumerable<OfferMadeModel>> GetMyOffers(string sessionKey)
        {
            try
            {
                IDictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add(new KeyValuePair<string, string>("X-sessionKey", sessionKey));

                HttpResponseMessage response = await HttpRequester.Get(this.CurrentBaseUrl + "offers/made", headers);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string contentAsString = await response.Content.ReadAsStringAsync();
                    IEnumerable<OfferMadeModel> myOffers = await JsonConvert.DeserializeObjectAsync<IEnumerable<OfferMadeModel>>(contentAsString);
                    return myOffers;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        public async Task<IEnumerable<OfferModel>> GetReceivedOffers(string sessionKey)
        {
            try
            {
                IDictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add(new KeyValuePair<string, string>("X-sessionKey", sessionKey));

                HttpResponseMessage response = await HttpRequester.Get(this.CurrentBaseUrl + "offers/provided", headers);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string contentAsString = await response.Content.ReadAsStringAsync();
                    IEnumerable<OfferModel> myOffers = await JsonConvert.DeserializeObjectAsync<IEnumerable<OfferModel>>(contentAsString);
                    return myOffers;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    HttpResponseErrorModel error = await JsonConvert.DeserializeObjectAsync<HttpResponseErrorModel>(errorAsString);

                    string shortenedErrorMessage = error.Message;
                    int index = shortenedErrorMessage.IndexOf("\r\n");
                    if (index != -1)
                    {
                        shortenedErrorMessage = shortenedErrorMessage.Remove(index);
                    }
                    throw new InvalidOperationException(shortenedErrorMessage);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Web;
using AuctionBay.DataModels;
using Spring.Social.Dropbox.Connect;
using Spring.Social.Dropbox.Api;
using Spring.Social.OAuth1;
using Spring.IO;

namespace AuctionBayWebApi.Repositories
{
    public class DropboxUploader
    {
        private const string DropboxAppKey = "4uhikx6utlqb3ym";
        private const string DropboxAppSecret = "h3ypy0spx41e7m6";

        private const string OAuthTokenFileName = "OAuthTokenFileName.txt";

        private const string FolderLocation = "Public";
        private const string ProfileImagesFolder = "ProfileImages";
        private const string ProductImagesFolder = "ProductImages";

        private OAuthToken oauthAccessToken;
        private IDropbox dropbox;

        public DropboxUploader()
        {
            DropboxServiceProvider dropboxServiceProvider =
                new DropboxServiceProvider(DropboxAppKey, DropboxAppSecret, AccessLevel.Full);

            this.oauthAccessToken = LoadOAuthToken();

            // Login in Dropbox
            this.dropbox = dropboxServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);
        }

        public string RenewImageLocation(string directoryName, string fileName)
        {
            DropboxLink link = this.dropbox.GetMediaLinkAsync(directoryName + "/" + fileName).Result;
            return link.Url;
        }

        public ProductImage UploadProductImage(string filePath)
        {
            string directoryName = FolderLocation + "/" + ProductImagesFolder;
            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

            Entry uploadFileEntry = this.dropbox.UploadFileAsync(new FileResource(filePath), directoryName + "/" + fileName).Result;
            DropboxLink link = this.dropbox.GetMediaLinkAsync(directoryName + "/" + fileName).Result;
            return new ProductImage()
            {
                ImageFolder = directoryName,
                ImageName = fileName,
                ImagePublicLocation = link.Url
            };
        }

        public ProfileImage UploadProfileImage(string filePath)
        {
            string directoryName = FolderLocation + "/" + ProfileImagesFolder;
            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

            Entry uploadFileEntry = this.dropbox.UploadFileAsync(new FileResource(filePath), directoryName + "/" + fileName).Result;
            DropboxLink link = this.dropbox.GetMediaLinkAsync(directoryName + "/" + fileName).Result;
            return new ProfileImage()
            {
                ImageFolder = directoryName,
                ImageName = fileName,
                ImagePublicLocation = link.Url
            };
        }

        private static OAuthToken LoadOAuthToken()
        {
            var filePath = HttpContext.Current.Server.MapPath("~/AuthenticationData/" + OAuthTokenFileName);
            string[] lines = File.ReadAllLines(filePath);
            OAuthToken oauthAccessToken = new OAuthToken(lines[0], lines[1]);
            return oauthAccessToken;
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using AuctionBay.DataModels;
using AuctionBayWebApi.Attributes;
using AuctionBayWebApi.Mappers;
using AuctionBayWebApi.Models;
using AuctionBayWebApi.Repositories;
using AuctionBayWebApi.Validators;

namespace AuctionBayWebApi.Controllers
{
    public class UsersController : BaseController
    {
        private DbUsersRepository usersRepository;
        private DropboxUploader uploader;

        public UsersController(DbUsersRepository repository, DropboxUploader uploader)
        {
            this.usersRepository = repository;
            this.uploader = uploader;
        }

        [HttpPost, ActionName("register")]
        public HttpResponseMessage RegisterUser(UserRegisterModel userModel)
        {
            HttpResponseMessage responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    UserValidator.ValidateNewUserData(userModel);

                    User duplicatedUser = this.usersRepository.GetByUsername(userModel.Username);
                    if (duplicatedUser != null)
                    {
                        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Username is already taken.");
                    }
                    duplicatedUser = this.usersRepository.GetByNickname(userModel.Nickname);
                    if (duplicatedUser != null)
                    {
                        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Nickname is already taken.");
                    }

                    User newUser = null;
                    try
                    {
                        newUser = UsersMapper.ToUserEntity(userModel);
                    }
                    catch (Exception)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid user register model provided!");
                    }

                    usersRepository.Add(newUser);

                    User inDbUser = this.usersRepository.GetByUsernameAndAuthCode(newUser.Username, newUser.AuthCode);
                    inDbUser.SessionKey = UserValidator.GenerateSessionKey(inDbUser.Id);
                    this.usersRepository.Update(inDbUser.Id, inDbUser);
                    UserLoggedModel loggedUser = new UserLoggedModel()
                    {
                        Nickname = inDbUser.Nickname,
                        SessionKey = inDbUser.SessionKey
                    };

                    var response = this.Request.CreateResponse(HttpStatusCode.Created, loggedUser);
                    return response;
                }
            );

            return responseMsg;
        }

        [HttpPut, ActionName("logout")]
        public HttpResponseMessage LogoutUser(
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

                    user.SessionKey = null;
                    this.usersRepository.Update(user.Id, user);

                    HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                });

            return responseMsg;
        }

        [HttpPost, ActionName("login")]
        public HttpResponseMessage LoginUser(UserLoginModel userModel)
        {
            HttpResponseMessage responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    UserValidator.ValidateAuthCode(userModel.AuthCode);
                    UserValidator.ValidateUsername(userModel.Username);

                    User user = null;
                    try
                    {
                        user = this.usersRepository.GetByUsernameAndAuthCode(userModel.Username, userModel.AuthCode);
                    }
                    catch (Exception)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid username or password!");
                    }

                    user.SessionKey = UserValidator.GenerateSessionKey(user.Id);
                    this.usersRepository.Update(user.Id, user);

                    UserLoggedModel loggedUser = new UserLoggedModel()
                    {
                        Nickname = user.Nickname,
                        SessionKey = user.SessionKey
                    };

                    var response = this.Request.CreateResponse(HttpStatusCode.Created, loggedUser);
                    return response;
                }
            );

            return responseMsg;
        }

        [HttpGet, ActionName("getprofile")]
        public HttpResponseMessage GetProfile(
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

                   string imageLocation = null;
                   if (user.ProfileImage != null)
                   {
                       imageLocation = this.uploader.RenewImageLocation(user.ProfileImage.ImageFolder, user.ProfileImage.ImageName);
                   }

                   ProfileModel profile = UsersMapper.ToProfileModel(user, imageLocation);

                   HttpResponseMessage response = this.Request.CreateResponse<ProfileModel>(HttpStatusCode.OK, profile);
                   return response;
               });

            return responseMsg;
        }

        [HttpGet, ActionName("view")]
        public HttpResponseMessage ViewProfile(int userId,
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

                   User viewedUser = null;
                   try
                   {
                       viewedUser = this.usersRepository.Get(userId);
                   }
                   catch (Exception)
                   {
                       return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid user id provided!");
                   }

                   string imageLocation = null;
                   if (user.ProfileImage != null)
                   {
                       imageLocation = this.uploader.RenewImageLocation(viewedUser.ProfileImage.ImageFolder, viewedUser.ProfileImage.ImageName);
                   }

                   ProfileModel profile = UsersMapper.ToProfileModel(viewedUser, imageLocation);

                   HttpResponseMessage response = this.Request.CreateResponse<ProfileModel>(HttpStatusCode.OK, profile);
                   return response;
               });

            return responseMsg;
        }

        [HttpPost, ActionName("uploadimage")]
        public HttpResponseMessage UploadImage(string sessionKey)
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

                   var files = HttpContext.Current.Request.Files;

                   ProfileImage image;

                   if (files.Count != 1)
                   {
                       throw new ArgumentException("Profile image must be a single image.");
                   }
                   else
                   {
                       var profileImage = files.Get(0);
                       var filePath = Path.GetTempPath() + profileImage.FileName;
                       profileImage.SaveAs(filePath);

                       image = this.uploader.UploadProfileImage(filePath);

                       user.ProfileImage = image;
                       this.usersRepository.Update(user.Id, user);

                       //are we
                       //File.Delete(filePath);
                   }

                   HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.Created);
                   return response;
               });

            return responseMsg;
        }

        [HttpPut, ActionName("updateprofile")]
        public HttpResponseMessage UpdateProfile(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]string sessionKey,
            [FromBody]ProfileModel profile)
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

                   UserValidator.ValidateNickname(profile.Nickname);
                   UserValidator.ValidateEmail(profile.Email);

                   user.Nickname = profile.Nickname;
                   user.Location = profile.Location;
                   user.PhoneNumber = profile.PhoneNumber;
                   user.Email = profile.Email;
                   this.usersRepository.Update(user.Id, user); 

                   HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
                   return response;
               });

            return responseMsg;
        }
    }
}

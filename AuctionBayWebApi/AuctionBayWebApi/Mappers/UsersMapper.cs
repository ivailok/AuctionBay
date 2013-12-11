using System;
using System.Linq;
using AuctionBay.DataModels;
using AuctionBayWebApi.Models;

namespace AuctionBayWebApi.Mappers
{
    public class UsersMapper
    {
        public static User ToUserEntity(UserRegisterModel userModel)
        {
            User userEntity = new User()
            {
                Username = userModel.Username.ToLower(),
                AuthCode = userModel.AuthCode,
                Nickname = userModel.Nickname,
                Location = userModel.Location
            };

            return userEntity;
        }

        public static ProfileModel ToProfileModel(User entity, string imagePublicLocation)
        {
            ProfileModel userEntity = new ProfileModel()
            {
                Nickname = entity.Nickname,
                Location = entity.Location,
                ProfileImage = imagePublicLocation,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Reputation = entity.Reputation
            };

            return userEntity;
        }
    }
}
using System;
using System.Data.Entity;
using System.Linq;
using AuctionBay.DataModels;

namespace AuctionBayWebApi.Repositories
{
    public class DbUsersRepository : DbRepository<User>
    {
        public DbUsersRepository(DbContext context)
            : base(context)
        { }

        public User GetByNickname(string nickname)
        {
            return this.GetAll().FirstOrDefault<User>(u => u.Nickname == nickname);
        }

        public User GetByUsernameAndAuthCode(string username, string authCode)
        {
            return this.GetAll().First<User>(u => u.Username == username && u.AuthCode == authCode);
        }

        public User GetByUsername(string username)
        {
            return this.GetAll().FirstOrDefault<User>(u => u.Username == username);
        }

        public User GetBySessionKey(string sessionKey)
        {
            return this.GetAll().FirstOrDefault<User>(u => u.SessionKey == sessionKey);
        }
    }
}
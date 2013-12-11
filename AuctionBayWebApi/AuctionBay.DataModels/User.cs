using System;
using System.Linq;

namespace AuctionBay.DataModels
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string AuthCode { get; set; }

        public string SessionKey { get; set; }

        public string Location { get; set; }

        public string Nickname { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int Reputation { get; set; }

        public virtual ProfileImage ProfileImage { get; set; }
    }
}

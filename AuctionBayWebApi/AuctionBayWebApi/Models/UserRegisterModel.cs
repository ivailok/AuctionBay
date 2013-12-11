using System;
using System.Linq;
using System.Runtime.Serialization;

namespace AuctionBayWebApi.Models
{
    [DataContract]
    public class UserRegisterModel
    {
        [DataMember(Name = "username", IsRequired = true, Order = 1)]
        public string Username { get; set; }

        [DataMember(Name = "authCode", IsRequired = true, Order = 2)]
        public string AuthCode { get; set; }

        [DataMember(Name = "nickname", IsRequired = true, Order = 3)]
        public string Nickname { get; set; }

        [DataMember(Name = "location", IsRequired = false, Order = 4)]
        public string Location { get; set; }
    }
}
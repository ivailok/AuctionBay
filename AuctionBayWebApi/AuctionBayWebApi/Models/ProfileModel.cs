using System;
using System.Linq;
using System.Runtime.Serialization;

namespace AuctionBayWebApi.Models
{
    [DataContract]
    public class ProfileModel
    {
        [DataMember(Name = "nickname", IsRequired = true)]
        public string Nickname { get; set; }

        [DataMember(Name = "profileImage", IsRequired = false)]
        public string ProfileImage { get; set; }

        [DataMember(Name = "email", IsRequired = true)]
        public string Email { get; set; }

        [DataMember(Name = "phoneNumber", IsRequired = true)]
        public string PhoneNumber { get; set; }

        [DataMember(Name = "location", IsRequired = true)]
        public string Location { get; set; }

        public int Reputation { get; set; }
    }
}
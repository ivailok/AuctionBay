﻿using System;
using System.Linq;
using System.Runtime.Serialization;

namespace AuctionBayWebApi.Models
{
    [DataContract]
    public class UserLoggedModel
    {
        [DataMember(Name = "sessionKey")]
        public string SessionKey { get; set; }

        [DataMember(Name = "nickname")]
        public string Nickname { get; set; }
    }
}
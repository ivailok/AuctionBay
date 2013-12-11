using System;
using System.Linq;

namespace AuctionBay.Model
{
    public class RegisterModel : LoginModel
    {
        public string Nickname { get; set; }

        public string Location { get; set; }
    }
}

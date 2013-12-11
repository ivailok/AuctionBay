using System;
using System.Linq;

namespace AuctionBayWebApi.Models
{
    public class MyProductPartialModel : ProductPartialModel
    {
        public bool IsTimeOver { get; set; }

        public bool IsSold { get; set; }

        public bool IsActive { get; set; }

        public bool AreNegotiationsActive { get; set; }
    }
}
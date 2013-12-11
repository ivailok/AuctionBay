using System;
using System.Linq;

namespace AuctionBay.DataModels
{
    public class ProfileImage
    {
        public int Id { get; set; }

        public string ImageFolder { get; set; }

        public string ImageName { get; set; }

        public string ImagePublicLocation { get; set; }
    }
}

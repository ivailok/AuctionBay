using System;
using System.Collections.Generic;
using System.Linq;

namespace AuctionBay.Model
{
    public class CountriesContainer
    {
        public IEnumerable<LocationModel> Geonames { get; set; }
    }
}

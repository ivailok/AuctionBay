using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionBay.Model;

namespace AuctionBay.Services
{
    public interface ICountryService
    {
        Task<CountriesContainer> GetCountries();
    }
}

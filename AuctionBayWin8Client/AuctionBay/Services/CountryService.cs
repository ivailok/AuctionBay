using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AuctionBay.Model;
using System.Net.Http;
using Newtonsoft.Json;

namespace AuctionBay.Services
{
    public class CountryService : ICountryService
    {
        private const string BaseUrl = "http://api.geonames.org/countryInfoJSON?part=countryName&username=ivailok1";

        public async Task<CountriesContainer> GetCountries()
        {
            try
            {
                HttpResponseMessage response = await HttpRequester.Get(BaseUrl);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string contentAsString = await response.Content.ReadAsStringAsync();
                    CountriesContainer countries = await JsonConvert.DeserializeObjectAsync<CountriesContainer>(contentAsString);
                    return countries;
                }
                else
                {
                    string errorAsString = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException("Cannot get data.");
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                {
                    throw new InvalidOperationException(e.InnerException.Message + ". Try again in few moments.");
                }
                else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }
    }
}

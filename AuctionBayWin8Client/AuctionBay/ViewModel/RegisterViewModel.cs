using System;
using System.Collections.Generic;
using System.Linq;
using AuctionBay.Services;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using AuctionBay.Model;
using System.Collections.ObjectModel;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.Security.Cryptography.Core;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace AuctionBay.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private ICountryService countryService;

        private bool isRegisterNotTriggered;
        private bool isRegisterTriggered;
        private string errorMessage;
        private int selectedLocation;
        private ObservableCollection<LocationModel> locations;

        public string Username { get; set; }

        public string Nickname { get; set; }

        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
            set
            {
                this.errorMessage = value;
                this.RaisePropertyChanged("ErrorMessage");
            }
        }

        public bool IsRegisterNotTriggered
        {
            get
            {
                return this.isRegisterNotTriggered;
            }
            set
            {
                if (this.isRegisterNotTriggered != value)
                {
                    this.isRegisterNotTriggered = value;
                    this.RaisePropertyChanged("IsRegisterNotTriggered");
                }
            }
        }

        public bool IsRegisterTriggered
        {
            get
            {
                return this.isRegisterTriggered;
            }
            set
            {
                if (this.isRegisterTriggered != value)
                {
                    this.isRegisterTriggered = value;
                    this.RaisePropertyChanged("IsRegisterTriggered");
                }
            }
        }

        public IEnumerable<LocationModel> Locations
        {
            get
            {
                if (this.locations == null)
                {
                    this.Locations = new List<LocationModel>()
                    {
                        new LocationModel() { CountryName = "Default" },
                        new LocationModel() { CountryName = "All" }
                    };
                }
                return this.locations;
            }
            set
            {
                if (this.locations == null)
                {
                    this.locations = new ObservableCollection<LocationModel>();
                }

                this.locations.Clear();
                foreach (var item in value)
                {
                    this.locations.Add(item);
                }
            }
        }

        public int SelectedLocation
        {
            get
            {
                return this.selectedLocation;
            }
            set
            {
                this.selectedLocation = value;
                this.RaisePropertyChanged("SelectedLocation");
            }
        }

        public ICommand Register { get; private set; }

        public ICommand GoToLoginPage { get; private set; }

        public RegisterViewModel()
        {
            this.SelectedLocation = 0;
            this.IsRegisterNotTriggered = true;
            this.IsRegisterTriggered = false;
            this.ErrorMessage = "";
            
            INavigationService navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            IDataService dataService = ServiceLocator.Current.GetInstance<IDataService>();
            this.countryService = ServiceLocator.Current.GetInstance<ICountryService>();

            this.Register = new RelayCommand<PasswordBox>(async (param) =>
                {
                    this.IsRegisterNotTriggered = false;
                    this.IsRegisterTriggered = true;
                    this.ErrorMessage = "";

                    string password = param.Password;
                    string authCode = this.GetSha1Hash(password);

                    string countryName = this.locations[this.SelectedLocation].CountryName;
                    if (countryName == "Default")
                    {
                        countryName = await this.GetCountryName();
                        if (countryName == null)
                        {
                            this.ErrorMessage = "Your location cannot be determined.";
                            this.IsRegisterTriggered = false;
                            this.IsRegisterNotTriggered = true;
                            return;
                        }
                    }

                    RegisterModel registerModel = new RegisterModel()
                    {
                        Username = this.Username,
                        AuthCode = authCode,
                        Nickname = this.Nickname,
                        Location = countryName
                    };

                    try
                    {
                        LoggedModel loggedData = await dataService.Register(registerModel);
                        ((App)App.Current).AuthenticatedUser = loggedData;
                        var vault = new PasswordVault();
                        vault.Add(new PasswordCredential()
                        {
                            Resource = "auctionBay",
                            UserName = this.Username,
                            Password = authCode
                        });
                        navigationService.Navigate(ListedViews.AvailableItems);
                    }
                    catch (InvalidOperationException e)
                    {
                        this.ErrorMessage = e.Message;
                        this.IsRegisterTriggered = false;
                        this.IsRegisterNotTriggered = true;
                    }
                });

            this.GoToLoginPage = new RelayCommand(() =>
                {
                    navigationService.Navigate(ListedViews.Login);
                });
        }

        public async void LoadCountries()
        {
            if (this.locations != null)
            {
                if (this.locations.Count > 2)
                {
                    return;
                }
            }

            try
            {
                var countries = await this.countryService.GetCountries();
                var sortedCountries = countries.Geonames.OrderBy(x => x.CountryName);
                foreach (var country in sortedCountries)
                {
                    this.locations.Add(country);
                }
            }
            catch (InvalidOperationException e)
            {
                this.ErrorMessage = e.Message;
            }
        }

        private string GetSha1Hash(string data)
        {
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);
            HashAlgorithmProvider hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            IBuffer hashBuffer = hashAlgorithm.HashData(buffer);
            var hashedData = CryptographicBuffer.EncodeToBase64String(hashBuffer);
            return hashedData;
        }

        private async Task<string> GetCountryName()
        {
            var geolocator = new Windows.Devices.Geolocation.Geolocator();
            var geoposition = await geolocator.GetGeopositionAsync();
            string countryCode = geoposition.CivicAddress.Country;

            string countryName = null;
            foreach (var country in this.locations)
            {
                if (countryCode == country.CountryCode)
                {
                    countryName = country.CountryName;
                    break;
                }
            }

            return countryName;
        }
    }
}

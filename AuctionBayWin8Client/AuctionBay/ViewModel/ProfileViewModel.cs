using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AuctionBay.Services;
using GalaSoft.MvvmLight;
using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Command;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using AuctionBay.Model;
using System.Collections.Generic;

namespace AuctionBay.ViewModel
{
    public class ProfileViewModel : ViewModelBase
    {
        private IDataService dataService;
        private ICountryService countryService;

        private bool isEventTriggered;

        private string progressMessage;
        private string profileImage;
        private string profileImageErrorMessage;

        private string nickname;
        private string location;
        private string email;
        private string phoneNumber;

        private bool isNewNicknameVisible;
        private bool isNewLocationVisible;
        private bool isNewEmailVisible;
        private bool isNewPhoneNumberVisible;

        private string newNickname;
        private ObservableCollection<LocationModel> locations;
        private int selectedLocation;
        private string newEmail;
        private string newPhoneNumber;

        private string updateProfileErrorMessage;
        private string getProfileErrorMessage;


        public bool IsEventTriggered
        {
            get
            {
                return this.isEventTriggered;
            }
            set
            {
                if (this.isEventTriggered != value)
                {
                    this.isEventTriggered = value;
                    this.RaisePropertyChanged("IsEventTriggered");
                }
            }
        }

        public string ProgressMessage
        {
            get
            {
                return this.progressMessage;
            }
            set
            {
                if (this.progressMessage != value)
                {
                    this.progressMessage = value;
                    this.RaisePropertyChanged("ProgressMessage");
                }
            }
        }

        public string ProfileImage
        {
            get
            {
                return this.profileImage;
            }
            set
            {
                if (this.profileImage != value)
                {
                    this.profileImage = value;
                    this.RaisePropertyChanged("ProfileImage");
                }
            }
        }

        public string ProfileImageErrorMessage
        {
            get
            {
                return this.profileImageErrorMessage;
            }
            set
            {
                if (this.profileImageErrorMessage != value)
                {
                    this.profileImageErrorMessage = value;
                    this.RaisePropertyChanged("ProfileImageErrorMessage");
                }
            }
        }


        public string Nickname
        {
            get
            {
                return this.nickname;
            }
            set
            {
                if (this.nickname != value)
                {
                    this.nickname = value;
                    this.RaisePropertyChanged("Nickname");
                }
            }
        }

        public string Location
        {
            get
            {
                return this.location;
            }
            set
            {
                if (this.location != value)
                {
                    this.location = value;
                    this.RaisePropertyChanged("Location");
                }
            }
        }

        public string Email
        {
            get
            {
                return this.email;
            }
            set
            {
                if (this.email != value)
                {
                    this.email = value;
                    this.RaisePropertyChanged("Email");
                }
            }
        }

        public string PhoneNumber
        {
            get
            {
                return this.phoneNumber;
            }
            set
            {
                if (this.phoneNumber != value)
                {
                    this.phoneNumber = value;
                    this.RaisePropertyChanged("PhoneNumber");
                }
            }
        }


        public bool IsNewNicknameVisible
        {
            get
            {
                return this.isNewNicknameVisible;
            }
            set
            {
                if (this.isNewNicknameVisible != value)
                {
                    this.isNewNicknameVisible = value;
                    this.RaisePropertyChanged("IsNewNicknameVisible");
                }
            }
        }

        public bool IsNewLocationVisible
        {
            get
            {
                return this.isNewLocationVisible;
            }
            set
            {
                if (this.isNewLocationVisible != value)
                {
                    this.isNewLocationVisible = value;
                    this.RaisePropertyChanged("IsNewLocationVisible");
                }
            }
        }

        public bool IsNewEmailVisible
        {
            get
            {
                return this.isNewEmailVisible;
            }
            set
            {
                if (this.isNewEmailVisible != value)
                {
                    this.isNewEmailVisible = value;
                    this.RaisePropertyChanged("IsNewEmailVisible");
                }
            }
        }

        public bool IsNewPhoneNumberVisible
        {
            get
            {
                return this.isNewPhoneNumberVisible;
            }
            set
            {
                if (this.isNewPhoneNumberVisible != value)
                {
                    this.isNewPhoneNumberVisible = value;
                    this.RaisePropertyChanged("IsNewPhoneNumberVisible");
                }
            }
        }


        public string NewNickname
        {
            get
            {
                return this.newNickname;
            }
            set
            {
                if (this.newNickname != value)
                {
                    this.newNickname = value;
                    this.RaisePropertyChanged("NewNickname");
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

        public string NewEmail
        {
            get
            {
                return this.newEmail;
            }
            set
            {
                if (this.newEmail != value)
                {
                    this.newEmail = value;
                    this.RaisePropertyChanged("NewEmail");
                }
            }
        }

        public string NewPhoneNumber
        {
            get
            {
                return this.newPhoneNumber;
            }
            set
            {
                if (this.newPhoneNumber != value)
                {
                    this.newPhoneNumber = value;
                    this.RaisePropertyChanged("NewPhoneNumber");
                }
            }
        }

        public string UpdateProfileErrorMessage
        {
            get
            {
                return this.updateProfileErrorMessage;
            }
            set
            {
                if (this.updateProfileErrorMessage != value)
                {
                    this.updateProfileErrorMessage = value;
                    this.RaisePropertyChanged("UpdateProfileErrorMessage");
                }
            }
        }

        public string GetProfileErrorMessage
        {
            get
            {
                return this.getProfileErrorMessage;
            }
            set
            {
                if (this.getProfileErrorMessage != value)
                {
                    this.getProfileErrorMessage = value;
                    this.RaisePropertyChanged("GetProfileErrorMessage");
                }
            }
        }


        public ICommand StartUpload { get; private set; }

        public ICommand Update { get; private set; }

        public ICommand Edit { get; private set; }


        public ProfileViewModel()
        {
            this.countryService = ServiceLocator.Current.GetInstance<ICountryService>();
            this.dataService = ServiceLocator.Current.GetInstance<IDataService>();
            INavigationService navigationService = ServiceLocator.Current.GetInstance<INavigationService>();

            this.ProgressMessage = "";

            this.StartUpload = new RelayCommand<IStorageFile>(async (file) =>
                {
                    try
                    {
                        this.IsEventTriggered = true;
                        Progress<UploadOperation> uploadProgressCallback = new Progress<UploadOperation>(UploadProgress);
                        await dataService.UploadProfileImage(file, uploadProgressCallback, ((App)App.Current).AuthenticatedUser.SessionKey);
                        await this.LoadData();
                        this.ProgressMessage = "";
                        this.IsEventTriggered = false;
                    }
                    catch (InvalidOperationException e)
                    {
                        this.ProfileImageErrorMessage = e.Message;
                        this.IsEventTriggered = false;
                    }
                });

            this.Update = new RelayCommand(async () =>
                {
                    try
                    {
                        this.IsEventTriggered = true;
                        string countryName = this.locations[this.SelectedLocation].CountryName;
                        if (countryName == "Default")
                        {
                            countryName = await this.GetCountryName();
                            if (countryName == null)
                            {
                                throw new InvalidOperationException("Your location cannot be determined.");
                            }
                        }

                        ProfileModel profile = new ProfileModel()
                        {
                            Nickname = this.IsNewNicknameVisible ? this.NewNickname : this.Nickname,
                            Location = this.IsNewLocationVisible ? countryName : this.Location,
                            Email = this.IsNewEmailVisible ? this.NewEmail : this.Email,
                            PhoneNumber = this.IsNewPhoneNumberVisible ? this.NewPhoneNumber : this.PhoneNumber
                        };

                        await dataService.UpdateProfile(((App)App.Current).AuthenticatedUser.SessionKey, profile);
                        ((App)App.Current).AuthenticatedUser.Nickname = profile.Nickname;

                        this.IsEventTriggered = false;
                        navigationService.Navigate(ListedViews.Profile);
                    }
                    catch (InvalidOperationException e)
                    {
                        this.UpdateProfileErrorMessage = e.Message;
                        this.IsEventTriggered = false;
                    }
                });

            this.Edit = new RelayCommand<string>((param) =>
                {
                    switch (param)
                    {
                        case "Nickname":
                            {
                                this.IsNewNicknameVisible = true;
                                break;
                            }
                        case "Email":
                            {
                                this.IsNewEmailVisible = true;
                                break;
                            }
                        case "Location":
                            {
                                this.IsNewLocationVisible = true;
                                break;
                            }
                        case "PhoneNumber":
                            {
                                this.IsNewPhoneNumberVisible = true;
                                break;
                            }
                        default:
                            {
                                throw new NotSupportedException();
                            }
                    }
                });
        }

        public async Task LoadData()
        {
            try
            {
                this.IsEventTriggered = true;

                ProfileModel profileData = await dataService.GetProfile(((App)App.Current).AuthenticatedUser.SessionKey);
                this.Nickname = profileData.Nickname;
                this.Location = profileData.Location;
                this.Email = profileData.Email;
                this.PhoneNumber = profileData.PhoneNumber;
                this.ProfileImage = profileData.ProfileImage;

                this.IsEventTriggered = false;
            }
            catch (InvalidOperationException e)
            {
                this.GetProfileErrorMessage = e.Message;
                this.IsEventTriggered = false;
            }
        }

        public async Task LoadCountries()
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
                this.IsEventTriggered = true;

                var countries = await this.countryService.GetCountries();
                var sortedCountries = countries.Geonames.OrderBy(x => x.CountryName);
                foreach (var country in sortedCountries)
                {
                    this.locations.Add(country);
                }

                this.IsEventTriggered = false;
            }
            catch (InvalidOperationException e)
            {
                this.UpdateProfileErrorMessage = e.Message;

                this.IsEventTriggered = false;
            }
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

        private void UploadProgress(UploadOperation obj)
        {
            var totalBytes = obj.Progress.TotalBytesToSend;
            var currentBytes = obj.Progress.BytesSent;
            var percentage = currentBytes / totalBytes * 100;
            this.ProgressMessage = string.Format("{0:00.00}%", percentage);

            if (percentage == 100)
            {
                this.ProgressMessage = "Uploaded";
            }
        }
    }
}

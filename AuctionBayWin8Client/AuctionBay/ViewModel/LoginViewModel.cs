using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using AuctionBay.Services;
using Microsoft.Practices.ServiceLocation;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.Security.Cryptography.Core;
using Windows.UI.Xaml.Controls;
using AuctionBay.Model;
using Windows.Security.Credentials;

namespace AuctionBay.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string errorMessage;

        private bool isLoginTriggered;
        private bool isLoginNotTriggered;

        public ICommand Login { get; private set; }

        public ICommand GoToRegisterPage { get; private set; }

        public string Username { get; set; }

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

        public bool IsLoginNotTriggered
        {
            get
            {
                return this.isLoginNotTriggered;
            }
            set
            {
                if (this.isLoginNotTriggered != value)
                {
                    this.isLoginNotTriggered = value;
                    this.RaisePropertyChanged("IsLoginNotTriggered");
                }
            }
        }

        public bool IsLoginTriggered
        {
            get
            {
                return this.isLoginTriggered;
            }
            set
            {
                if (this.isLoginTriggered != value)
                {
                    this.isLoginTriggered = value;
                    this.RaisePropertyChanged("IsLoginTriggered");
                }
            }
        }

        public LoginViewModel()
        {
            this.IsLoginNotTriggered = true;
            this.IsLoginTriggered = false;
            this.ErrorMessage = "";

            INavigationService navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            IDataService dataService = ServiceLocator.Current.GetInstance<IDataService>();

            this.Login = new RelayCommand<PasswordBox>(async (param) =>
                {
                    this.ErrorMessage = "";
                    this.IsLoginNotTriggered = false;
                    this.IsLoginTriggered = true;

                    var password = param.Password;
                    var authCode = this.GetSha1Hash(password);
                    try
                    {
                        LoggedModel loggedData = await dataService.Login(new LoginModel() { Username = this.Username, AuthCode = authCode });
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
                        this.IsLoginTriggered = false;
                        this.IsLoginNotTriggered = true;
                    }
                });

            this.GoToRegisterPage = new RelayCommand(() =>
                {
                    navigationService.Navigate(ListedViews.Register);
                });
        }

        private string GetSha1Hash(string data)
        {
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);
            HashAlgorithmProvider hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            IBuffer hashBuffer = hashAlgorithm.HashData(buffer);
            var hashedData = CryptographicBuffer.EncodeToBase64String(hashBuffer);
            return hashedData;
        }
    }
}

using AuctionBay.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.Windows.Input;
using Windows.Security.Credentials;

namespace AuctionBay.ViewModel
{
    public class NavigationViewModel : ViewModelBase
    {
        private string nickname;

        public string Nickname
        {
            get
            {
                var realNickname = ((App)App.Current).AuthenticatedUser.Nickname;

                if (nickname == null || nickname != realNickname)
                {
                    nickname = realNickname;
                    this.RaisePropertyChanged("Nickname");
                }
                return nickname;
            }
        }

        public ICommand NavigateCommand { get; private set; }

        public ICommand GoBack { get; private set; }

        public ICommand Logout { get; private set; }

        public NavigationViewModel()
        {
            INavigationService navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            IDataService dataService = ServiceLocator.Current.GetInstance<IDataService>();

            this.NavigateCommand = new RelayCommand<string>((s) =>
            {
                var view = (ListedViews)Enum.Parse(typeof(ListedViews), s);

                navigationService.Navigate(view);
            });

            this.GoBack = new RelayCommand(() =>
                {
                    navigationService.GoBack();
                });

            this.Logout = new RelayCommand(async () =>
                {
                    await dataService.Logout(((App)App.Current).AuthenticatedUser.SessionKey);
                    var vault = new PasswordVault();
                    var credetential = vault.FindAllByResource("auctionBay").FirstOrDefault();
                    vault.Remove(credetential);
                    navigationService.Navigate(ListedViews.Login);
                });
        }
    }
}

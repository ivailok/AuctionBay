using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AuctionBay.Model;
using GalaSoft.MvvmLight;
using AuctionBay.Services;
using Microsoft.Practices.ServiceLocation;

namespace AuctionBay.ViewModel
{
    public class MyOffersViewModel : ViewModelBase
    {
        private IDataService dataService;

        private string errorMessage;
        private bool loadingOffers;

        private ObservableCollection<OfferMadeModel> offers;

        public IEnumerable<OfferMadeModel> Offers
        {
            get
            {
                return this.offers;
            }
            set
            {
                if (this.offers == null)
                {
                    this.offers = new ObservableCollection<OfferMadeModel>();
                }
                this.offers.Clear();
                foreach (var item in value)
                {
                    this.offers.Add(item);
                }
                this.RaisePropertyChanged("Offers");
            }
        }

        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
            set
            {
                if (this.errorMessage != value)
                {
                    this.errorMessage = value;
                    this.RaisePropertyChanged("ErrorMessage");
                }
            }
        }

        public bool LoadingOffers
        {
            get
            {
                return this.loadingOffers;
            }
            set
            {
                this.loadingOffers = value;
                this.RaisePropertyChanged("LoadingOffers");
            }
        }

        public MyOffersViewModel()
        {
            this.dataService = ServiceLocator.Current.GetInstance<IDataService>();
        }

        public async Task Load()
        {
            try
            {
                this.LoadingOffers = true;
                this.Offers = await this.dataService.GetMyOffers(((App)App.Current).AuthenticatedUser.SessionKey);
                this.LoadingOffers = false;
            }
            catch (InvalidOperationException e)
            {
                this.ErrorMessage = e.Message;
                this.LoadingOffers = false;
            }
        }
    }
}

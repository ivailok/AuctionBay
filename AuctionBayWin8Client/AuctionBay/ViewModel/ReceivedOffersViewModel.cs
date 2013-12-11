using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AuctionBay.Services;
using GalaSoft.MvvmLight;
using AuctionBay.Model;
using Microsoft.Practices.ServiceLocation;

namespace AuctionBay.ViewModel
{
    public class ReceivedOffersViewModel : ViewModelBase
    {
        private IDataService dataService;

        private string errorMessage;
        private bool loadingOffers;

        private ObservableCollection<OfferModel> offers;

        public IEnumerable<OfferModel> Offers
        {
            get
            {
                return this.offers;
            }
            set
            {
                if (this.offers == null)
                {
                    this.offers = new ObservableCollection<OfferModel>();
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

        public ReceivedOffersViewModel()
        {
            this.dataService = ServiceLocator.Current.GetInstance<IDataService>();
        }

        public async Task Load()
        {
            try
            {
                this.LoadingOffers = true;
                this.Offers = await this.dataService.GetReceivedOffers(((App)App.Current).AuthenticatedUser.SessionKey);
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

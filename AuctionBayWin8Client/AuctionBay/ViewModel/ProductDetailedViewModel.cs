using System.Windows.Input;
using AuctionBay.Model;
using AuctionBay.Services;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace AuctionBay.ViewModel
{
    public class ProductDetailedViewModel : ViewModelBase
    {
        private IDataService dataService;

        private bool isLoading;

        private int id;
        private string title;
        private string description;
        private string auctioneer;
        private string currentBidder;
        private decimal currentPrice;
        private TimeSpan biddingTimeLeft;

        private bool isBiddable;
        private bool isNotBiddable;

        private string errorMessage;

        private int selectedBid;
        private ObservableCollection<BidModel> bids;
        private ObservableCollection<string> images;
        private int reputation;

        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }
            set
            {
                if (this.isLoading != value)
                {
                    this.isLoading = value;
                    this.RaisePropertyChanged("IsLoading");
                }
            }
        }

        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    this.RaisePropertyChanged("Title");
                }
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (this.description != value)
                {
                    this.description = value;
                    this.RaisePropertyChanged("Description");
                }
            }
        }

        public decimal CurrentPrice
        {
            get
            {
                return this.currentPrice;
            }
            set
            {
                if (this.currentPrice != value)
                {
                    this.currentPrice = value;
                    this.RaisePropertyChanged("CurrentPrice");
                }
            }
        }

        public string CurrentBidder
        {
            get
            {
                return this.currentBidder;
            }
            set
            {
                if (this.currentBidder != value)
                {
                    this.currentBidder = value;
                    this.RaisePropertyChanged("CurrentBidder");
                }
            }
        }

        public string Auctioneer
        {
            get
            {
                return this.auctioneer;
            }
            set
            {
                if (this.auctioneer != value)
                {
                    this.auctioneer = value;
                    this.RaisePropertyChanged("Auctioneer");
                }
            }
        }

        public int Reputation
        {
            get
            {
                return this.reputation;
            }
            set
            {
                if (this.reputation != value)
                {
                    this.reputation = value;
                    this.RaisePropertyChanged("Reputation");
                }
            }
        }

        public TimeSpan BiddingTimeLeft
        {
            get
            {
                return this.biddingTimeLeft;
            }
            set
            {
                if (this.biddingTimeLeft != value)
                {
                    this.biddingTimeLeft = value;
                    this.RaisePropertyChanged("BiddingTimeLeft");
                }
            }
        }

        public IEnumerable<string> Images 
        {
            get
            {
                return this.images;
            }
            set
            {
                if (this.images == null)
                {
                    this.images = new ObservableCollection<string>();
                }
                this.images.Clear();
                foreach (var item in value)
                {
                    this.images.Add(item);
                }
                this.RaisePropertyChanged("Images");
            }
        }

        public IEnumerable<BidModel> Bids
        {
            get
            {
                if (this.bids == null)
                {
                    this.Bids = new List<BidModel>()
                    {
                        new BidModel() { Bid = "0,05" },
                        new BidModel() { Bid = "0,10" },
                        new BidModel() { Bid = "0,20" },
                        new BidModel() { Bid = "0,50" },
                        new BidModel() { Bid = "1,00" },
                        new BidModel() { Bid = "2,00" },
                        new BidModel() { Bid = "5,00" },
                        new BidModel() { Bid = "10,00" },
                        new BidModel() { Bid = "20,00" },
                        new BidModel() { Bid = "50,00" },
                        new BidModel() { Bid = "100,00" }
                    };
                }
                return this.bids;
            }
            set
            {
                if (this.bids == null)
                {
                    this.bids = new ObservableCollection<BidModel>();
                }

                this.bids.Clear();
                foreach (var item in value)
                {
                    this.bids.Add(item);
                }
            }
        }

        public int SelectedBid
        {
            get
            {
                return this.selectedBid;
            }
            set
            {
                this.selectedBid = value;
                this.RaisePropertyChanged("SelectedLocation");
            }
        }

        public bool IsBiddable
        {
            get
            {
                return this.isBiddable;
            }
            set
            {
                if (this.isBiddable != value)
                {
                    this.isBiddable = value;
                    this.RaisePropertyChanged("IsBiddable");
                }
            }
        }

        public bool IsNotBiddable
        {
            get
            {
                return this.isNotBiddable;
            }
            set
            {
                if (this.isNotBiddable != value)
                {
                    this.isNotBiddable = value;
                    this.RaisePropertyChanged("IsNotBiddable");
                }
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

        public ICommand Bid { get; private set; }

        public ICommand Buyout { get; private set; }

        public ProductDetailedViewModel()
        {
            this.isBiddable = true;
            this.isNotBiddable = false;

            this.dataService = ServiceLocator.Current.GetInstance<IDataService>();
            INavigationService navigationService = ServiceLocator.Current.GetInstance<INavigationService>();

            this.Bid = new RelayCommand(async () =>
                {
                    try
                    {
                        this.IsLoading = true;
                        await this.dataService.Bid(((App)App.Current).AuthenticatedUser.SessionKey, this.Id, this.bids[this.SelectedBid].Bid);
                        await this.Load();
                    }
                    catch (InvalidOperationException e)
                    {
                        this.ErrorMessage = e.Message;
                        this.IsLoading = false;
                    }
                });

            this.Buyout = new RelayCommand(async () =>
                {
                    try
                    {
                        this.IsLoading = true;
                        await this.dataService.Buyout(((App)App.Current).AuthenticatedUser.SessionKey, this.Id);
                        navigationService.Navigate(ListedViews.AvailableItems);

                        var xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml("<toast><visual version=\"1\"><binding template=\"ToastText01\"><text id=\"1\">You have successfully bought out the item</text></binding></visual></toast>");
                        var notification = new ToastNotification(xmlDocument);
                        var toastNotifier = ToastNotificationManager.CreateToastNotifier();
                        toastNotifier.Show(notification);
                    }
                    catch (InvalidOperationException e)
                    {
                        this.ErrorMessage = e.Message;
                        this.IsLoading = false;
                    }
                });
        }

        public async Task Load()
        {
            try
            {
                this.IsLoading = true;
                ProductDetailedModel product = await this.dataService.ViewProduct(((App)App.Current).AuthenticatedUser.SessionKey, this.Id);
                this.Title = product.Title;
                this.CurrentPrice = product.CurrentPrice;
                this.CurrentBidder = product.CurrentBidder;
                this.Description = product.Description;
                this.BiddingTimeLeft = product.BiddingTimeLeft;
                this.Auctioneer = product.Auctioneer;
                this.Images = product.Images;
                this.Reputation = product.AuctioneerReputation;
                if (product.BidingType == "Direct")
                {
                    this.IsNotBiddable = true;
                    this.IsBiddable = false;
                }
                else
                {
                    this.IsBiddable = true;
                    this.IsNotBiddable = false;
                }
                if (this.CurrentBidder == ((App)App.Current).AuthenticatedUser.Nickname)
                {
                    this.IsBiddable = false;
                }

                this.IsLoading = false;
            }
            catch (InvalidOperationException e)
            {
                this.ErrorMessage = e.Message;
                this.IsLoading = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionBay.Model;
using AuctionBay.Services;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace AuctionBay.ViewModel
{
    public class AddNewItemViewModel : ViewModelBase
    {
        private IDataService dataService;

        private bool isEventTriggered;

        private string title;
        private string description;
        private decimal startingPrice;

        private int selectedBiddingType;
        private ObservableCollection<BiddingTypeModel> biddingTypes;
        private ObservableCollection<BitmapImage> images;
        private IEnumerable<IStorageFile> files;

        private string errorMessage;
        private string progressMessage;

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

        public decimal StartingPrice
        {
            get
            {
                return this.startingPrice;
            }
            set
            {
                if (this.startingPrice != value)
                {
                    this.startingPrice = value;
                    this.RaisePropertyChanged("StartingPrice");
                }
            }
        }

        public IEnumerable<BiddingTypeModel> BiddingTypes
        {
            get
            {
                if (this.biddingTypes == null)
                {
                    this.BiddingTypes = new List<BiddingTypeModel>()
                    {
                        new BiddingTypeModel() { BiddingType = "48 hours" },
                        new BiddingTypeModel() { BiddingType = "Direct" }
                    };
                }
                return this.biddingTypes;
            }
            set
            {
                if (this.biddingTypes == null)
                {
                    this.biddingTypes = new ObservableCollection<BiddingTypeModel>();
                }

                this.biddingTypes.Clear();
                foreach (var item in value)
                {
                    this.biddingTypes.Add(item);
                }
            }
        }

        public IEnumerable<BitmapImage> Images
        {
            get
            {
                return this.images;
            }
            set
            {
                if (this.images == null)
                {
                    this.images = new ObservableCollection<BitmapImage>();
                }

                this.images.Clear();
                foreach (var item in value)
                {
                    this.images.Add(item);
                }
            }
        }

        public IEnumerable<IStorageFile> Files
        {
            get
            {
                return this.files;
            }
            set
            {
                this.files = value;
            }
        }

        public int SelectedBiddingType
        {
            get
            {
                return this.selectedBiddingType;
            }
            set
            {
                this.selectedBiddingType = value;
                this.RaisePropertyChanged("SelectedBiddingType");
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

        public ICommand AttachImages { get; private set; }

        public ICommand AddProduct { get; private set; }

        public AddNewItemViewModel()
        {
            this.images = new ObservableCollection<BitmapImage>();

            this.dataService = ServiceLocator.Current.GetInstance<IDataService>();
            INavigationService navigationService = ServiceLocator.Current.GetInstance<INavigationService>();

            this.AddProduct = new RelayCommand(async () =>
                {
                    if (this.Title == "" || this.Title == null)
                    {
                        this.ErrorMessage = "Title is an obligatory field and should not be empty.";
                        return;
                    }

                    if (this.Description == "" || this.Description == null)
                    {
                        this.ErrorMessage = "Description is an obligatory field and should not be empty.";
                        return;
                    }

                    if (this.StartingPrice == default(decimal))
                    {
                        this.ErrorMessage = "Starting price could not be zero.";
                        return;
                    }

                    if (this.images.Count == 0)
                    {
                        this.ErrorMessage = "At least one image must be provided.";
                        return;
                    }

                    this.ErrorMessage = "";
                    ProductFullModel product = new ProductFullModel()
                    {
                        Title = this.Title,
                        Description = this.Description,
                        BidingType = this.biddingTypes[this.SelectedBiddingType].BiddingType,
                        StartingPrice = this.StartingPrice
                    };

                    try
                    {
                        this.IsEventTriggered = true;
                        int productId = await this.dataService.AddProduct(product, ((App)App.Current).AuthenticatedUser.SessionKey);
                        Progress<UploadOperation> uploadProgressCallback = new Progress<UploadOperation>(UploadProgress);
                        await this.dataService.UploadImages(this.Files, uploadProgressCallback, ((App)App.Current).AuthenticatedUser.SessionKey, productId);
                        this.IsEventTriggered = false;
                        navigationService.Navigate(ListedViews.AvailableItems);
                    }
                    catch (InvalidOperationException e)
                    {
                        this.ErrorMessage = e.Message;
                        this.IsEventTriggered = false;
                    }
                });

            this.AttachImages = new RelayCommand<IEnumerable<IStorageFile>>(async (files) =>
                {
                    this.IsEventTriggered = true;
                    this.Files = files;
                    foreach (var file in files)
                    {
                        BitmapImage img = new BitmapImage();
                        img.SetSource(await file.OpenReadAsync());
                        this.images.Add(img);
                    }
                    this.IsEventTriggered = false;
                });
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

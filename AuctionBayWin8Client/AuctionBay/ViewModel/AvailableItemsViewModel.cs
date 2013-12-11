using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionBay.Model;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using AuctionBay.Services;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Command;

namespace AuctionBay.ViewModel
{
    public class AvailableItemsViewModel : ViewModelBase
    {
        private IDataService dataService;

        private string errorMessage;
        private bool loadingProducts;

        private ObservableCollection<ProductPartialModel> products;

        public IEnumerable<ProductPartialModel> Products
        {
            get
            {
                return this.products;
            }
            set
            {
                if (this.products == null)
                {
                    this.products = new ObservableCollection<ProductPartialModel>();
                }
                if (this.products != value)
                {
                    this.products.Clear();
                    foreach (var product in value)
                    {
                        this.products.Add(product);
                    }
                    this.RaisePropertyChanged("Products");
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

        public bool LoadingProducts
        {
            get
            {
                return this.loadingProducts;
            }
            set
            {
                this.loadingProducts = value;
                this.RaisePropertyChanged("LoadingProducts");
            }
        }

        public ICommand AddNewItem { get; private set; }

        public ICommand ProductNavigationAction { get; private set; }

        public AvailableItemsViewModel()
        {
            INavigationService navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            this.dataService = ServiceLocator.Current.GetInstance<IDataService>();

            this.AddNewItem = new RelayCommand(() =>
                {
                    navigationService.Navigate(ListedViews.AddNewItem);
                });

            this.ProductNavigationAction = new RelayCommand<ProductPartialModel>((product) =>
                {
                    navigationService.Navigate(ListedViews.DetailedProduct, product.Id);
                });
        }

        public async void LoadProducts()
        {
            try
            {
                this.LoadingProducts = true;
                this.Products = await this.dataService.GetProducts(((App)App.Current).AuthenticatedUser.SessionKey);
                this.LoadingProducts = false;
            }
            catch (InvalidOperationException e)
            {
                this.ErrorMessage = e.Message;
            }
        }
    }
}

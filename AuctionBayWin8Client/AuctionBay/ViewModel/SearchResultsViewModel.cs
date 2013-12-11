using System.Collections.ObjectModel;
using System.Windows.Input;
using AuctionBay.Model;
using AuctionBay.Services;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Command;

namespace AuctionBay.ViewModel
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private IDataService dataService;

        private string queryText;
        private bool loadingProducts;

        private ObservableCollection<ProductPartialModel> products;

        public string QueryText
        {
            get
            {
                return this.queryText;
            }
            set
            {
                this.queryText = value;
                this.RaisePropertyChanged("QueryText");
                this.LoadProducts();
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

        public IEnumerable<ProductPartialModel> Products
        {
            get
            {
                if (this.products == null)
                {
                    products = new ObservableCollection<ProductPartialModel>();
                }
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

        public ICommand ProductNavigationAction { get; private set; }

        public SearchResultsViewModel()
        {
            INavigationService navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            this.dataService = ServiceLocator.Current.GetInstance<IDataService>();

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
                this.Products = new ObservableCollection<ProductPartialModel>();
                var products = await this.dataService.GetProducts(((App)App.Current).AuthenticatedUser.SessionKey);
                foreach (var product in products)
                {
                    if (product.Title.ToLower().Contains(this.QueryText))
                    {
                        this.products.Add(product);
                    }
                }
                this.LoadingProducts = false;
            }
            catch (Exception)
            {

            }
        }
    }
}

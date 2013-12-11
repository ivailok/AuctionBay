/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:AuctionBay"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using AuctionBay.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace AuctionBay.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();

            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<IDataService, ExternalDataService>();
            SimpleIoc.Default.Register<ICountryService, CountryService>();

            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<RegisterViewModel>();
            SimpleIoc.Default.Register<NavigationViewModel>();
            SimpleIoc.Default.Register<AvailableItemsViewModel>();
            SimpleIoc.Default.Register<AddNewItemViewModel>();
            SimpleIoc.Default.Register<ProfileViewModel>();
            SimpleIoc.Default.Register<ProductDetailedViewModel>();
            SimpleIoc.Default.Register<SearchResultsViewModel>();
            SimpleIoc.Default.Register<MyOffersViewModel>();
            SimpleIoc.Default.Register<ReceivedOffersViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ViewModelBase LoginViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }

        public ViewModelBase RegisterViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RegisterViewModel>();
            }
        }

        public ViewModelBase AvailableItemsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AvailableItemsViewModel>();
            }
        }

        public ViewModelBase NavigationViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NavigationViewModel>();
            }
        }

        public ViewModelBase AddNewItemViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddNewItemViewModel>();
            }
        }

        public ViewModelBase ProfileViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ProfileViewModel>();
            }
        }

        public ViewModelBase ProductDetailedViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ProductDetailedViewModel>();
            }
        }

        public ViewModelBase SearchResultsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SearchResultsViewModel>();
            }
        }

        public ViewModelBase MyOffersViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyOffersViewModel>();
            }
        }

        public ViewModelBase ReceivedOffersViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReceivedOffersViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
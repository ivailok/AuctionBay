using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AuctionBay.Services
{
    public class NavigationService : INavigationService
    {
        private Type GetViewType(ListedViews view)
        {
            switch (view)
            {
                case ListedViews.Login:
                    return typeof(View.LoginPage);
                case ListedViews.Register:
                    return typeof(View.RegisterPage);
                case ListedViews.AvailableItems:
                    return typeof(View.AvailableItemsPage);
                case ListedViews.AddNewItem:
                    return typeof(View.AddNewItemPage);
                case ListedViews.Profile:
                    return typeof(View.ProfilePage);
                case ListedViews.DetailedProduct:
                    return typeof(View.ProductDetailedPage);
                case ListedViews.MyOffers:
                    return typeof(View.MyOffers);
                case ListedViews.ReceivedOffers:
                    return typeof(View.ReceivedOffers);
                default:
                    break;
            }

            return null;
        }

        public void Navigate(ListedViews view)
        {
            var pageType = this.GetViewType(view);

            if (pageType != null)
            {
                ((Frame)Window.Current.Content).Navigate(pageType);
            }
        }

        public void Navigate(ListedViews view, object parameter)
        {
            var pageType = this.GetViewType(view);

            if (pageType != null)
            {
                ((Frame)Window.Current.Content).Navigate(pageType, parameter);
            }
        }

        public void GoBack()
        {
            var originalPage = ((Frame)Window.Current.Content).CurrentSourcePageType.Name;
            //string state = ((Frame)Window.Current.Content).GetNavigationState();
            //string originalQueryText = null;
            //if (originalPage == "SearchResultsPage")
            //{
            //    originalQueryText = state.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Last();
            //}

            ((Frame)Window.Current.Content).GoBack();

            var currentPage = ((Frame)Window.Current.Content).CurrentSourcePageType.Name;
            //state = ((Frame)Window.Current.Content).GetNavigationState();
            //string queryText = null;
            //if (currentPage == "SearchResultsPage")
            //{
            //    queryText = state.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Last();
            //}

            while (currentPage == originalPage && (currentPage != "SearchResultsPage"))
            {
                ((Frame)Window.Current.Content).GoBack();
                currentPage = ((Frame)Window.Current.Content).CurrentSourcePageType.Name;
            }

            //while (currentPage == "SearchResultsPage" && queryText == originalQueryText)
            //{
            //    ((Frame)Window.Current.Content).GoBack();
            //    currentPage = ((Frame)Window.Current.Content).CurrentSourcePageType.Name;
            //    state = ((Frame)Window.Current.Content).GetNavigationState();
            //    if (currentPage == "SearchResultsPage")
            //    {
            //        queryText = state.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Last();
            //    }
            //}
        }

        public void GoBack(int levels)
        {
            try
            {
                for (int i = 0; i < levels; i++)
                {
                    ((Frame)Window.Current.Content).GoBack();
                }
            }
            catch
            {
                //TODO: 
                // if GoBack() could not be completed, the method is stopped
            }
        }
    }
}

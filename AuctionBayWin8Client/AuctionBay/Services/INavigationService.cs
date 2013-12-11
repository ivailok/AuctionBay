using System;
using System.Linq;

namespace AuctionBay.Services
{
    public interface INavigationService
    {
        void Navigate(ListedViews view);
        void Navigate(ListedViews view, object parameter);
        void GoBack();
        void GoBack(int levels);
    }
}

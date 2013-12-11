using AuctionBay.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AuctionBay.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProductDetailedPage : Page
    {
        public ProductDetailedPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var currentData = this.DataContext as ProductDetailedViewModel;
            currentData.IsLoading = false;
            currentData.Id = (int)e.Parameter;
            await currentData.Load();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var currentData = this.DataContext as ProductDetailedViewModel;
            currentData.Title = "";
            currentData.CurrentPrice = 0.0M;
            currentData.Description = "";
            currentData.BiddingTimeLeft = new TimeSpan();
            currentData.Auctioneer = "";
            currentData.Images = new List<string>();
            currentData.IsBiddable = true;
            currentData.IsNotBiddable = false;
        }
    }
}

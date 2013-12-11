using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AuctionBay.ViewModel;
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
    public sealed partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var currentData = this.DataContext as ProfileViewModel;
            await currentData.LoadData();
            await currentData.LoadCountries();

            currentData.IsEventTriggered = false;
            currentData.IsNewNicknameVisible = false;
            currentData.IsNewEmailVisible = false;
            currentData.IsNewLocationVisible = false;
            currentData.IsNewPhoneNumberVisible = false;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var currentData = this.DataContext as ProfileViewModel;
            currentData.ProgressMessage = "";
            currentData.ProfileImageErrorMessage = "";
            currentData.UpdateProfileErrorMessage = "";
            currentData.GetProfileErrorMessage = "";
            currentData.Nickname = "";
            currentData.Location = "";
            currentData.PhoneNumber = "";
            currentData.Email = "";
            currentData.ProfileImage = "";
        }
    }
}

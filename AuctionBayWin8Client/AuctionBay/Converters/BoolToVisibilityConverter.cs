using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AuctionBay.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(Visibility))
            {
                return null;
            }
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            var isVisible = bool.Parse(value.ToString());

            return (isVisible) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace AuctionBay.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
            {
                return null;
            }

            var time = (TimeSpan)value;
            string toString = time.Days + "d " + time.Hours + "h " + time.Minutes + "m";
            return toString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}

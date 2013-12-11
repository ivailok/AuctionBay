using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace AuctionBay.Converters
{
    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
            {
                return null;
            }
            if ((decimal)value == default(decimal))
            {
                return "";
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == "" || value == null)
            {
                return default(decimal);
            }

            decimal converted;
            if (decimal.TryParse(value.ToString(), out converted))
            {
                return converted;
            }
            else
            {
                return default(decimal);
            }
        }
    }
}

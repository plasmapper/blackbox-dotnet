using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Data;

namespace BlackBoxConfigurator.View
{
    /// <summary>
    /// View converter that converts IP address to string.
    /// </summary>
    internal class IpAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ((IPAddress)value).ToString();
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return IPAddress.Parse((string)value);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}

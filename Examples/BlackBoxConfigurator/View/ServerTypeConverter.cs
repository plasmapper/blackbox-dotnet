using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlackBoxConfigurator.View
{
    /// <summary>
    /// View converter that converts server type to string.
    /// </summary>
    internal class ServerTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                PL.BlackBox.ServerType.StreamServer => "Stream server",
                PL.BlackBox.ServerType.NetworkServer => "Network server",
                PL.BlackBox.ServerType.StreamModbusServer => "Stream Modbus server",
                PL.BlackBox.ServerType.NetworkModbusServer => "Network Modbus server",
                PL.BlackBox.ServerType.HttpServer => "HTTP server",
                PL.BlackBox.ServerType.MdnsServer => "mDNS server",
                _ => "Unknown"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}

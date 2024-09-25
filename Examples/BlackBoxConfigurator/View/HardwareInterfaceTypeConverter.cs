using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlackBoxConfigurator.View
{
    /// <summary>
    /// View converter that converts hardware interface type to string.
    /// </summary>
    internal class HardwareInterfaceTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                PL.BlackBox.HardwareInterfaceType.Uart => "Uart",
                PL.BlackBox.HardwareInterfaceType.NetworkInterface => "Network interface",
                PL.BlackBox.HardwareInterfaceType.Ethernet => "Ethernet",
                PL.BlackBox.HardwareInterfaceType.WifiStation => "Wi-Fi station",
                PL.BlackBox.HardwareInterfaceType.UsbDeviceCdc => "USB device CDC",
                _ => "Unknown"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}

using System;
using System.Linq;
using System.Net;

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox hardware interface configuration.
    /// </summary>
    public class HardwareInterfaceConfiguration
    {
        private bool _isEnabled;
        private uint _uartBaudRate;
        private ushort _uartDataBits;
        private UartParity _uartParity;
        private UartStopBits _uartStopBits;
        private UartFlowControl _uartFlowControl;
        private bool _ipV4DhcpClientIsEnabled;
        private bool _ipV6DhcpClientIsEnabled;
        private IPAddress _ipV4Address;
        private IPAddress _ipV4Netmask;
        private IPAddress _ipV4Gateway;
        private IPAddress _ipV6GlobalAddress;
        private string _wifiSsid;

        /// <summary>
        /// Gets the hardware interface type.
        /// </summary>
        public HardwareInterfaceType HardwareInterfaceType { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the hardware interface is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            internal set => _isEnabled = value;
        }

        /// <summary>
        /// Gets the UART baud rate.
        /// </summary>
        public uint UartBaudRate
        {
            get => HardwareInterfaceType == HardwareInterfaceType.Uart ? _uartBaudRate : throw new NotSupportedException();
            internal set => _uartBaudRate = value;
        }

        /// <summary>
        /// Gets the UART data bits.
        /// </summary>
        public ushort UartDataBits
        {
            get => HardwareInterfaceType == HardwareInterfaceType.Uart ? _uartDataBits : throw new NotSupportedException();
            internal set => _uartDataBits = value;
        }

        /// <summary>
        /// Gets the UART parity.
        /// </summary>
        public UartParity UartParity
        {
            get => HardwareInterfaceType == HardwareInterfaceType.Uart ? _uartParity : throw new NotSupportedException();
            internal set => _uartParity = value;
        }

        /// <summary>
        /// Gets the UART stop bits.
        /// </summary>
        public UartStopBits UartStopBits
        {
            get => HardwareInterfaceType == HardwareInterfaceType.Uart ? _uartStopBits : throw new NotSupportedException();
            internal set => _uartStopBits = value;
        }

        /// <summary>
        /// Gets the UART flow control.
        /// </summary>
        public UartFlowControl UartFlowControl
        {
            get => HardwareInterfaceType == HardwareInterfaceType.Uart ? _uartFlowControl : throw new NotSupportedException();
            internal set => _uartFlowControl = value;
        }

        /// <summary>
        /// Gets a value indicating whether the IPv4 DHCP client is enabled.
        /// </summary>
        public bool IpV4DhcpClientIsEnabled
        {
            get => new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(HardwareInterfaceType) ?
                _ipV4DhcpClientIsEnabled : throw new NotSupportedException();
            internal set => _ipV4DhcpClientIsEnabled = value;
        }

        /// <summary>
        /// Gets a value indicating whether the IPv6 DHCP client is enabled.
        /// </summary>
        public bool IpV6DhcpClientIsEnabled
        {
            get => new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(HardwareInterfaceType) ?
                _ipV6DhcpClientIsEnabled : throw new NotSupportedException();
            internal set => _ipV6DhcpClientIsEnabled = value;
        }

        /// <summary>
        /// Gets the IPv4 address.
        /// </summary>
        public IPAddress IpV4Address
        {
            get => new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(HardwareInterfaceType) ?
                _ipV4Address : throw new NotSupportedException();
            internal set => _ipV4Address = value;
        }

        /// <summary>
        /// Gets the IPv4 netmask.
        /// </summary>
        public IPAddress IpV4Netmask
        {
            get => new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(HardwareInterfaceType) ?
                _ipV4Netmask : throw new NotSupportedException();
            internal set => _ipV4Netmask = value;
        }

        /// <summary>
        /// Gets the IPv4 gateway.
        /// </summary>
        public IPAddress IpV4Gateway
        {
            get => new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(HardwareInterfaceType) ?
                _ipV4Gateway : throw new NotSupportedException();
            internal set => _ipV4Gateway = value;
        }

        /// <summary>
        /// Gets the IPv6 global address.
        /// </summary>
        public IPAddress IpV6GlobalAddress
        {
            get => new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(HardwareInterfaceType) ?
                _ipV6GlobalAddress : throw new NotSupportedException();
            internal set => _ipV6GlobalAddress = value;
        }

        /// <summary>
        /// Gets the Wi-Fi SSID.
        /// </summary>
        public string WiFiSsid
        {
            get => HardwareInterfaceType == HardwareInterfaceType.WifiStation ? _wifiSsid : throw new NotSupportedException();
            internal set => _wifiSsid = value;
        }
    }
}

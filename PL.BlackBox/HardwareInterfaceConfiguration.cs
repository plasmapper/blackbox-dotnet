using System.Net;

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox hardware interface configuration.
    /// </summary>
    public class HardwareInterfaceConfiguration
    {
        /// <summary>
        /// Gets the hardware interface type.
        /// </summary>
        public HardwareInterfaceType HardwareInterfaceType { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the hardware interface is enabled.
        /// </summary>
        public bool IsEnabled { get; internal set; }

        /// <summary>
        /// Gets the UART baud rate, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public uint? UartBaudRate { get; internal set; }

        /// <summary>
        /// Gets the UART data bits, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public ushort? UartDataBits { get; internal set; }

        /// <summary>
        /// Gets the UART parity, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public UartParity? UartParity { get; internal set; }

        /// <summary>
        /// Gets the UART stop bits, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public UartStopBits? UartStopBits { get; internal set; }

        /// <summary>
        /// Gets the UART flow control, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public UartFlowControl? UartFlowControl { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the IPv4 DHCP client is enabled, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public bool? IpV4DhcpClientIsEnabled { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the IPv6 DHCP client is enabled, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public bool? IpV6DhcpClientIsEnabled { get; internal set; }

        /// <summary>
        /// Gets the IPv4 address, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public IPAddress IpV4Address { get; internal set; }

        /// <summary>
        /// Gets the IPv4 netmask, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public IPAddress IpV4Netmask { get; internal set; }

        /// <summary>
        /// Gets the IPv4 gateway, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public IPAddress IpV4Gateway { get; internal set; }

        /// <summary>
        /// Gets the IPv6 global address, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public IPAddress IpV6GlobalAddress { get; internal set; }

        /// <summary>
        /// Gets the Wi-Fi SSID, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public string WiFiSsid { get; internal set; }
    }
}

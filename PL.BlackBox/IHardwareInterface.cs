using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox hardware interface.
    /// </summary>
    public interface IHardwareInterface
    {
        /// <summary>
        /// Gets the hardware interface index.
        /// </summary>
        ushort Index { get; }

        /// <summary>
        /// Gets the hardware interface type.
        /// </summary>
        HardwareInterfaceType Type { get; }

        /// <summary>
        /// Reads the hardware interface configuration.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        HardwareInterfaceConfiguration ReadConfiguration();

        /// <summary>
        /// Reads the hardware interface configuration asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        Task<HardwareInterfaceConfiguration> ReadConfigurationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the hardware interface state.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        HardwareInterfaceState ReadState();

        /// <summary>
        /// Reads the hardware interface state asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        Task<HardwareInterfaceState> ReadStateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables the hardware interface.
        /// </summary>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        bool Enable();

        /// <summary>
        /// Enables the hardware interface asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        Task<bool> EnableAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disables the hardware interface.
        /// </summary>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        bool Disable();

        /// <summary>
        /// Disables the hardware interface asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        Task<bool> DisableAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the UART baud rate.
        /// </summary>
        /// <param name="baudRate">Baud rate.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not Uart.</exception>
        uint SetUartBaudRate(uint baudRate);

        /// <summary>
        /// Sets the UART baud rate asynchronously.
        /// </summary>
        /// <param name="baudRate">Baud rate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not Uart.</exception>
        Task<uint> SetUartBaudRateAsync(uint baudRate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the UART data bits.
        /// </summary>
        /// <param name="dataBits">Data bits.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not Uart.</exception>
        ushort SetUartDataBits(ushort dataBits);

        /// <summary>
        /// Sets the UART data bits asynchronously.
        /// </summary>
        /// <param name="dataBits">Data bits.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not Uart.</exception>
        Task<ushort> SetUartDataBitsAsync(ushort dataBits, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the UART parity.
        /// </summary>
        /// <param name="parity">Parity.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not Uart.</exception>
        UartParity SetUartParity(UartParity parity);

        /// <summary>
        /// Sets the UART parity asynchronously.
        /// </summary>
        /// <param name="parity">Parity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not Uart.</exception>
        Task<UartParity> SetUartParityAsync(UartParity parity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the UART stop bits.
        /// </summary>
        /// <param name="stopBits">Stop bits.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not Uart.</exception>
        UartStopBits SetUartStopBits(UartStopBits stopBits);

        /// <summary>
        /// Sets the UART stop bits asynchronously.
        /// </summary>
        /// <param name="stopBits">Stop bits.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not Uart.</exception>
        Task<UartStopBits> SetUartStopBitsAsync(UartStopBits stopBits, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the UART flow control.
        /// </summary>
        /// <param name="flowControl">Flow control.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not Uart.</exception>
        UartFlowControl SetUartFlowControl(UartFlowControl flowControl);

        /// <summary>
        /// Sets the UART flow control asynchronously.
        /// </summary>
        /// <param name="flowControl">Flow control.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not Uart.</exception>
        Task<UartFlowControl> SetUartFlowControlAsync(UartFlowControl flowControl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables the IPv4 DHCP client.
        /// </summary>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        bool EnableIpV4DhcpClient();

        /// <summary>
        /// Enables the IPv4 DHCP client asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        Task<bool> EnableIpV4DhcpClientAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disables the IPv4 DHCP client.
        /// </summary>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        bool DisableIpV4DhcpClient();

        /// <summary>
        /// Disables the IPv4 DHCP client asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        Task<bool> DisableIpV4DhcpClientAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables the IPv6 DHCP client.
        /// </summary>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        bool EnableIpV6DhcpClient();

        /// <summary>
        /// Enables the IPv6 DHCP client asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        Task<bool> EnableIpV6DhcpClientAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disables the IPv6 DHCP client.
        /// </summary>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        bool DisableIpV6DhcpClient();

        /// <summary>
        /// Disables the IPv6 DHCP client asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        Task<bool> DisableIpV6DhcpClientAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the IPv4 address.
        /// </summary>
        /// <param name="ipV4Address">IPv4 address.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        IPAddress SetIpV4Address(IPAddress ipV4Address);

        /// <summary>
        /// Sets the IPv4 address asynchronously.
        /// </summary>
        /// <param name="ipV4Address">IPv4 address.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        Task<IPAddress> SetIpV4AddressAsync(IPAddress ipV4Address, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the IPv4 netmask.
        /// </summary>
        /// <param name="ipV4Netmask">IPv4 netmask.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        IPAddress SetIpV4Netmask(IPAddress ipV4Netmask);

        /// <summary>
        /// Sets the IPv4 netmask asynchronously.
        /// </summary>
        /// <param name="ipV4Netmask">IPv4 netmask.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        Task<IPAddress> SetIpV4NetmaskAsync(IPAddress ipV4Netmask, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the IPv4 gateway.
        /// </summary>
        /// <param name="ipV4Gateway">IPv4 gateway.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        IPAddress SetIpV4Gateway(IPAddress ipV4Gateway);

        /// <summary>
        /// Sets the IPv4 gateway asynchronously.
        /// </summary>
        /// <param name="ipV4Gateway">IPv4 gateway.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        Task<IPAddress> SetIpV4GatewayAsync(IPAddress ipV4Gateway, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the IPv6 global address.
        /// </summary>
        /// <param name="ipV6GlobalAddress">IPv6 global address.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        IPAddress SetIpV6GlobalAddress(IPAddress ipV6GlobalAddress);

        /// <summary>
        /// Sets the IPv6 global address asynchronously.
        /// </summary>
        /// <param name="ipV6GlobalAddress">IPv6 global address.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkInterface, Ethernet or WifiStation.</exception>
        Task<IPAddress> SetIpV6GlobalAddressAsync(IPAddress ipV6GlobalAddress, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the Wi-Fi SSID.
        /// </summary>
        /// <param name="ssid">Wi-Fi station SSID.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not WifiStation.</exception>
        string SetWiFiSsid(string ssid);

        /// <summary>
        /// Sets the Wi-Fi SSID asynchronously.
        /// </summary>
        /// <param name="ssid">Wi-Fi station SSID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not WifiStation.</exception>
        Task<string> SetWiFiSsidAsync(string ssid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the Wi-Fi password.
        /// </summary>
        /// <param name="password">Wi-Fi station password.</param>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not WifiStation.</exception>
        void SetWiFiPassword(string password);

        /// <summary>
        /// Sets the Wi-Fi password asynchronously.
        /// </summary>
        /// <param name="password">Wi-Fi station password.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Hardware interface control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not WifiStation.</exception>
        Task SetWiFiPasswordAsync(string password, CancellationToken cancellationToken = default);
    }
}

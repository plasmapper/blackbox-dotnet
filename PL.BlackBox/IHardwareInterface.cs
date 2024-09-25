using System.Net;

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox hardware interface.
    /// </summary>
    public interface IHardwareInterface
    {
        /// <summary>
        /// Gets the hardware interface type.
        /// </summary>
        HardwareInterfaceType Type { get; }

        /// <summary>
        /// Reads the hardware interface configuration.
        /// </summary>
        /// <returns></returns>
        HardwareInterfaceConfiguration ReadConfiguration();

        /// <summary>
        /// Reads the hardware interface state.
        /// </summary>
        /// <returns></returns>
        HardwareInterfaceState ReadState();

        /// <summary>
        /// Enables the hardware interface.
        /// </summary>
        /// <returns>Set state.</returns>
        bool Enable();

        /// <summary>
        /// Disables the hardware interface.
        /// </summary>
        /// <returns>Set state.</returns>
        bool Disable();

        /// <summary>
        /// Sets the UART baud rate.
        /// </summary>
        /// <param name="baudRate">Baud rate.</param>
        /// <returns>Set value.</returns>
        uint SetUartBaudRate(uint baudRate);

        /// <summary>
        /// Sets the UART data bits.
        /// </summary>
        /// <param name="dataBits">Data bits.</param>
        /// <returns>Set value.</returns>
        ushort SetUartDataBits(ushort dataBits);

        /// <summary>
        /// Sets the UART parity.
        /// </summary>
        /// <param name="parity">Parity.</param>
        /// <returns>Set value.</returns>
        UartParity SetUartParity(UartParity parity);

        /// <summary>
        /// Sets the UART stop bits.
        /// </summary>
        /// <param name="stopBits">Stop bits.</param>
        /// <returns>Set value.</returns>
        UartStopBits SetUartStopBits(UartStopBits stopBits);

        /// <summary>
        /// Sets the UART flow control.
        /// </summary>
        /// <param name="flowControl">Flow control.</param>
        /// <returns>Set value.</returns>
        UartFlowControl SetUartFlowControl(UartFlowControl flowControl);

        /// <summary>
        /// Enables the IPv4 DHCP client.
        /// </summary>
        /// <returns>Set state.</returns>
        bool EnableIpV4DhcpClient();

        /// <summary>
        /// Disables the IPv4 DHCP client.
        /// </summary>
        /// <returns>Set state.</returns>
        bool DisableIpV4DhcpClient();

        /// <summary>
        /// Enables the IPv6 DHCP client.
        /// </summary>
        /// <returns>Set state.</returns>
        bool EnableIpV6DhcpClient();

        /// <summary>
        /// Disables the IPv6 DHCP client.
        /// </summary>
        /// <returns>Set state.</returns>
        bool DisableIpV6DhcpClient();

        /// <summary>
        /// Sets the IPv4 address.
        /// </summary>
        /// <param name="ipV4Address">IPv4 address.</param>
        /// <returns>Set value.</returns>
        IPAddress SetIpV4Address(IPAddress ipV4Address);

        /// <summary>
        /// Sets the IPv4 netmask.
        /// </summary>
        /// <param name="ipV4Netmask">IPv4 netmask.</param>
        /// <returns>Set value.</returns>
        IPAddress SetIpV4Netmask(IPAddress ipV4Netmask);

        /// <summary>
        /// Sets the IPv4 gateway.
        /// </summary>
        /// <param name="ipV4Gateway">IPv4 gateway.</param>
        /// <returns>Set value.</returns>
        IPAddress SetIpV4Gateway(IPAddress ipV4Gateway);

        /// <summary>
        /// Sets the IPv6 global address.
        /// </summary>
        /// <param name="ipV6GlobalAddress">IPv6 global address.</param>
        /// <returns>Set value.</returns>
        IPAddress SetIpV6GlobalAddress(IPAddress ipV6GlobalAddress);

        /// <summary>
        /// Sets the Wi-Fi SSID.
        /// </summary>
        /// <param name="ssid">Wi-Fi station SSID.</param>
        /// <returns>Set value.</returns>
        string SetWiFiSsid(string ssid);

        /// <summary>
        /// Sets the Wi-Fi password.
        /// </summary>
        /// <param name="password">Wi-Fi station password.</param>
        void SetWiFiPassword(string password);
    }
}

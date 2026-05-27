namespace PL.BlackBox
{
    /// <summary>
    /// Hardware interface type.
    /// </summary>
    public enum HardwareInterfaceType : byte
    {
        /// <summary>
        /// Unknown hardware interface.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// UART.
        /// </summary>
        Uart = 1,

        /// <summary>
        /// Network interface.
        /// </summary>
        NetworkInterface = 2,

        /// <summary>
        /// Ethernet.
        /// </summary>
        Ethernet = 3,

        /// <summary>
        /// Wi-Fi station.
        /// </summary>
        WifiStation = 4,

        /// <summary>
        /// USB CDC.
        /// </summary>
        UsbDeviceCdc = 5
    }
}

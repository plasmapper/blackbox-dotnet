namespace PL.BlackBox
{
    /// <summary>
    /// Hardware interface type.
    /// </summary>
    public enum HardwareInterfaceType : byte
    {
        Unknown = 0,
        Uart = 1,
        NetworkInterface = 2,
        Ethernet = 3,
        WifiStation = 4,
        UsbDeviceCdc = 5
    }
}

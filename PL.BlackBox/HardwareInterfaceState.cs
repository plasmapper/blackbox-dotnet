using System.Net;

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox hardware interface state.
    /// </summary>
    public class HardwareInterfaceState
    {
        /// <summary>
        /// Gets the hardware interface type.
        /// </summary>
        public HardwareInterfaceType HardwareInterfaceType { get; internal set; }

        /// <summary>
        /// Gets the hardware interface name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the hardware interface is connected, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public bool? IsConnected { get; internal set; }

        /// <summary>
        /// Gets the IPv6 local address, or null if not applicable to <see cref="HardwareInterfaceType"/>.
        /// </summary>
        public IPAddress IpV6LocalAddress { get; internal set; }
    }
}

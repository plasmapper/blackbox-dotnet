using System;
using System.Linq;
using System.Net;

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox hardware interface state.
    /// </summary>
    public class HardwareInterfaceState
    {
        private string _name;
        private bool _isConnected;
        private IPAddress _ipV6LocalAddress;

        /// <summary>
        /// Gets the hardware interface type.
        /// </summary>
        public HardwareInterfaceType HardwareInterfaceType { get; internal set; }

        /// <summary>
        /// Gets the hardware interface name.
        /// </summary>
        public string Name
        {
            get => _name;
            internal set => _name = value;
        }

        /// <summary>
        /// Gets a value indicating whether the hardware interface is connected.
        /// </summary>
        public bool IsConnected
        {
            get => new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(HardwareInterfaceType) ?
                _isConnected : throw new NotSupportedException();
            internal set => _isConnected = value;
        }

        /// <summary>
        /// Gets the IPv6 local address.
        /// </summary>
        public IPAddress IpV6LocalAddress
        {
            get => new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(HardwareInterfaceType) ?
                _ipV6LocalAddress : throw new NotSupportedException();
            internal set => _ipV6LocalAddress = value;
        }
    }
}

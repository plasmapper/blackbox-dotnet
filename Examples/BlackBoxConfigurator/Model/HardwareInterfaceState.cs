using System.Net;

namespace BlackBoxConfigurator.Model
{
    internal class HardwareInterfaceState
    {
        public HardwareInterfaceState() { }

        public Parameter<bool> IsConnected { get; } = new(false);
        public Parameter<IPAddress> IpV6LocalAddress { get; } = new(new(0));
    }
}
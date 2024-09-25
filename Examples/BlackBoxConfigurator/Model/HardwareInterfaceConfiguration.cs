using System.Net;

namespace BlackBoxConfigurator.Model
{
    internal class HardwareInterfaceConfiguration
    {
        private readonly PL.BlackBox.IClient _client;
        private readonly Parameter<int> _hardwareInterfaceIndex;

        public HardwareInterfaceConfiguration(PL.BlackBox.IClient client, Parameter<int> hardwareInterfaceIndex)
        {
            _client = client;
            _hardwareInterfaceIndex = hardwareInterfaceIndex;

            Enabled = new(false, value => value ? HardwareInterface.Enable() : HardwareInterface.Disable());
            UartBaudRate = new(0, value => HardwareInterface.SetUartBaudRate(value));
            UartDataBits = new(0, value => HardwareInterface.SetUartDataBits(value));
            UartParity = new(PL.BlackBox.UartParity.None, value => HardwareInterface.SetUartParity(value));
            UartStopBits = new(PL.BlackBox.UartStopBits.One, value => HardwareInterface.SetUartStopBits(value));
            UartFlowControl = new(PL.BlackBox.UartFlowControl.None, value => HardwareInterface.SetUartFlowControl(value));
            
            IpV4DhcpClientEnabled = new(false, value => value ? HardwareInterface.EnableIpV4DhcpClient() : HardwareInterface.DisableIpV4DhcpClient());
            IpV6DhcpClientEnabled = new(false, value => value ? HardwareInterface.EnableIpV6DhcpClient() : HardwareInterface.DisableIpV6DhcpClient());
            IpV4Address = new(new(0), value => HardwareInterface.SetIpV4Address(value));
            IpV4Netmask = new(new(0), value => HardwareInterface.SetIpV4Netmask(value));
            IpV4Gateway = new(new(0), value => HardwareInterface.SetIpV4Gateway(value));
            IpV6GlobalAddress = new(new(0), value => HardwareInterface.SetIpV6GlobalAddress(value));
            WiFiSsid = new("", value => HardwareInterface.SetWiFiSsid(value));
            WiFiPassword = new("", value =>
            {
                HardwareInterface.SetWiFiPassword(value);
                return "";
            });
        }

        public Parameter<bool> Enabled { get; }
        public Parameter<uint> UartBaudRate { get; }
        public Parameter<ushort> UartDataBits { get; }
        public Parameter<PL.BlackBox.UartParity> UartParity { get; }
        public Parameter<PL.BlackBox.UartStopBits> UartStopBits { get; }
        public Parameter<PL.BlackBox.UartFlowControl> UartFlowControl { get; }
        
        public Parameter<bool> IpV4DhcpClientEnabled { get; }
        public Parameter<bool> IpV6DhcpClientEnabled { get; }
        public Parameter<IPAddress> IpV4Address { get; }
        public Parameter<IPAddress> IpV4Netmask { get; }
        public Parameter<IPAddress> IpV4Gateway { get; }
        public Parameter<IPAddress> IpV6GlobalAddress { get; }
        public Parameter<string> WiFiSsid { get; }
        public Parameter<string> WiFiPassword { get; }

        private PL.BlackBox.IHardwareInterface HardwareInterface { get => _client.GetHardwareInterface((ushort)_hardwareInterfaceIndex.Value); }
    }
}
using System.Net;
using System.Windows.Input;

namespace BlackBoxConfigurator.ViewModel
{
    internal class HardwareInterfaceConfiguration
    {
        public HardwareInterfaceConfiguration(Model.HardwareInterfaceConfiguration configuration)
        {
            EnabledState = new Parameter<State, bool>(configuration.Enabled, value => value ? State.On : State.Off);
            ToggleEnabledState = new Command(() => configuration.Enabled.SetValue(!configuration.Enabled.Value));
            UartBaudRate = new(configuration.UartBaudRate);
            UartDataBits = new(configuration.UartDataBits);
            UartParity = new(configuration.UartParity);
            UartStopBits = new(configuration.UartStopBits);
            UartFlowControl = new(configuration.UartFlowControl);

            IpV4DhcpClientState = new Parameter<State, bool>(configuration.IpV4DhcpClientEnabled, value => value ? State.On : State.Off);
            ToggleIpV4DhcpClientState = new Command(() => configuration.IpV4DhcpClientEnabled.SetValue(!configuration.IpV4DhcpClientEnabled.Value));
            IpV6DhcpClientState = new Parameter<State, bool>(configuration.IpV6DhcpClientEnabled, value => value ? State.On : State.Off);
            ToggleIpV6DhcpClientState = new Command(() => configuration.IpV6DhcpClientEnabled.SetValue(!configuration.IpV6DhcpClientEnabled.Value));
            IpV4Address = new(configuration.IpV4Address);
            IpV4Netmask = new(configuration.IpV4Netmask);
            IpV4Gateway = new(configuration.IpV4Gateway);
            IpV6GlobalAddress = new(configuration.IpV6GlobalAddress);
            WiFiSsid = new(configuration.WiFiSsid);
            WiFiPassword = new(configuration.WiFiPassword);
        }

        public Parameter<State> EnabledState { get; }
        public ICommand ToggleEnabledState { get; }

        public Parameter<uint> UartBaudRate { get; }
        public Parameter<ushort> UartDataBits { get; }
        public List<PL.BlackBox.UartParity> UartParityValues { get; } = new() { PL.BlackBox.UartParity.None, PL.BlackBox.UartParity.Even, PL.BlackBox.UartParity.Odd };
        public Parameter<PL.BlackBox.UartParity> UartParity { get; }
        public List<PL.BlackBox.UartStopBits> UartStopBitsValues { get; } = new() { PL.BlackBox.UartStopBits.One, PL.BlackBox.UartStopBits.OnePointFive, PL.BlackBox.UartStopBits.Two };
        public Parameter<PL.BlackBox.UartStopBits> UartStopBits { get; }
        public List<PL.BlackBox.UartFlowControl> UartFlowControlValues { get; } = new() { PL.BlackBox.UartFlowControl.None, PL.BlackBox.UartFlowControl.Rts, PL.BlackBox.UartFlowControl.Cts, PL.BlackBox.UartFlowControl.RtsCts };
        public Parameter<PL.BlackBox.UartFlowControl> UartFlowControl { get; }

        public Parameter<State> IpV4DhcpClientState { get; }
        public ICommand ToggleIpV4DhcpClientState { get; }
        public Parameter<State> IpV6DhcpClientState { get; }
        public ICommand ToggleIpV6DhcpClientState { get; }
        public Parameter<IPAddress> IpV4Address { get; }
        public Parameter<IPAddress> IpV4Netmask { get; }
        public Parameter<IPAddress> IpV4Gateway { get; }
        public Parameter<IPAddress> IpV6GlobalAddress { get; }
        public Parameter<string> WiFiSsid { get; }
        public Parameter<string> WiFiPassword { get; }
    }
}
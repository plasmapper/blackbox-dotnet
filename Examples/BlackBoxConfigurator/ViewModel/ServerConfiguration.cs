using System.Windows.Input;

namespace BlackBoxConfigurator.ViewModel
{
    internal class ServerConfiguration
    {
        public ServerConfiguration(Model.ServerConfiguration configuration)
        {
            EnabledState = new Parameter<State, bool>(configuration.Enabled, value => value ? State.On : State.Off);
            ToggleEnabledState = new Command(() => configuration.Enabled.SetValue(!configuration.Enabled.Value));
            NetworkPort = new(configuration.NetworkPort);
            MaxNumberOfClients = new(configuration.MaxNumberOfClients);
            ModbusProtocol = new(configuration.ModbusProtocol);
            ModbusStationAddress = new(configuration.ModbusStationAddress);
        }

        public Parameter<State> EnabledState { get; }
        public ICommand ToggleEnabledState { get; }
        public Parameter<ushort> NetworkPort { get; }
        public Parameter<ushort> MaxNumberOfClients { get; }
        public List<PL.BlackBox.ModbusProtocol> ModbusProtocolValues { get; } = new() { PL.BlackBox.ModbusProtocol.Rtu, PL.BlackBox.ModbusProtocol.Ascii, PL.BlackBox.ModbusProtocol.Tcp };
        public Parameter<PL.BlackBox.ModbusProtocol> ModbusProtocol { get; }
        public Parameter<byte> ModbusStationAddress { get; }
    }
}
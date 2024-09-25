namespace BlackBoxConfigurator.Model
{
    /// <summary>
    /// System model class.
    /// Properties are parameter model objects that implement setting logic.
    /// Methods implement some other logic apart from parameter setting.
    /// </summary>
    internal class SystemModel
    {
        private readonly PL.BlackBox.ModbusClient _client;
        private System.IO.Ports.SerialPort? _port;

        public SystemModel(PL.BlackBox.ModbusClient client)
        {
            _client = client;

            DeviceName = new("", value => _client.SetDeviceName(value));

            HardwareInterfaceConfiguration = new(client, HardwareInterfaceIndex);
            ServerConfiguration = new(client, ServerIndex);

            List<Parameter> parameters = new() { Connection.Interface,
                Connection.SerialPortName, Connection.SerialBaudRate, Connection.SerialDataBits, Connection.SerialParity, Connection.SerialStopBits, Connection.SerialFlowControl,
                Connection.SerialProtocol, Connection.SerialStationAddress,
                Connection.NetworkAddress, Connection.NetworkPort, Connection.NetworkStationAddress };
            foreach (var parameter in parameters)
                parameter.ValueChanged += (s, e) => ChangeStream();

            ChangeStream();
        }

        public Connection Connection { get; } = new();
        public Parameter<Exception?> Exception { get; } = new(null);

        public Parameter<bool> Restarted { get; } = new(false);
        public Parameter<string> HardwareNameAndVersion { get; } = new("");
        public Parameter<string> HardwareUid { get; } = new("");
        public Parameter<string> FirmwareNameAndVersion { get; } = new("");
        public Parameter<string> DeviceName { get; }

        public Parameter<List<string>> HardwareInterfaceNames { get; } = new(new());
        public Parameter<PL.BlackBox.HardwareInterfaceType> HardwareInterfaceType { get; } = new(PL.BlackBox.HardwareInterfaceType.Unknown);
        public Parameter<int> HardwareInterfaceIndex { get; } = new(0);
        public HardwareInterfaceConfiguration HardwareInterfaceConfiguration { get; }
        public HardwareInterfaceState HardwareInterfaceState { get; } = new();

        public Parameter<List<string>> ServerNames { get; } = new(new());
        public Parameter<PL.BlackBox.ServerType> ServerType { get; } = new(PL.BlackBox.ServerType.Unknown);
        public Parameter<int> ServerIndex { get; } = new(0);
        public ServerConfiguration ServerConfiguration { get; }

        public void Restart() => _client.Restart();
        public void ClearRestartedFlag() => _client.ClearRestartedFlag();
        public void SaveAllConfigurations() => _client.SaveAllConfigurations();

        private void ChangeStream()
        {
            lock(_client)
            {
                if (Connection.Interface.Value == PL.Modbus.Interface.Serial)
                {
                    _port?.Close();
                    _port = new System.IO.Ports.SerialPort();
                    if (Connection.SerialPortName.Value is not null && Connection.SerialPortName.Value != "")
                        _port.PortName = Connection.SerialPortName.Value;
                    _port.BaudRate = Connection.SerialBaudRate.Value;
                    _port.DataBits = Connection.SerialDataBits.Value;
                    _port.Parity = Connection.SerialParity.Value;
                    _port.StopBits = Connection.SerialStopBits.Value;
                    _port.Handshake = Connection.SerialFlowControl.Value;

                    _client.Stream = new PL.Modbus.SerialStream(_port);
                    _client.Protocol = Connection.SerialProtocol.Value;
                    _client.StationAddress = Connection.SerialStationAddress.Value;
                }
                else
                {
                    _client.Stream.Close();
                    _client.Stream = new PL.Modbus.NetworkStream(Connection.NetworkAddress.Value, Connection.NetworkPort.Value);
                    _client.Protocol = PL.Modbus.Protocol.Tcp;
                    _client.StationAddress = Connection.NetworkStationAddress.Value;
                }
            }
        }
    }
}
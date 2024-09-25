namespace BlackBoxConfigurator.Model
{
    internal class ServerConfiguration
    {
        private readonly PL.BlackBox.IClient _client;
        private readonly Parameter<int> _serverIndex;

        public ServerConfiguration(PL.BlackBox.IClient client, Parameter<int> serverIndex)
        {
            _client = client;
            _serverIndex = serverIndex;

            Enabled = new(false, value => value ? Server.Enable() : Server.Disable());
            NetworkPort = new(0, value => Server.SetPort(value));
            MaxNumberOfClients = new(0, value => Server.SetMaxNumberOfClients(value));
            ModbusProtocol = new(PL.BlackBox.ModbusProtocol.Rtu, value => Server.SetModbusProtocol(value));
            ModbusStationAddress = new(0, value => Server.SetModbusStationAddress(value));
        }

        public Parameter<bool> Enabled { get; }
        public Parameter<ushort> NetworkPort { get; }
        public Parameter<ushort> MaxNumberOfClients { get; }
        public Parameter<PL.BlackBox.ModbusProtocol> ModbusProtocol { get; }
        public Parameter<byte> ModbusStationAddress { get; }

        private PL.BlackBox.IServer Server { get => _client.GetServer((ushort)_serverIndex.Value); }
    }
}
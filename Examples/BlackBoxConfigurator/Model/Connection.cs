namespace BlackBoxConfigurator.Model
{
    internal class Connection
    {
        public Connection() { }

        public Parameter<PL.Modbus.Interface> Interface { get; } = new(PL.Modbus.Interface.Serial);

        public Parameter<List<string>> SerialPortNames { get; } = new(new());
        public Parameter<string> SerialPortName { get; } = new("");
        public Parameter<int> SerialBaudRate { get; } = new(115200);
        public Parameter<int> SerialDataBits { get; } = new(8);
        public Parameter<System.IO.Ports.Parity> SerialParity { get; } = new(System.IO.Ports.Parity.Even);
        public Parameter<System.IO.Ports.StopBits> SerialStopBits { get; } = new(System.IO.Ports.StopBits.One);
        public Parameter<System.IO.Ports.Handshake> SerialFlowControl { get; } = new(System.IO.Ports.Handshake.None);
        public Parameter<PL.Modbus.Protocol> SerialProtocol { get; } = new(PL.Modbus.Protocol.Rtu);
        public Parameter<byte> SerialStationAddress { get; } = new(1);

        public Parameter<string> NetworkAddress { get; } = new("192.168.1.1");
        public Parameter<ushort> NetworkPort { get; } = new(502);
        public Parameter<byte> NetworkStationAddress { get; } = new(255);
    }
}
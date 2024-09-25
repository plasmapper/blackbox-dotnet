using System;
using System.Linq;

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox server configuration.
    /// </summary>
    public class ServerConfiguration
    {
        private bool _isEnabled;
        private ushort _port;
        private ushort _maxNumberOfClients;
        private ModbusProtocol _modbusProtocol;
        private byte _modbusStationAddress;

        /// <summary>
        /// Gets the server type.
        /// </summary>
        public ServerType ServerType { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the server is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            internal set => _isEnabled = value;
        }

        /// <summary>
        /// Gets the network server port.
        /// </summary>
        public ushort NetworkPort
        {
            get => new[] { ServerType.NetworkServer, ServerType.NetworkModbusServer, ServerType.HttpServer, ServerType.MdnsServer }.Contains(ServerType) ?
                _port : throw new NotSupportedException();
            internal set => _port = value;
        }

        /// <summary>
        /// Gets the network server maximum number of clients.
        /// </summary>
        public ushort MaxNumberOfClients
        {
            get => new[] { ServerType.NetworkServer, ServerType.NetworkModbusServer, ServerType.HttpServer, ServerType.MdnsServer }.Contains(ServerType) ?
                _maxNumberOfClients : throw new NotSupportedException();
            internal set => _maxNumberOfClients = value;
        }

        /// <summary>
        /// Gets the Modbus protocol.
        /// </summary>
        public ModbusProtocol ModbusProtocol
        {
            get => new[] { ServerType.StreamModbusServer, ServerType.NetworkModbusServer }.Contains(ServerType) ?
                _modbusProtocol : throw new NotSupportedException();
            internal set => _modbusProtocol = value;
        }

        /// <summary>
        /// Gets the Modbus station address.
        /// </summary>
        public byte ModbusStationAddress
        {
            get => new[] { ServerType.StreamModbusServer, ServerType.NetworkModbusServer }.Contains(ServerType) ?
                _modbusStationAddress : throw new NotSupportedException();
            internal set => _modbusStationAddress = value;
        }
    }
}

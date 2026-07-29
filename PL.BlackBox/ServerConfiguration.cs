namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox server configuration.
    /// </summary>
    public class ServerConfiguration
    {
        /// <summary>
        /// Gets the server type.
        /// </summary>
        public ServerType ServerType { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the server is enabled.
        /// </summary>
        public bool IsEnabled { get; internal set; }

        /// <summary>
        /// Gets the network server port, or null if not applicable to <see cref="ServerType"/>.
        /// </summary>
        public ushort? NetworkPort { get; internal set; }

        /// <summary>
        /// Gets the network server maximum number of clients, or null if not applicable to <see cref="ServerType"/>.
        /// </summary>
        public ushort? MaxNumberOfClients { get; internal set; }

        /// <summary>
        /// Gets the Modbus protocol, or null if not applicable to <see cref="ServerType"/>.
        /// </summary>
        public ModbusProtocol? ModbusProtocol { get; internal set; }

        /// <summary>
        /// Gets the Modbus station address, or null if not applicable to <see cref="ServerType"/>.
        /// </summary>
        public byte? ModbusStationAddress { get; internal set; }
    }
}

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox server.
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Gets the server type.
        /// </summary>
        ServerType Type { get; }

        /// <summary>
        /// Reads the server configuration.
        /// </summary>
        /// <returns></returns>
        ServerConfiguration ReadConfiguration();

        /// <summary>
        /// Reads the server state.
        /// </summary>
        /// <returns></returns>
        ServerState ReadState();

        /// <summary>
        /// Enables the server.
        /// </summary>
        /// <returns>Set state.</returns>
        bool Enable();

        /// <summary>
        /// Disables the server.
        /// </summary>
        /// <returns>Set state.</returns>
        bool Disable();

        /// <summary>
        /// Sets the network server port.
        /// </summary>
        /// <param name="port">Network server port.</param>
        /// <returns>Set value.</returns>
        ushort SetPort(ushort port);

        /// <summary>
        /// Sets the network server maximum number of clients.
        /// </summary>
        /// <param name="port">Network server maximum number of clients.</param>
        /// <returns>Set value.</returns>
        ushort SetMaxNumberOfClients(ushort maxNumberOfClients);

        /// <summary>
        /// Sets the Modbus protocol.
        /// </summary>
        /// <param name="protocol">Modbus protocol.</param>
        /// <returns>Set value</returns>
        ModbusProtocol SetModbusProtocol(ModbusProtocol protocol);

        /// <summary>
        /// Sets the Modbus station address.
        /// </summary>
        /// <param name="stationAddress">Modbus station address.</param>
        /// <returns>Set value</returns>
        byte SetModbusStationAddress(byte stationAddress);
    }
}

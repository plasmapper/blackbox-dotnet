using System;
using System.Threading;
using System.Threading.Tasks;

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox server.
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Gets the server index.
        /// </summary>
        ushort Index { get; }

        /// <summary>
        /// Gets the server type.
        /// </summary>
        ServerType Type { get; }

        /// <summary>
        /// Reads the server configuration.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        ServerConfiguration ReadConfiguration();

        /// <summary>
        /// Reads the server configuration asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        Task<ServerConfiguration> ReadConfigurationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the server state.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        ServerState ReadState();

        /// <summary>
        /// Reads the server state asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        Task<ServerState> ReadStateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables the server.
        /// </summary>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        bool Enable();

        /// <summary>
        /// Enables the server asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        Task<bool> EnableAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disables the server.
        /// </summary>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        bool Disable();

        /// <summary>
        /// Disables the server asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        Task<bool> DisableAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the network server port.
        /// </summary>
        /// <param name="port">Network server port.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkServer, NetworkModbusServer, HttpServer or MdnsServer.</exception>
        ushort SetPort(ushort port);

        /// <summary>
        /// Sets the network server port asynchronously.
        /// </summary>
        /// <param name="port">Network server port.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkServer, NetworkModbusServer, HttpServer or MdnsServer.</exception>
        Task<ushort> SetPortAsync(ushort port, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the network server maximum number of clients.
        /// </summary>
        /// <param name="maxNumberOfClients">Network server maximum number of clients.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkServer, NetworkModbusServer, HttpServer or MdnsServer.</exception>
        ushort SetMaxNumberOfClients(ushort maxNumberOfClients);

        /// <summary>
        /// Sets the network server maximum number of clients asynchronously.
        /// </summary>
        /// <param name="maxNumberOfClients">Network server maximum number of clients.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not NetworkServer, NetworkModbusServer, HttpServer or MdnsServer.</exception>
        Task<ushort> SetMaxNumberOfClientsAsync(ushort maxNumberOfClients, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the Modbus protocol.
        /// </summary>
        /// <param name="protocol">Modbus protocol.</param>
        /// <returns>Set value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not StreamModbusServer or NetworkModbusServer.</exception>
        ModbusProtocol SetModbusProtocol(ModbusProtocol protocol);

        /// <summary>
        /// Sets the Modbus protocol asynchronously.
        /// </summary>
        /// <param name="protocol">Modbus protocol.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not StreamModbusServer or NetworkModbusServer.</exception>
        Task<ModbusProtocol> SetModbusProtocolAsync(ModbusProtocol protocol, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the Modbus station address.
        /// </summary>
        /// <param name="stationAddress">Modbus station address.</param>
        /// <returns>Set value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not StreamModbusServer or NetworkModbusServer.</exception>
        byte SetModbusStationAddress(byte stationAddress);

        /// <summary>
        /// Sets the Modbus station address asynchronously.
        /// </summary>
        /// <param name="stationAddress">Modbus station address.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        /// <exception cref="Exception">Server control instance is invalid.</exception>
        /// <exception cref="NotSupportedException">Type is not StreamModbusServer or NetworkModbusServer.</exception>
        Task<byte> SetModbusStationAddressAsync(byte stationAddress, CancellationToken cancellationToken = default);
    }
}

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
        ServerConfiguration ReadConfiguration();

        /// <summary>
        /// Reads the server configuration asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<ServerConfiguration> ReadConfigurationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the server state.
        /// </summary>
        /// <returns></returns>
        ServerState ReadState();

        /// <summary>
        /// Reads the server state asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<ServerState> ReadStateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables the server.
        /// </summary>
        /// <returns>Set state.</returns>
        bool Enable();

        /// <summary>
        /// Enables the server asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set state.</returns>
        Task<bool> EnableAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disables the server.
        /// </summary>
        /// <returns>Set state.</returns>
        bool Disable();

        /// <summary>
        /// Disables the server asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set state.</returns>
        Task<bool> DisableAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the network server port.
        /// </summary>
        /// <param name="port">Network server port.</param>
        /// <returns>Set value.</returns>
        ushort SetPort(ushort port);

        /// <summary>
        /// Sets the network server port asynchronously.
        /// </summary>
        /// <param name="port">Network server port.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        Task<ushort> SetPortAsync(ushort port, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the network server maximum number of clients.
        /// </summary>
        /// <param name="maxNumberOfClients">Network server maximum number of clients.</param>
        /// <returns>Set value.</returns>
        ushort SetMaxNumberOfClients(ushort maxNumberOfClients);

        /// <summary>
        /// Sets the network server maximum number of clients asynchronously.
        /// </summary>
        /// <param name="maxNumberOfClients">Network server maximum number of clients.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        Task<ushort> SetMaxNumberOfClientsAsync(ushort maxNumberOfClients, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the Modbus protocol.
        /// </summary>
        /// <param name="protocol">Modbus protocol.</param>
        /// <returns>Set value</returns>
        ModbusProtocol SetModbusProtocol(ModbusProtocol protocol);

        /// <summary>
        /// Sets the Modbus protocol asynchronously.
        /// </summary>
        /// <param name="protocol">Modbus protocol.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value</returns>
        Task<ModbusProtocol> SetModbusProtocolAsync(ModbusProtocol protocol, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the Modbus station address.
        /// </summary>
        /// <param name="stationAddress">Modbus station address.</param>
        /// <returns>Set value</returns>
        byte SetModbusStationAddress(byte stationAddress);

        /// <summary>
        /// Sets the Modbus station address asynchronously.
        /// </summary>
        /// <param name="stationAddress">Modbus station address.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value</returns>
        Task<byte> SetModbusStationAddressAsync(byte stationAddress, CancellationToken cancellationToken = default);
    }
}

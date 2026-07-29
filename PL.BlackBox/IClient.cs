using System;
using System.Threading;
using System.Threading.Tasks;

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox client.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Reads the device configuration.
        /// </summary>
        /// <returns>Device configuration.</returns>
        DeviceConfiguration ReadDeviceConfiguration();

        /// <summary>
        /// Reads the device configuration asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Device configuration.</returns>
        Task<DeviceConfiguration> ReadDeviceConfigurationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the device state.
        /// </summary>
        /// <returns>Device state.</returns>
        DeviceState ReadDeviceState();

        /// <summary>
        /// Reads the device state asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Device state.</returns>
        Task<DeviceState> ReadDeviceStateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Restarts the device.
        /// </summary>
        void Restart();

        /// <summary>
        /// Restarts the device asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task RestartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves the device configuration to non-volatile memory.
        /// </summary>
        void SaveAllConfigurations();

        /// <summary>
        /// Saves the device configuration to non-volatile memory asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SaveAllConfigurationsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Clears the device restarted flag.
        /// </summary>
        void ClearRestartedFlag();

        /// <summary>
        /// Clears the device restarted flag asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ClearRestartedFlagAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the device name.
        /// </summary>
        /// <param name="deviceName">Device name.</param>
        /// <returns>Set value.</returns>
        string SetDeviceName(string deviceName);

        /// <summary>
        /// Sets the device name asynchronously.
        /// </summary>
        /// <param name="deviceName">Device name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set value.</returns>
        Task<string> SetDeviceNameAsync(string deviceName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the hardware interface control instance.
        /// </summary>
        /// <param name="index">Hardware interface index</param>
        /// <returns>Hardware interface control instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        IHardwareInterface GetHardwareInterface(ushort index);

        /// <summary>
        /// Gets the hardware interface control instance asynchronously.
        /// </summary>
        /// <param name="index">Hardware interface index</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Hardware interface control instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        Task<IHardwareInterface> GetHardwareInterfaceAsync(ushort index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the server control instance.
        /// </summary>
        /// <param name="index">Server index</param>
        /// <returns>Server control instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        IServer GetServer(ushort index);

        /// <summary>
        /// Gets the server control instance asynchronously.
        /// </summary>
        /// <param name="index">Server index</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Server control instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Index is out of range.</exception>
        Task<IServer> GetServerAsync(ushort index, CancellationToken cancellationToken = default);
    }
}

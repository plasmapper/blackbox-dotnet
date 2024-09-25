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
        /// Reads the device state.
        /// </summary>
        /// <returns>Device state.</returns>
        DeviceState ReadDeviceState();

        /// <summary>
        /// Restarts the device.
        /// </summary>
        void Restart();

        /// <summary>
        /// Saves the device configuration to non-volatile memory.
        /// </summary>
        void SaveAllConfigurations();

        /// <summary>
        /// Clears the device restarted flag.
        /// </summary>
        void ClearRestartedFlag();

        /// <summary>
        /// Sets the device name.
        /// </summary>
        /// <param name="deviceName">Device name.</param>
        /// <returns>Set value.</returns>
        string SetDeviceName(string deviceName);

        /// <summary>
        /// Gets the hardware interface control instance.
        /// </summary>
        /// <param name="index">Hardware interface index</param>
        /// <returns>Hardware interface control instance.</returns>
        IHardwareInterface GetHardwareInterface(ushort index);

        /// <summary>
        /// Gets the server control instance.
        /// </summary>
        /// <param name="index">Server index</param>
        /// <returns>Server control instance.</returns>
        IServer GetServer(ushort index);
    }
}
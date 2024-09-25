namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox device state.
    /// </summary>
    public class DeviceState
    {
        /// <summary>
        /// Gets the restarted flag.
        /// </summary>
        public bool Restarted { get; internal set; }

        /// <summary>
        /// Gets the hardware information.
        /// </summary>
        public HardwareInfo HardwareInfo { get; internal set; }

        /// <summary>
        /// Gets the firmware information.
        /// </summary>
        public FirmwareInfo FirmwareInfo { get; internal set; }

        /// <summary>
        /// Gets the number of hardware interfaces.
        /// </summary>
        public ushort NumberOfHardwareInterfaces { get; internal set; }

        /// <summary>
        /// Gets the number of servers.
        /// </summary>
        public ushort NumberOfServers { get; internal set; }
    }
}

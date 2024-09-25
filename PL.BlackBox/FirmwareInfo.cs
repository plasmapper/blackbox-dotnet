namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox device firmware information.
    /// </summary>
    public class FirmwareInfo
    {
        /// <summary>
        /// Gets the firmware name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the firmware version.
        /// </summary>
        public Version Version { get; internal set; }
    }
}

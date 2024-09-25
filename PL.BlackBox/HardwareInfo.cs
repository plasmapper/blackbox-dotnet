namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox device hardware information.
    /// </summary>
    public class HardwareInfo
    {
        /// <summary>
        /// Gets the hardware name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the hardware version.
        /// </summary>
        public Version Version { get; internal set; }

        /// <summary>
        /// Gets the hardware unique ID.
        /// </summary>
        public string Uid { get; internal set; }
    }
}

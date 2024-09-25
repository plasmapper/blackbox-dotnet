namespace PL.BlackBox
{
    /// <summary>
    /// Semantic version.
    /// </summary>
    public class Version
    {
        /// <summary>
        /// Gets the major version.
        /// </summary>
        public ushort Major { get; internal set; }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        public ushort Minor { get; internal set; }

        /// <summary>
        /// Gets the patch version.
        /// </summary>
        public ushort Patch { get; internal set; }
    }
}

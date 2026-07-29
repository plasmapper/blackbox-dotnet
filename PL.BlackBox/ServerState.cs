namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox server state.
    /// </summary>
    public class ServerState
    {
        /// <summary>
        /// Gets the server type.
        /// </summary>
        public ServerType ServerType { get; internal set; }

        /// <summary>
        /// Gets the server name.
        /// </summary>
        public string Name { get; internal set; }
    }
}

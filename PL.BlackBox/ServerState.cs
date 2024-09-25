namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox server state.
    /// </summary>
    public class ServerState
    {
        private string _name;

        /// <summary>
        /// Gets the server type.
        /// </summary>
        public ServerType ServerType { get; internal set; }

        /// <summary>
        /// Gets the server name.
        /// </summary>
        public string Name
        {
            get => _name;
            internal set => _name = value;
        }
    }
}

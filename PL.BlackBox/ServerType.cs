namespace PL.BlackBox
{
    /// <summary>
    /// Server type.
    /// </summary>
    public enum ServerType : byte
    {
        /// <summary>
        /// Unknown server.
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// Stream server.
        /// </summary>
        StreamServer = 1,

        /// <summary>
        /// Network server.
        /// </summary>
        NetworkServer = 2,

        /// <summary>
        /// Stream Modbus server.
        /// </summary>
        StreamModbusServer = 3,

        /// <summary>
        /// Network Modbus server.
        /// </summary>
        NetworkModbusServer = 4,

        /// <summary>
        /// HTTP server.
        /// </summary>
        HttpServer = 5,

        /// <summary>
        /// mDNS server.
        /// </summary>
        MdnsServer = 6
    }
}

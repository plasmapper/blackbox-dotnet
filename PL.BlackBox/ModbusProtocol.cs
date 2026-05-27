namespace PL.BlackBox
{
    /// <summary>
    /// Modbus protocol.
    /// </summary>
    public enum ModbusProtocol : byte
    {
        /// <summary>
        /// Modbus RTU protocol.
        /// </summary>
        Rtu = 0,

        /// <summary>
        /// Modbus ASCII protocol.
        /// </summary>
        Ascii = 1,

        /// <summary>
        /// Modbus TCP protocol.
        /// </summary>
        Tcp = 2
    }
}

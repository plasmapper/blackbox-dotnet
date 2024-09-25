namespace PL.BlackBox
{
    /// <summary>
    /// Modbus protocol.
    /// </summary>
    public enum ModbusProtocol : byte
    {
        Rtu = 0,
        Ascii = 1,
        Tcp = 2
    }
}

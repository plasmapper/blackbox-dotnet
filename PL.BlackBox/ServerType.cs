namespace PL.BlackBox
{
    /// <summary>
    /// Server type.
    /// </summary>
    public enum ServerType : byte
    {
        Unknown = 0,
        StreamServer = 1,
        NetworkServer = 2,
        StreamModbusServer = 3,
        NetworkModbusServer = 4,
        HttpServer = 5,
        MdnsServer = 6
    }
}

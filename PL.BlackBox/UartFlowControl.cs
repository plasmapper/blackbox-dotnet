namespace PL.BlackBox
{
    /// <summary>
    /// Uart flow control.
    /// </summary>
    public enum UartFlowControl : byte
    {
        None = 0,
        Rts = 1,
        Cts = 2,
        RtsCts = 3
    }
}

namespace PL.BlackBox
{
    /// <summary>
    /// Uart flow control.
    /// </summary>
    public enum UartFlowControl : byte
    {
        /// <summary>
        /// No flow control.
        /// </summary>
        None = 0,

        /// <summary>
        /// RTS flow control.
        /// </summary>
        Rts = 1,

        /// <summary>
        /// CTS flow control.
        /// </summary>
        Cts = 2,

        /// <summary>
        /// RTS/CTS flow control.
        /// </summary>
        RtsCts = 3
    }
}

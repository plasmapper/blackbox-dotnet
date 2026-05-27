namespace PL.BlackBox
{
    /// <summary>
    /// Uart stop bits.
    /// </summary>
    public enum UartStopBits : byte
    {
        /// <summary>
        /// One stop bit.
        /// </summary>
        One = 0,

        /// <summary>
        /// 1.5 stop bits.
        /// </summary>
        OnePointFive = 1,

        /// <summary>
        /// Two stop bits.
        /// </summary>
        Two = 2
    }
}
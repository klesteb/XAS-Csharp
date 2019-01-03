
namespace XAS.Network.STOMP {

    /// <summary>
    /// A delegate` for the OnConnected event.
    /// </summary>
    /// <param name="frame">A STOMP connected frame.</param>
    ///
    public delegate void StompConnectedHandler(Frame frame);

    /// <summary>
    /// A delegate for the OnMessage event.
    /// </summary>
    /// <param name="frame">A STOMP message frame.</param>
    /// 
    public delegate void StompMessageHandler(Frame frame);

    /// <summary>
    /// A delegate for the OnReceipt event.
    /// </summary>
    /// <param name="frame">A STOMP receipt frame.</param>
    /// 
    public delegate void StompReceiptHandler(Frame frame);

    /// <summary>
    /// A delegate for the OnError event.
    /// </summary>
    /// <param name="frame">A STOMP error frame.</param>
    /// 
    public delegate void StompErrorHandler(Frame frame);

    /// <summary>
    /// A delegate for the OnNoop event.
    /// </summary>
    /// <param name="frame">A STOMP keepalive frame.</param>
    /// 
    public delegate void StompNoopHandler(Frame frame);

}

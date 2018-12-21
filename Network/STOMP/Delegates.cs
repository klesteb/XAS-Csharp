
namespace XAS.Network.STOMP {

    /// <summary>
    /// A delegate` for the OnConnected event.
    /// </summary>
    /// <param name="frame">A STOMP connected frame.</param>
    ///
    public delegate void OnStompConnected(Frame frame);

    /// <summary>
    /// A delegate for the OnMessage event.
    /// </summary>
    /// <param name="frame">A STOMP message frame.</param>
    /// 
    public delegate void OnStompMessage(Frame frame);

    /// <summary>
    /// A delegate for the OnReceipt event.
    /// </summary>
    /// <param name="frame">A STOMP receipt frame.</param>
    /// 
    public delegate void OnStompReceipt(Frame frame);

    /// <summary>
    /// A delegate for the OnError event.
    /// </summary>
    /// <param name="frame">A STOMP error frame.</param>
    /// 
    public delegate void OnStompError(Frame frame);

    /// <summary>
    /// A delegate for the OnNoop event.
    /// </summary>
    /// <param name="frame">A STOMP keepalive frame.</param>
    /// 
    public delegate void OnStompNoop(Frame frame);

}

using System;

namespace XAS.Network.TCP {


    public delegate void ServerDataSentHandler(Int32 id);
    public delegate void ServerExceptionHandler(Int32 id, Exception ex);
    public delegate void ServerDataReceivedHandler(Int32 id, Byte[] buffer);

    public delegate void ClientConnectHandler();
    public delegate void ClientDataSentHandler();
    public delegate void ClientDisconnectHandler();
    public delegate void ClientExceptionHandler(Exception ex);
    public delegate void ClientDataReceivedHandler(Byte[] buffer);

}

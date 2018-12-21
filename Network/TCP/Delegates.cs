using System;

namespace XAS.Network.TCP {


    public delegate void OnServerDataSent(Int32 id);
    public delegate void OnServerException(Int32 id, Exception ex);
    public delegate void OnServerDataReceived(Int32 id, Byte[] buffer);

    public delegate void OnClientConnect();
    public delegate void OnClientDataSent();
    public delegate void OnClientDisconnect();
    public delegate void OnClientException(Exception ex);
    public delegate void OnClientDataReceived(Byte[] buffer);

}

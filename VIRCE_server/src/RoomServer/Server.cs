using System.Net;
using System.Net.Sockets;

namespace VIRCE_server.RoomServer;

public abstract class Server
{
    public int Port { get; private set; }
    protected UdpClient? UdpClient;
    public bool IsRunning { get; } = false;
    public ushort RoomId { get; }

    protected Server()
    {
    }

    protected void Bind()
    {
        UdpClient ??= new UdpClient();
        Port = (UdpClient.Client.LocalEndPoint as IPEndPoint)!.Port;
    }

    public abstract void Start();
    public abstract void Stop();
}

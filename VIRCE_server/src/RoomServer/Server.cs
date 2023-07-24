using System.Net;
using System.Net.Sockets;
using VIRCE_server.DataBase;

namespace VIRCE_server.RoomServer;

public abstract class Server
{
    public int Port { get; private set; }
    protected UdpClient? UdpClient;
    public bool IsRunning { get; protected set; } = false;
    public int RoomId { get; protected set; }
    protected RoomServerInfo ServerInfo = null!;

    protected Server()
    {
    }

    protected void Bind()
    {
        UdpClient ??= new UdpClient();
        Port = (UdpClient.Client.LocalEndPoint as IPEndPoint)!.Port;
    }

    protected static int GetId()
    {
        var ids = DataBaseManager.GetRoomIds();
        var id = 0;
        while (true)
        {
            if (!ids.Contains(id)) return id;
            id++;
        }
    }

    public abstract void Start();
    public abstract void Stop();
}

using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using VIRCE_server.DataBase;

namespace VIRCE_server.RoomServer;

public abstract class Server
{
    public int Port { get; private set; }
    protected UdpClient? UdpClient;
    public bool IsRunning { get; protected set; } = false;
    public ushort RoomId { get; protected set; }
    protected RoomServerInfo ServerInfo = null!;

    protected Server()
    {
    }

    protected void Bind()
    {
        UdpClient ??= new UdpClient();
        Port = (UdpClient.Client.LocalEndPoint as IPEndPoint)!.Port;
    }

    protected static ushort GetId()
    {
        var ids = DataBaseManager.GetRoomIds();
        ushort id = 0;
        while (true)
        {
            if (!ids.Contains(id)) return id;
            id++;
        }
    }

    protected async UniTask ReceiveStart()
    {
        if (UdpClient is null) return;
        var res = await UdpClient.ReceiveAsync();
        UniTask.Run(() => OnReceive(res.RemoteEndPoint, res.Buffer), false);
    }

    public abstract void Start();
    public abstract void Stop();
    public abstract void OnReceive(IPEndPoint remoteEndPoint, byte[] data);
}

using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using VIRCE_server.DataBase;

namespace VIRCE_server.RoomServer;

public abstract class Server
{
    public int Port { get; private set; }
    protected UdpClient? UdpClient;
    public bool IsRunning { get; protected set; }
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

    protected void Broadcast(ushort roomId, byte[] data)
    {
        UniTask.Run(() =>
        {
            var roomMembers = DataBaseManager.GetUsers(roomId);
            foreach (var roomMember in roomMembers)
            {
                UdpClient?.SendAsync(data, data.Length, roomMember.RemoteEndPoint);
            }
        }, false);
    }

    protected void Entry(in IPEndPoint endPoint)
    {
        var userId = GetUserId();
        var user = new UserData
        {
            UserId = userId,
            RoomId = RoomId,
            RemoteEndPoint = endPoint
        };
        DataBaseManager.AddUserData(user);
        var msg = BitConverter.GetBytes(userId);
        UdpClient?.SendAsync(msg, msg.Length, endPoint);
    }

    protected static ushort GetRoomId()
    {
        var ids = DataBaseManager.GetRoomIds();
        ushort id = 1;
        while (true)
        {
            if (!ids.Contains(id)) return id;
            id++;
        }
    }

    private ushort GetUserId()
    {
        var ids = DataBaseManager.GetUsers(RoomId).Select(data => data.UserId).ToArray();
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
        while(true)
        {
            var res = await UdpClient.ReceiveAsync();
            UniTask.Run(() => OnReceive(res.RemoteEndPoint, res.Buffer), false);
        }
    }

    public abstract void Start();
    public abstract void Stop();
    protected abstract void OnReceive(in IPEndPoint remoteEndPoint, in byte[] data);
}

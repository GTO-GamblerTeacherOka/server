using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using VIRCE_server.DataBase;

namespace VIRCE_server.RoomServer;

public abstract class Server
{
    protected RoomServerInfo ServerInfo = null!;
    protected UdpClient? UdpClient;

    protected Server(int port)
    {
        Port = port;
    }

    public int Port { get; private set; }

    ~Server()
    {
        Stop();
    }

    protected void Bind(int port = 0)
    {
        UdpClient ??= new UdpClient(port);
        Port = (UdpClient.Client.LocalEndPoint as IPEndPoint)!.Port;
    }

    protected void Broadcast(byte roomId, byte[] data)
    {
        UniTask.Run(() =>
        {
            var roomMembers = DataBaseManager.GetUsers(roomId);
            foreach (var roomMember in roomMembers) UdpClient?.SendAsync(data, data.Length, roomMember.RemoteEndPoint);
        }, false);
    }

    public void Entry(in IPEndPoint endPoint)
    {
        var isLobby = this is LobbyRoom;
        var roomIds = DataBaseManager.GetRooms()
            .Where(info =>
                info.Type == (isLobby ? RoomServerInfo.ServerType.Lobby : RoomServerInfo.ServerType.MiniGame))
            .Select(info => info.RoomId).ToArray();
        byte roomId = 0;
        Parallel.ForEach(roomIds, id =>
        {
            if (roomId == 0 || DataBaseManager.GetUsers(id).Count < DataBaseManager.GetUsers(roomId).Count)
                roomId = id;
        });
        var userId = GetUserId(roomId);
        var user = new UserData
        {
            UserId = userId,
            RoomId = roomId,
            RemoteEndPoint = endPoint
        };
        DataBaseManager.AddUserData(user);
        var msg = BitConverter.GetBytes(user.GlobalUserId);
        Console.WriteLine($"Entry: {user.GlobalUserId}");
        UdpClient?.SendAsync(msg, msg.Length, endPoint);
    }

    private static byte GetUserId(in byte roomId)
    {
        var ids = DataBaseManager.GetUsers(roomId).Select(data => data.UserId).ToArray();
        byte id = 1;
        while (true)
        {
            if (!ids.Contains(id)) return id;
            id++;
        }
    }

    protected static byte GetRoomId()
    {
        byte id = 1;
        var ids = DataBaseManager.GetRoomIds();
        while (true)
        {
            if (!ids.Contains(id)) return id;
            id++;
        }
    }

    protected async UniTask ReceiveStart()
    {
        if (UdpClient is null) return;
        while (true)
        {
            var res = await UdpClient.ReceiveAsync();
            UniTask.Run(() => OnReceive(res.RemoteEndPoint, res.Buffer), false);
        }
    }

    public abstract void Start();
    public abstract void Stop();
    protected abstract void OnReceive(in IPEndPoint remoteEndPoint, in byte[] data);
}
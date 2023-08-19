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
    public byte RoomId { get; protected set; }
    protected RoomServerInfo ServerInfo = null!;

    public int Occupants { get; private set; }
    private bool _available = true;

    protected Server()
    {
        Occupants = 0;
        UniTask.Run(() =>
        {
            while(_available)
            {
                if (IsRunning)
                {
                    Occupants = DataBaseManager.GetUsers(RoomId).Count;
                }
            }
        });
    }
    
    ~Server()
    {
        _available = false;
        Task.Delay(2000).Wait();
    }

    protected void Bind()
    {
        UdpClient ??= new UdpClient(0);
        Port = (UdpClient.Client.LocalEndPoint as IPEndPoint)!.Port;
    }

    protected void Broadcast(byte roomId, byte[] data)
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

    public void Entry(in IPEndPoint endPoint)
    {
        var userId = GetUserId();
        var user = new UserData
        {
            UserId = userId,
            RoomId = RoomId,
            RemoteEndPoint = endPoint
        };
        DataBaseManager.AddUserData(user);
        var msg = BitConverter.GetBytes(user.GlobalUserId);
        Console.WriteLine($"Entry: {user.GlobalUserId}");
        UdpClient?.SendAsync(msg, msg.Length, endPoint);
    }

    protected static byte GetRoomId()
    {
        var ids = DataBaseManager.GetRoomIds();
        byte id = 1;
        while (true)
        {
            if (!ids.Contains(id)) return id;
            id++;
        }
    }

    private byte GetUserId()
    {
        var ids = DataBaseManager.GetUsers(RoomId).Select(data => data.UserId).ToArray();
        byte id = 1;
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

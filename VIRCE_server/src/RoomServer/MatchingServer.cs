using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using VIRCE_server.PacketUtil;

namespace VIRCE_server.RoomServer;

public class MatchingServer
{
    private static MatchingServer? _instance;
    
    private readonly ServerCluster _lobbyServerCluster;
    private readonly ServerCluster _miniGameServerCluster;

    private const int Port = 5000;
    private readonly UdpClient _udpClient;
    public bool IsRunning { get; private set; }

    public static MatchingServer Instance
    {
        get
        {
            return _instance ??= new MatchingServer();
        }
    }
    
    private MatchingServer()
    {
        _lobbyServerCluster = new LobbyRoomCluster();
        _miniGameServerCluster = new MiniGameRoomCluster();
        _udpClient = new UdpClient(Port);
        IsRunning = false;
    }

    public void Start()
    {
        _lobbyServerCluster.Start();
        _miniGameServerCluster.Start();
        IsRunning = true;
        ReceiveStart().Forget();
    }

    public void Stop()
    {
        IsRunning = false;
    }

    private async UniTask ReceiveStart()
    {
        while (true)
        {
            if (IsRunning is false) break;
            var data = await _udpClient.ReceiveAsync();
            var (rawHeader,rawBody) = DataParser.Split(data.Buffer);
            var header = DataParser.AnalyzeHeader(rawHeader);
            if (header.flag != DataParser.Flag.RoomEntry) continue;
            var roomType = rawBody[0];
            switch (roomType)
            {
                case 0:
                    _lobbyServerCluster.Entry(data.RemoteEndPoint);
                    break;
                case 1:
                    _miniGameServerCluster.Entry(data.RemoteEndPoint);
                    break;
            }
        }
    }
}

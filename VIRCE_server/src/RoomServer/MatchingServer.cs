using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using VIRCE_server.PacketUtil;

namespace VIRCE_server.RoomServer;

public class MatchingServer
{
    private const int Port = 5000;
    private static MatchingServer? _instance;

    private readonly ServerCluster _lobbyServerCluster;
    private readonly ServerCluster _miniGameServerCluster;
    private readonly UdpClient _udpClient;

    private MatchingServer()
    {
        _lobbyServerCluster = new LobbyRoomCluster();
        _miniGameServerCluster = new MiniGameRoomCluster();
        _udpClient = new UdpClient(Port);
        IsRunning = false;
    }

    public bool IsRunning { get; private set; }

    public static MatchingServer Instance
    {
        get { return _instance ??= new MatchingServer(); }
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
            ReceiveHandler(data).Forget();
        }
    }

    private async UniTask ReceiveHandler(UdpReceiveResult res)
    {
        await UniTask.Run(() =>
        {
            var (rawHeader, rawBody) = DataParser.Split(res.Buffer);
            var header = DataParser.AnalyzeHeader(rawHeader);
            if (header.flag != DataParser.Flag.RoomEntry) return;
            var roomType = rawBody[0];
            switch (roomType)
            {
                case 0:
                    _lobbyServerCluster.Entry(res.RemoteEndPoint);
                    break;
                case 1:
                    _miniGameServerCluster.Entry(res.RemoteEndPoint);
                    break;
            }
        });
    }
}
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using VIRCE_server.PacketUtil;

namespace VIRCE_server.RoomServer;

public class MatchingServer
{
    private const int Port = 5000;
    private static MatchingServer? _instance;
    private readonly Server _lobbyServer;
    private readonly Server _miniGameServer;
    private readonly UdpClient _udpClient;

    private MatchingServer()
    {
        _lobbyServer = new LobbyRoom(8192);
        _miniGameServer = new MiniGameRoom(8193);
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
        _lobbyServer.Start();
        _miniGameServer.Start();
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
                    _lobbyServer.Entry(res.RemoteEndPoint);
                    break;
                case 1:
                    _miniGameServer.Entry(res.RemoteEndPoint);
                    break;
            }
        });
    }
}
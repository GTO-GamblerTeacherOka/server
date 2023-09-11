using System.Net;
using Cysharp.Threading.Tasks;
using VIRCE_server.DataBase;
using VIRCE_server.PacketUtil;

namespace VIRCE_server.RoomServer;

public class MiniGameRoom : Server
{
    public MiniGameRoom(int port) : base(port)
    {
    }

    public override void Start()
    {
        if (UdpClient is null) Bind();
        var roomId = GetRoomId();
        ServerInfo = new RoomServerInfo
        {
            RoomId = roomId,
            Type = RoomServerInfo.ServerType.MiniGame
        };
        DataBaseManager.AddRoomServerInfo(ServerInfo);
        ReceiveStart().Forget();
    }

    public override void Stop()
    {
        UdpClient?.Close();
        UdpClient = null;
        DataBaseManager.RemoveRoomServerInfo(ServerInfo);
    }

    protected override void OnReceive(in IPEndPoint remoteEndPoint, in byte[] data)
    {
        var parsedData = DataParser.Split(data);
        var header = DataParser.AnalyzeHeader(parsedData.header);
        switch (header.flag)
        {
            case DataParser.Flag.PositionData:
            case DataParser.Flag.AvatarData:
                Broadcast(header.roomID, data);
                break;
            case DataParser.Flag.RoomEntry:
                Entry(remoteEndPoint);
                break;
            case DataParser.Flag.RoomExit:
                break;
            case DataParser.Flag.SendReaction:
                break;
            case DataParser.Flag.ReceiveReaction:
                break;
            case DataParser.Flag.ChatData:
                Broadcast(header.roomID, data);
                break;
            case DataParser.Flag.GetRemoteEndPoint:
                break;
            case DataParser.Flag.ReceiveRemoteEndPoint:
                break;
            default:
                throw new Exception("the flag is not defined.");
        }
    }
}
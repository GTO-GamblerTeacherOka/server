using System.Net;
using Cysharp.Threading.Tasks;
using VIRCE_server.DataBase;
using VIRCE_server.PacketUtil;

namespace VIRCE_server.RoomServer;

public class LobbyRoom : Server
{
    public override void Start()
    {
        if (UdpClient is null)
        {
            Bind();
        }
        RoomId = GetRoomId();
        ServerInfo = new RoomServerInfo
        {
            RoomId = RoomId,
            Port = Port,
            Type = RoomServerInfo.ServerType.Lobby
        };
        DataBaseManager.AddRoomServerInfo(ServerInfo);
        ReceiveStart().Forget();
        IsRunning = true;
    }
    
    public override void Stop()
    {
        IsRunning = false;
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

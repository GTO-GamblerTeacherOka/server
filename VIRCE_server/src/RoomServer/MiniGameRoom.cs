using VIRCE_server.DataBase;

namespace VIRCE_server.RoomServer;

public class MiniGameRoom : Server
{
    public override void Start()
    {
        if (UdpClient is null)
        {
            Bind();
        }
        RoomId = GetId();
        ServerInfo = new RoomServerInfo
        {
            RoomId = RoomId,
            Port = Port,
            Type = RoomServerInfo.ServerType.MiniGame
        };
        DataBaseManager.AddRoomServerInfo(ServerInfo);
        IsRunning = true;
    }
    
    public override void Stop()
    {
        IsRunning = false;
        UdpClient?.Close();
        UdpClient = null;
        DataBaseManager.RemoveRoomServerInfo(ServerInfo);
    }
}

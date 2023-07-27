using System.Net;

namespace VIRCE_server.RoomServer;

public class MiniGameRoomCluster : ServerCluster
{
    public MiniGameRoomCluster()
    {
        AddServer(new MiniGameRoom());
    }
    
    public override void Entry(in IPEndPoint ep)
    {
        Server server;
        if (Servers.Count is 0)
        {
            server = new MiniGameRoom();
            AddServer(server);
            server.Start();
        }
        server = Servers.OrderBy(s => s.Occupants).First();
        if (server.Occupants >= 24)
        {
            server = new MiniGameRoom();
            AddServer(server);
            server.Start();
        }
        server.Entry(ep);
    }
}

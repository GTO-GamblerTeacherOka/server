using System.Net;

namespace VIRCE_server.RoomServer;

public class LobbyRoomCluster : ServerCluster
{
    public LobbyRoomCluster()
    {
        AddServer(new LobbyRoom());
    }
    
    public override void Entry(in IPEndPoint ep)
    {
        Server server;
        if (Servers.Count is 0)
        {
            server = new LobbyRoom();
            AddServer(server);
            server.Start();
        }
        server = Servers.OrderBy(s => s.Occupants).First();
        if (server.Occupants >= 24)
        {
            server = new LobbyRoom();
            AddServer(server);
            server.Start();
        }
        server.Entry(ep);
    }
}
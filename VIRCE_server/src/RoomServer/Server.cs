using System.Net;
using System.Net.Sockets;

namespace VIRCE_server.RoomServer;

public abstract class Server
{
    public readonly IPEndPoint LocalEndPoint;
    private UdpClient _udpClient;

    protected Server(IPEndPoint localEndPoint)
    {
        LocalEndPoint = localEndPoint;
        _udpClient = new UdpClient(LocalEndPoint);
    }

    protected Server(string ip, int port)
    {
        LocalEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        _udpClient = new UdpClient(LocalEndPoint);
    }

    public abstract void Start();
    public abstract void Stop();
}
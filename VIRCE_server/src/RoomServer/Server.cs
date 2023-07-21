using System.Net;
using System.Net.Sockets;

namespace VIRCE_server.RoomServer;

public abstract class Server
{
    public int Port { get; }
    private UdpClient? _udpClient;
    public bool IsRunning { get; } = false;
    public ushort RoomId { get; }

    protected Server(int port)
    {
        Port = port;
    }

    public void Bind()
    {
        try
        {
            _udpClient ??= new UdpClient(Port);
        }
        catch (SocketException e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public abstract void Start();
    public abstract void Stop();
}

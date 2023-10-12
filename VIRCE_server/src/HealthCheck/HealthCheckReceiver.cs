using System.Net.Sockets;

namespace VIRCE_server.HealthCheck;

public static class HealthCheckReceiver
{
    private static readonly TcpClient Client = new();

    static HealthCheckReceiver()
    {
        Client.Connect("localhost", 8080);
    }

    public static void Initialize()
    {
    }
}
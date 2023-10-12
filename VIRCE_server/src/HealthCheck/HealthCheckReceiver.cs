using System.Net;
using System.Net.Sockets;

namespace VIRCE_server.HealthCheck;

public static class HealthCheckReceiver
{
    private static readonly TcpListener Client;

    static HealthCheckReceiver()
    {
        Client = new TcpListener(IPAddress.Any, 8000);
    }

    public static void Initialize()
    {
        Client.Start();
    }
}
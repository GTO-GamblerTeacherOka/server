using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace VIRCE_server.Server;

public static class Socket
{
    private static readonly UdpClient UdpClient;

    static Socket()
    {
        UdpClient = new UdpClient(5000);
    }

    public static async UniTask SendAsync(byte[] data, IPEndPoint endPoint)
    {
        await UdpClient.SendAsync(data, data.Length, endPoint);
    }

    public static async UniTask<UdpReceiveResult> ReceiveAsync()
    {
        return await UdpClient.ReceiveAsync();
    }
}
namespace VIRCE_server.RoomServer;

public class MiniGameRoom : Server
{
    public override void Start()
    {
        if (UdpClient is null)
        {
            Bind();
        }
    }
    
    public override void Stop()
    {
        UdpClient?.Close();
        UdpClient = null;
    }
}

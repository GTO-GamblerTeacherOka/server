using System.Net;

namespace VIRCE_server.RoomServer;

public abstract class ServerCluster
{
    protected readonly List<Server> Servers;
    
    protected ServerCluster()
    {
        Servers = new List<Server>();
    }
    
    public void AddServer(Server server)
    {
        Servers.Add(server);
    }
    
    public void Start()
    {
        foreach (var server in Servers.Where(server => !server.IsRunning))
        {
            server.Start();
        }
    }
    
    public void Stop()
    {
        foreach (var server in Servers.Where(server => server.IsRunning))
        {
            server.Stop();
        }
    }
    
    public void Restart()
    {
        Stop();
        Start();
    }
    
    public void RemoveServer(Server server)
    {
        Servers.Remove(server);
    }
    
    
    public void RemoveServer(int port)
    {
        Servers.RemoveAll(server => server.Port == port);
    }

    public abstract void Entry(in IPEndPoint ep);
}

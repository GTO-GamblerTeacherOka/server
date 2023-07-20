using System.Net;

namespace VIRCE_server.RoomServer;

public abstract class ServerCluster
{
    private readonly List<Server> _servers;
    private readonly List<int> _ports;
    
    protected ServerCluster()
    {
        _servers = new List<Server>();
        _ports = new List<int>();
    }
    
    public void AddServer(Server server)
    {
        _servers.Add(server);
        _ports.Add(server.Port);
    }
    
    public void Start()
    {
        foreach (var server in _servers.Where(server => !server.IsRunning))
        {
            server.Start();
        }
    }
    
    public void Stop()
    {
        foreach (var server in _servers.Where(server => server.IsRunning))
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
        _servers.Remove(server);
    }
    
    
    public void RemoveServer(int port)
    {
        _servers.RemoveAll(server => server.Port == port);
    }
}

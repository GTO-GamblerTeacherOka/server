using System.Net;

namespace VIRCE_server.RoomServer;

public abstract class ServerCluster
{
    private List<Server> _servers;
    
    protected ServerCluster()
    {
        _servers = new List<Server>();
    }
    
    public void AddServer(Server server)
    {
        _servers.Add(server);
    }
    
    public void Start()
    {
        foreach (var server in _servers)
        {
            server.Start();
        }
    }
    
    public void Stop()
    {
        foreach (var server in _servers)
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
    
    public void RemoveServer(int index)
    {
        _servers.RemoveAt(index);
    }
    
    public void RemoveServer(IPEndPoint localEndPoint)
    {
        _servers.RemoveAll(server => server.LocalEndPoint.Equals(localEndPoint));
    }
}

namespace VIRCE_server.RoomServer;

public class ClusterManager
{
    private static ClusterManager _instance;

    public static ClusterManager Instance
    {
        get
        {
            return _instance ??= new ClusterManager();
        }
    }
    
    private ServerCluster _lobbyCluster;
    private ServerCluster _miniGameCluster;
    
    private ClusterManager()
    {
        
    }
}

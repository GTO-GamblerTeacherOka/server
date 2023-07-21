namespace VIRCE_server.RoomServer;

public class MatchingServer
{
    private static MatchingServer _instance;

    public static MatchingServer Instance
    {
        get
        {
            return _instance ??= new MatchingServer();
        }
    }
    
    private MatchingServer()
    {
        
    }
    
    
}

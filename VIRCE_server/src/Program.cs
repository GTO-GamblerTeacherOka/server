using VIRCE_server.DataBase;
using VIRCE_server.RoomServer;

namespace VIRCE_server;

internal static class VirceServer
{
    private static async Task Main()
    {
        var client = new HttpClient();
        Console.WriteLine("Starting...");
        DataBaseManager.Initialize();
        MatchingServer.Instance.Start();
    }
}

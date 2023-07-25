using VIRCE_server.DataBase;

namespace VIRCE_server;

internal static class VirceServer
{
    private static async Task Main()
    {
        var client = new HttpClient();
        Console.WriteLine("Starting...");
        DataBaseManager.Initialize();
    }
}

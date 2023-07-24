using VIRCE_server.DataBase;

namespace VIRCE_server;

internal static class VirceServer
{
    private static void Main()
    {
        Console.WriteLine("Starting...");
        DataBaseManager.Initialize();
    }
}

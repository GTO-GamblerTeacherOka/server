using DotNetEnv;
using VIRCE_server.Controller;
using VIRCE_server.DataBase;

namespace VIRCE_server;

internal static class VirceServer
{
    private static async Task Main()
    {
        Console.WriteLine("Starting...");
        Init();
        await Server.Server.Run();
    }

    private static void Init()
    {
        Env.Load("./.env");
        DataBaseManager.Initialize();
        RedisController.Initialize();
        DataBaseManager.Initialize();
    }
}

// copyright 2023 Ukai Shota
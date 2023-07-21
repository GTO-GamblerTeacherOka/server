using MasterMemory;

namespace VIRCE_server.DataBase;

public static class DataBaseManager
{
    private static DatabaseBuilder _builder = new();
    private static MemoryDatabase? _db;
    private const string DbSavePath = "~/VIRECE_server_db.bytes";
    
    public static void Initialize()
    {
        _db = new MemoryDatabase(_builder.Build());
    }
    
    public static void AddUserData(UserData userData)
    {
        _builder.Append(new List<UserData> {userData});
        _db = new MemoryDatabase(_builder.Build());
        Save();
    }
    
    public static void RemoveUserData(ushort userId, ushort roomId)
    {
        var globalUserId = userId | roomId << 5;
        var builder = _db?.ToImmutableBuilder();
        builder?.RemoveUserData(new []{
            globalUserId
        });
        _db = builder?.Build();
        _builder = _db?.ToDatabaseBuilder() ?? _builder;
        Save();
    }
    
    private static async void Save()
    {
        var bytes = _builder.Build();
        await File.WriteAllBytesAsync(DbSavePath, bytes);
    }
    
    public static UserData? GetUserData(ushort userId, ushort roomId)
    {
        var globalUserId = userId | roomId << 5;
        return _db?.UserDataTable.FindByGlobalUserId(globalUserId);
    }

    public static RangeView<UserData>? GetUsers(ushort roomId)
    {
        return _db?.UserDataTable.FindByRoomId(roomId);
    }
    
    public static int[]? GetRoomIds()
    {
        return _db?.UserDataTable.SortByRoomId.Select(x => x.RoomId).Distinct().ToArray();
    }
}

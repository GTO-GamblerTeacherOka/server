using Cysharp.Threading.Tasks;
using MasterMemory;
using VIRCE_server.MasterMemoryDataBase;

namespace VIRCE_server.DataBase;

public static class DataBaseManager
{
    private const string DbSavePath = "./VIRECE_server_db.bytes";
    private static DatabaseBuilder _builder = new();
    private static MemoryDatabase _db;

    static DataBaseManager()
    {
        _db = new MemoryDatabase(_builder.Build());
    }

    public static void Initialize()
    {
    }

    public static void AddUserData(in UserData userData)
    {
        var builder = _db.ToImmutableBuilder();
        builder.Diff(new[] { userData });
        _db = builder.Build();
        Save().Forget();
    }

    public static void RemoveUserData(in ushort userId, in int roomId)
    {
        var globalUserId = (ushort)(userId | (roomId << 5));
        var builder = _db.ToImmutableBuilder();
        builder.RemoveUserData(new[]
        {
            globalUserId
        });
        _db = builder.Build();
        _builder = _db?.ToDatabaseBuilder() ?? _builder;
        Save().Forget();
    }

    public static void RemoveUserData(in UserData userData)
    {
        RemoveUserData(userData.UserId, userData.RoomId);
    }

    public static void AddRoomServerInfo(in RoomServerInfo roomServerInfo)
    {
        if (_db.RoomServerInfoTable.FindByRoomId(roomServerInfo.RoomId) is not null)
            throw new Exception("RoomServer is already exist");

        var builder = _db.ToImmutableBuilder();
        builder.Diff(new[] { roomServerInfo });
        _db = builder.Build();
        Save().Forget();
    }

    public static void RemoveRoomServerInfo(in byte roomId)
    {
        var builder = _db.ToImmutableBuilder();
        builder.RemoveRoomServerInfo(new[]
        {
            roomId
        });
        _db = builder.Build();
        _builder = _db?.ToDatabaseBuilder() ?? _builder;
        Save().Forget();
    }

    public static void RemoveRoomServerInfo(in RoomServerInfo roomServerInfo)
    {
        RemoveRoomServerInfo(roomServerInfo.RoomId);
    }

    private static async UniTask Save()
    {
        var bytes = _builder.Build();
        try
        {
            await File.WriteAllBytesAsync(DbSavePath, bytes);
        }
        catch
        {
            // ignored
        }
    }

    public static UserData GetUserData(in byte userId, in byte roomId)
    {
        var globalUserId = (ushort)(userId | (roomId << 5));
        return _db.UserDataTable.FindByGlobalUserId(globalUserId);
    }

    public static RangeView<UserData> GetUsers(in byte? roomId = null)
    {
        return roomId is null ? _db.UserDataTable.All : _db.UserDataTable.FindByRoomId(roomId.Value);
    }

    public static RangeView<RoomServerInfo> GetRooms()
    {
        return _db.RoomServerInfoTable.All;
    }

    public static RoomServerInfo GetRoomFromRoomId(in byte roomId)
    {
        return _db.RoomServerInfoTable.FindByRoomId(roomId);
    }

    public static byte[] GetRoomIds()
    {
        var roomIds = _db.RoomServerInfoTable.All.Select(info => info.RoomId);

        return roomIds.ToArray();
    }
}
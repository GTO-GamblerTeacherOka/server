using Cysharp.Threading.Tasks;
using MasterMemory;
using VIRCE_server.Controller;
using VIRCE_server.MasterMemoryDataBase;
using VIRCE_server.PacketUtil;

namespace VIRCE_server.DataBase;

public static class DataBaseManager
{
    private const int CacheTimeSpan = 3000;
    private static DatabaseBuilder _builder = new();
    private static MemoryDatabase _db;
    private static bool _isStarted;

    static DataBaseManager()
    {
        _db = new MemoryDatabase(_builder.Build());
    }

    public static void Initialize()
    {
        Start();
    }

    private static void Start()
    {
        if (_isStarted) return;
        _isStarted = true;
        UniTask.Run(() =>
        {
            while (_isStarted)
            {
                Cache().Forget();
                var users = _db.UserDataTable.All;
                foreach (var user in users)
                {
                    var globalUserId = DataParser.GetGlobalUserId(user.UserID, user.RoomID);
                    var lastTimeString = RedisController.GetString(globalUserId.ToString());
                    if (lastTimeString.Length is 0) continue;
                    var lastTime = int.Parse(lastTimeString);
                    var now = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    if (lastTime + 20 <= now) continue;
                    RemoveUserData(user);
                    MySqlController.DeleteUser(user.RoomID, user.UserID).Forget();
                }

                Task.Delay(CacheTimeSpan);
            }
        }).Forget();
    }

    private static async UniTask Cache()
    {
        var users = await MySqlController.Query<UserData>();
        var roomServerInfos = await MySqlController.Query<RoomServerInfo>();

        var builder = _db.ToImmutableBuilder();
        builder.ReplaceAll(users.ToArray());
        builder.ReplaceAll(roomServerInfos.ToArray());
        _db = builder.Build();
        _builder = _db.ToDatabaseBuilder();
    }

    public static void AddUserData(in UserData userData)
    {
        var builder = _db.ToImmutableBuilder();
        builder.Diff(new[] { userData });
        _db = builder.Build();
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
    }

    public static void RemoveUserData(in UserData userData)
    {
        RemoveUserData(userData.UserID, userData.RoomID);
    }

    public static void AddRoomServerInfo(in RoomServerInfo roomServerInfo)
    {
        if (_db.RoomServerInfoTable.FindByRoomID(roomServerInfo.RoomID) is not null)
            throw new Exception("RoomServer is already exist");

        var builder = _db.ToImmutableBuilder();
        builder.Diff(new[] { roomServerInfo });
        _db = builder.Build();
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
    }

    public static void RemoveRoomServerInfo(in RoomServerInfo roomServerInfo)
    {
        RemoveRoomServerInfo(roomServerInfo.RoomID);
    }

    public static UserData GetUserData(in byte userId, in byte roomId)
    {
        var globalUserId = (ushort)(userId | (roomId << 5));
        return _db.UserDataTable.FindByGlobalUserId(globalUserId);
    }

    public static RangeView<UserData> GetUsers(in byte? roomId = null)
    {
        return roomId is null ? _db.UserDataTable.All : _db.UserDataTable.FindByRoomID(roomId.Value);
    }

    public static RangeView<RoomServerInfo> GetRooms()
    {
        return _db.RoomServerInfoTable.All;
    }

    public static RoomServerInfo GetRoomFromRoomId(in byte roomId)
    {
        return _db.RoomServerInfoTable.FindByRoomID(roomId);
    }

    public static byte[] GetRoomIds()
    {
        var roomIds = _db.RoomServerInfoTable.All.Select(info => info.RoomID);

        return roomIds.ToArray();
    }
}
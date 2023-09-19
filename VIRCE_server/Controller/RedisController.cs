using DotNetEnv;
using StackExchange.Redis;

namespace VIRCE_server.Controller;

public static class RedisController
{
    private static ConnectionMultiplexer? _redis;
    private static IDatabase? _db;

    public static void Initialize()
    {
        _redis = ConnectionMultiplexer.Connect(Env.GetString("REDIS_HOST"));
        _db = _redis.GetDatabase();
    }

    public static void SetString(in string key, in string value)
    {
        _db?.StringSet(key, value);
    }

    public static string GetString(in string key)
    {
        return (_db?.StringGet(key) ?? string.Empty)!;
    }

    public static void SetHash(in string key, in HashEntry[] hashEntries)
    {
        _db?.HashSet(key, hashEntries);
    }

    public static HashEntry[] GetHash(in string key)
    {
        return _db?.HashGetAll(key) ?? Array.Empty<HashEntry>();
    }

    public static void SetList(in string key, in string[] values)
    {
        _db?.ListRightPush(key, values.Select(RedisValue.Unbox).ToArray());
    }

    public static void SetList(in string key, in string value)
    {
        _db?.ListRightPush(key, value);
    }

    public static string[] GetList(in string key)
    {
        return (_db?.ListRange(key) ?? Array.Empty<RedisValue>()).Select(x => x.ToString()).ToArray();
    }

    public static void Remove(in string key)
    {
        _db?.KeyDelete(key);
    }

    public static void Remove(in string[] keys)
    {
        _db?.KeyDelete(keys.Select(x => (RedisKey)x).ToArray());
    }

    public static void Remove(in string key, in string value)
    {
        _db?.ListRemove(key, value);
    }

    public static bool SetNx(in string key)
    {
        return _db?.StringSet(key, "1", TimeSpan.FromSeconds(100), When.NotExists, CommandFlags.None) ?? false;
    }
}
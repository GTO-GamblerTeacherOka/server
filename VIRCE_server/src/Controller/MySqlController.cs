using System.Data;
using Dapper;
using DotNetEnv;
using MySql.Data.MySqlClient;
using VIRCE_server.DataBase;

namespace VIRCE_server.Controller;

public static class MySqlController
{
    private static IDbConnection? _connection;

    public static void Initialize()
    {
        var connectionString = $"Server={Env.GetString("MYSQL_HOST")};User ID=virce;Password=virce;Database=virce";
        _connection = new MySqlConnection(connectionString);
    }

    public static IEnumerable<T> Query<T>()
    {
        if (_connection is null)
            Initialize();

        string sql;
        if (typeof(T) == typeof(UserData))
            sql = @"SELECT * FROM User";
        else if (typeof(T) == typeof(RoomServerInfo))
            sql = @"SELECT * FROM Room";
        else
            throw new Exception("Invalid Type");
        _connection!.Open();
        var result = _connection.Query<T>(sql);
        _connection.Close();
        return result;
    }

    public static void Insert<T>(in T data)
    {
        if (_connection is null)
            Initialize();

        string sql;
        if (typeof(T) == typeof(UserData) || typeof(T) == typeof(List<UserData>))
            sql =
                @"INSERT INTO User (UserID, RoomID, ModelID, DisplayName, IPAddress, Port) VALUES (@UserID, @RoomID, @ModelID, @DisplayName, @IPAddress, @Port)";
        else if (typeof(T) == typeof(RoomServerInfo) || typeof(T) == typeof(List<RoomServerInfo>))
            sql = @"INSERT INTO Room (RoomID, RoomType) VALUES (@RoomID, @RoomType)";
        else
            throw new Exception("Invalid Type");

        _connection!.Open();
        _connection.Execute(sql, data);
        _connection.Close();
    }

    public static void Update<T>(in T data)
    {
        if (_connection is null)
            Initialize();

        string sql;
        if (typeof(T) == typeof(UserData) || typeof(T) == typeof(List<UserData>))
            sql =
                @"UPDATE User SET UserID = @UserID, RoomID = @RoomID, ModelID = @ModelID, DisplayName = @DisplayName, IPAddress = @IPAddress, Port = @Port WHERE UserID = @UserID AND RoomID = @RoomID";
        else if (typeof(T) == typeof(RoomServerInfo) || typeof(T) == typeof(List<RoomServerInfo>))
            sql = @"UPDATE Room SET RoomID = @RoomID, RoomType = @RoomType WHERE RoomID = @RoomID";
        else
            throw new Exception("Invalid Type");

        _connection!.Open();
        _connection.Execute(sql, data);
        _connection.Close();
    }

    public static void DeleteUser(byte roomId, byte userId)
    {
        if (_connection is null)
            Initialize();

        const string sql = @"DELETE FROM User WHERE UserID = @UserID AND RoomID = @RoomID";
        _connection!.Open();
        _connection.Execute(sql, new { UserID = userId, RoomID = roomId });
        _connection.Close();
    }

    public static void DeleteRoom(byte roomId)
    {
        if (_connection is null)
            Initialize();

        _connection!.Open();

        var users = _connection.Query<UserData>(@"SELECT * FROM User WHERE RoomID = @RoomID", new { RoomID = roomId });
        if (users.Any()) throw new Exception("number of users is not zero");
        const string sql = @"DELETE FROM Room WHERE RoomID = @RoomID";
        _connection.Execute(sql, new { RoomID = roomId });
        _connection.Close();
    }
}
using Cysharp.Threading.Tasks;
using Dapper;
using DotNetEnv;
using MySql.Data.MySqlClient;
using VIRCE_server.DataBase;

namespace VIRCE_server.Controller;

public static class MySqlController
{
    private static readonly string ConnectionString =
        $"Server={Env.GetString("MYSQL_HOST")};User ID=virce;Password=virce;Database=virce";

    public static async UniTask<IEnumerable<T>> Query<T>()
    {
        var connection = new MySqlConnection(ConnectionString);

        try
        {
            string sql;
            if (typeof(T) == typeof(UserData))
                sql = @"SELECT * FROM User";
            else if (typeof(T) == typeof(RoomServerInfo))
                sql = @"SELECT * FROM Room";
            else
                throw new Exception("Invalid Type");
            // waiting for connection to close when opened
            connection.Open();
            var result = await connection.QueryAsync<T>(sql);
            await connection.CloseAsync();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await connection.CloseAsync();
            throw;
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }

    public static async UniTask Insert<T>(T data)
    {
        var connection = new MySqlConnection(ConnectionString);
        try
        {
            string sql;
            if (typeof(T) == typeof(UserData) || typeof(T) == typeof(List<UserData>))
                sql =
                    @"INSERT INTO User (UserID, RoomID, ModelID, DisplayName, IPAddress, Port) VALUES (@UserID, @RoomID, @ModelID, @DisplayName, @IPAddress, @Port)";
            else if (typeof(T) == typeof(RoomServerInfo) || typeof(T) == typeof(List<RoomServerInfo>))
                sql = @"INSERT INTO Room (RoomID, RoomType) VALUES (@RoomID, @RoomType)";
            else
                throw new Exception("Invalid Type");

            connection.Open();
            await connection.ExecuteAsync(sql, data);
            await connection.CloseAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await connection.CloseAsync();
            throw;
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }

    public static async UniTask Update<T>(T data)
    {
        var connection = new MySqlConnection(ConnectionString);

        try
        {
            string sql;
            if (typeof(T) == typeof(UserData) || typeof(T) == typeof(List<UserData>))
                sql =
                    @"UPDATE User SET UserID = @UserID, RoomID = @RoomID, ModelID = @ModelID, DisplayName = @DisplayName, IPAddress = @IPAddress, Port = @Port WHERE UserID = @UserID AND RoomID = @RoomID";
            else if (typeof(T) == typeof(RoomServerInfo) || typeof(T) == typeof(List<RoomServerInfo>))
                sql = @"UPDATE Room SET RoomID = @RoomID, RoomType = @RoomType WHERE RoomID = @RoomID";
            else
                throw new Exception("Invalid Type");

            connection.Open();
            await connection.ExecuteAsync(sql, data);
            await connection.CloseAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await connection.CloseAsync();
            throw;
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }

    public static async UniTask DeleteUser(byte roomId, byte userId)
    {
        var connection = new MySqlConnection(ConnectionString);

        try
        {
            const string sql = @"DELETE FROM User WHERE UserID = @UserID AND RoomID = @RoomID";

            connection.Open();
            await connection.ExecuteAsync(sql, new { UserID = userId, RoomID = roomId });
            await connection.CloseAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await connection.CloseAsync();
            throw;
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }

    public static async UniTask DeleteRoom(byte roomId)
    {
        var connection = new MySqlConnection(ConnectionString);

        try
        {
            connection.Open();

            var users = connection.Query<UserData>(@"SELECT * FROM User WHERE RoomID = @RoomID",
                new { RoomID = roomId });
            if (!users.Any())
            {
                const string sql = @"DELETE FROM Room WHERE RoomID = @RoomID";
                await connection.ExecuteAsync(sql, new { RoomID = roomId });
            }

            await connection.CloseAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await connection.CloseAsync();
            throw;
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }
}
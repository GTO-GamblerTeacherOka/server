using System.Net;
using System.Text;
using Cysharp.Threading.Tasks;
using VIRCE_server.Controller;
using VIRCE_server.DataBase;
using VIRCE_server.PacketUtil;

namespace VIRCE_server.Server;

public static class Server
{
    private static bool _isRunning;

    public static async UniTask Run()
    {
        _isRunning = true;
        while (_isRunning)
        {
            var recvData = await Socket.ReceiveAsync();
            // 非同期で処理
            UniTask.Run(async () =>
            {
                byte[] sendData;
                var (header, body) = DataParser.Split(recvData.Buffer);
                var (flag, userId, roomId) = DataParser.AnalyzeHeader(header);
                switch (flag)
                {
                    case DataParser.Flag.RoomEntry:
                        var (isSuccess, userData) = await Matching.Request(recvData);
                        sendData = DataParser.CreateHeader(DataParser.Flag.RoomEntry, userData.UserID, userData.RoomID);
                        Socket.SendAsync(sendData, recvData.RemoteEndPoint).Forget();
                        return;
                    case DataParser.Flag.PositionData:
                        break;
                    case DataParser.Flag.AvatarData:
                        var user = MySqlController.Query<UserData>()
                            .First(u => u.UserID == userId && u.RoomID == roomId);
                        user.ModelID = Encoding.UTF8.GetString(body);
                        MySqlController.Update(user);
                        break;
                    case DataParser.Flag.RoomExit:
                        ExitHandler(roomId, userId).Forget();
                        break;
                    case DataParser.Flag.Reaction:
                    case DataParser.Flag.ChatData:
                        break;
                    case DataParser.Flag.DisplayNameData:
                        var user2 = MySqlController.Query<UserData>()
                            .First(u => u.UserID == userId && u.RoomID == roomId);
                        user2.DisplayName = Encoding.UTF8.GetString(body);
                        MySqlController.Update(user2);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var users = MySqlController.Query<UserData>().Where(data => data.RoomID == roomId);
                foreach (var u in users)
                {
                    var sendEndPoint = new IPEndPoint(IPAddress.Parse(u.IPAddress), u.Port);
                    Socket.SendAsync(recvData.Buffer, sendEndPoint).Forget();
                }
            });
        }
    }

    private static async UniTask ExitHandler(byte roomId, byte userId)
    {
        await UniTask.Run(() =>
        {
            MySqlController.DeleteUser(roomId, userId);
            var users = MySqlController.Query<UserData>().Where(data => data.RoomID == roomId);
            if (users.Any()) MySqlController.DeleteRoom(roomId);
        });
    }

    public static void Stop()
    {
        _isRunning = false;
    }
}
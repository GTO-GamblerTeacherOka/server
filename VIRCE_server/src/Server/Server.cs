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
                    case DataParser.Flag.AvatarData:
                        var user = MySqlController.Query<UserData>()
                            .First(u => u.UserID == userId && u.RoomID == roomId);
                        user.ModelID = Encoding.UTF8.GetString(body);
                        MySqlController.Update(user);
                        break;
                    case DataParser.Flag.RoomExit:
                    case DataParser.Flag.Reaction:
                    case DataParser.Flag.ChatData:
                        break;
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

    public static void Stop()
    {
        _isRunning = false;
    }
}
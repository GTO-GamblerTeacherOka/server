using System.Net.Sockets;
using System.Text;
using Cysharp.Threading.Tasks;
using VIRCE_server.Controller;
using VIRCE_server.DataBase;
using VIRCE_server.PacketUtil;

namespace VIRCE_server.Server;

public static class Matching
{
    public static async UniTask<(bool success, UserData user)> Request(UdpReceiveResult recvData)
    {
        var (_, body) = DataParser.Split(recvData.Buffer);
        var waitTime = 2000;
        byte userId = 0;
        byte roomId = 0;
        while (!RedisController.SetNx("matching")) // マッチングの権限を取得
        {
            await Task.Delay(waitTime);
            waitTime /= 2;
        }

        var rooms = MySqlController.Query<RoomServerInfo>().ToArray();
        var users = MySqlController.Query<UserData>().ToArray();

        var minUserRoom = rooms.Min(room => users.Count(user => user.RoomID == room.RoomID));
        roomId = rooms.First(room => users.Count(user => user.RoomID == room.RoomID) == minUserRoom).RoomID;
        var minUserRoomUsers = users.Where(user => user.RoomID == roomId).ToArray();

        if (minUserRoomUsers.Length > 31)
        {
            byte r = 1;
            while (minUserRoomUsers.Any(user => user.UserID == r))
                r++;
            roomId = r;
            minUserRoomUsers = Array.Empty<UserData>();
        }

        for (byte i = 1; i <= 31; i++)
        {
            if (minUserRoomUsers.Any(user => user.UserID == i)) continue;
            userId = i;
            break;
        }

        var modelId = Encoding.UTF8.GetString(body);
        var user = new UserData
        {
            DisplayName = "", IPAddress = recvData.RemoteEndPoint.Address.ToString(), ModelID = modelId,
            Port = (ushort)recvData.RemoteEndPoint.Port, RoomID = roomId, UserID = userId
        };
        MySqlController.Insert(user);

        RedisController.Remove("matching"); // マッチングの権限を削除
        return (true, user);
    }
}
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
        byte roomId;
        while (!RedisController.SetNx("matching")) // マッチングの権限を取得
        {
            await Task.Delay(waitTime);
            waitTime /= 2;
        }

        RedisController.SetString("matching", "1");

        var rooms = (await MySqlController.Query<RoomServerInfo>()).OrderBy(room => room.RoomID).ToArray(); // roomの配列
        var users = (await MySqlController.Query<UserData>()).ToArray(); // userの配列

        if (rooms.Length == 0)
        {
            roomId = 1;
            var roomInfo = new RoomServerInfo { RoomID = roomId, RoomType = RoomServerInfo.ServerType.Lobby };
            await MySqlController.Insert(roomInfo);
            userId = 1;
        }
        else
        {
            var minUserRoom = rooms.Min(room => users.Count(user => user.RoomID == room.RoomID));
            var cheapRoom = rooms.First(room => users.Count(user => user.RoomID == room.RoomID) == minUserRoom);
            roomId = cheapRoom.RoomID;
            var minUserRoomUserIds = users.Where(user => user.RoomID == roomId).Select(user => user.UserID).ToArray();

            if (minUserRoomUserIds.Length > 31)
            {
                byte r = 1;
                foreach (var room in rooms)
                    if (room.RoomID == r) r++;
                    else break;
                roomId = r;
                var roomInfo = new RoomServerInfo { RoomID = roomId, RoomType = RoomServerInfo.ServerType.Lobby };
                await MySqlController.Insert(roomInfo);
                minUserRoomUserIds = Array.Empty<byte>();
            }

            for (byte i = 1; i <= 31; i++)
            {
                if (minUserRoomUserIds.Any(user => user == i)) continue;
                userId = i;
                break;
            }
        }

        var modelId = Encoding.UTF8.GetString(body[1..]);
        var user = new UserData
        {
            DisplayName = "", IPAddress = recvData.RemoteEndPoint.Address.ToString(), ModelID = modelId,
            Port = (ushort)recvData.RemoteEndPoint.Port, RoomID = roomId, UserID = userId
        };
        await MySqlController.Insert(user);

        RedisController.Remove("matching"); // マッチングの権限を削除
        return (true, user);
    }
}
﻿using System.Net;
using System.Net.Sockets;
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
        if (_isRunning) return;
        _isRunning = true;
        while (_isRunning)
        {
            var recvData = await Socket.ReceiveAsync();
            ReceiveHandler(recvData).Forget();
        }
    }

    private static async UniTask ReceiveHandler(UdpReceiveResult recvData)
    {
        var (header, body) = DataParser.Split(recvData.Buffer);
        var (flag, userId, roomId) = DataParser.AnalyzeHeader(header);
        switch (flag)
        {
            case DataParser.Flag.RoomEntry:
                var (isSuccess, userData) = await Matching.Request(recvData);
                roomId = userData.RoomID;
                userId = userData.UserID;
                header = DataParser.CreateHeader(DataParser.Flag.RoomEntry, userId, roomId);
                var entryData = DataParser.CreateHeader(DataParser.Flag.RoomEntry, userId, roomId);
                Socket.SendAsync(entryData, recvData.RemoteEndPoint).Forget();

                var sendData = header.Concat(body).ToArray();

                var roomUsers = MySqlController.Query<UserData>().Where(data => data.RoomID == userData.RoomID)
                    .Where(data => data.UserID != userData.UserID);
                foreach (var u in roomUsers)
                {
                    var h = DataParser.CreateHeader(DataParser.Flag.AvatarData, u.UserID, u.RoomID);
                    var b = Encoding.UTF8.GetBytes(u.ModelID);
                    var data = h.Concat(b).ToArray();
                    Socket.SendAsync(data, recvData.RemoteEndPoint).Forget(); // send user data to new user
                    Socket.SendAsync(sendData, new IPEndPoint(IPAddress.Parse(u.IPAddress), u.Port)).Forget();
                }

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
                break;
            case DataParser.Flag.ChatData:
                break;
            case DataParser.Flag.DisplayNameData:
                var user2 = MySqlController.Query<UserData>()
                    .First(u => u.UserID == userId && u.RoomID == roomId);
                user2.DisplayName = Encoding.UTF8.GetString(body);
                MySqlController.Update(user2);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(recvData));
        }

        var users = MySqlController.Query<UserData>().Where(data => data.RoomID == roomId);
        foreach (var u in users)
        {
            var sendEndPoint = new IPEndPoint(IPAddress.Parse(u.IPAddress), u.Port);
            Socket.SendAsync(recvData.Buffer, sendEndPoint).Forget();
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
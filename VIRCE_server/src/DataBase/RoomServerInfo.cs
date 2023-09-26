using MasterMemory;
using MessagePack;

namespace VIRCE_server.DataBase;

[MemoryTable("RoomServerInfo")]
[MessagePackObject(true)]
public class RoomServerInfo
{
    public enum ServerType
    {
        Lobby = 0,
        MiniGame = 1
    }

    [PrimaryKey] public byte RoomID { get; set; }

    public ServerType RoomType { get; set; }
}
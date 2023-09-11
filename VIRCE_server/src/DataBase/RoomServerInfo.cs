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

    [PrimaryKey] public byte RoomId { get; set; }

    public ServerType Type { get; set; }
}
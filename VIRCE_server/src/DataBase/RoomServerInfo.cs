using MasterMemory;
using MessagePack;
namespace VIRCE_server.DataBase;

[MemoryTable("RoomServerInfo"), MessagePackObject(true)]
public class RoomServerInfo
{
    public enum ServerType
    {
        Lobby = 0,
        MiniGame = 1
    }
    
    [PrimaryKey] 
    public int RoomId { get; set; }
    
    [SecondaryKey(0)]
    public int Port { get; set; }
    
    public ushort[] UserIds { get; set; } = null!;

    public ServerType Type { get; set; }
}

using MasterMemory;
using MessagePack;
namespace VIRCE_server.DataBase;

[MemoryTable("RoomServerInfo"), MessagePackObject(true)]
public class RoomServerInfo
{
    [PrimaryKey] 
    public int RoomId { get; set; }
    
    [SecondaryKey(0)]
    public int Port { get; set; }
    
    public ushort[] UserIds { get; set; }
}

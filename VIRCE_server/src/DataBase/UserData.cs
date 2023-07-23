using System.Net;
using MasterMemory;
using MessagePack;

namespace VIRCE_server.DataBase;

[MemoryTable("UserData"), MessagePackObject(true)]
public class UserData
{
    [PrimaryKey] public int GlobalUserId => UserId | RoomId << 5;
    
    [SecondaryKey(0), NonUnique]
    public ushort UserId { get; set; }
    
    [SecondaryKey(1), NonUnique]
    public int RoomId { get; set; }
    
    public string ModelId { get; set; }
    
    public string Name { get; set; }
    
    public IPEndPoint RemoteEndPoint { get; set; }
}

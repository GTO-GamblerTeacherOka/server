using MasterMemory;
using MessagePack;

namespace VIRCE_server.DataBase;

[MemoryTable("UserData")]
[MessagePackObject(true)]
public class UserData
{
    [PrimaryKey] public ushort GlobalUserId => (ushort)(UserID | (RoomID << 5));

    [SecondaryKey(0) , NonUnique]
    public byte UserID { get; set; }

    [SecondaryKey(1) , NonUnique] 
    public byte RoomID { get; set; }

    public string ModelID { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string IPAddress { get; set; } = string.Empty;

    public ushort Port { get; set; }
}
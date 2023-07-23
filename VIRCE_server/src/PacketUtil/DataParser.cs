namespace VIRCE_server.PacketUtil;

public static class DataParser
{
    public enum Flag
    {
        PositionData = 0,
        AvatarData = 1,
        RoomEntry = 2,
        RoomExit = 3,
        SendReaction = 4,
        ReceiveReaction = 5,
        ChatData = 7,
        GetRemoteEndPoint = 8,
        ReceiveRemoteEndPoint = 9,
    }
    
    private const int HeaderSize = 2;
    
    public static (byte[] header, byte[] body) Split(in byte[] data)
    {
        return (data[..HeaderSize], data[HeaderSize..]);
    }
    
    public static (Flag flag, ushort userID, ushort roomID) AnalyzeHeader(in byte[] header)
    {
        var flag = (Flag)(header[0] & 0b0000_1111);
        var uid = (ushort)((header[0] & 0b1111_0000) >> 4 | (header[1] & 0b0000_0001) << 4);
        var rid = (ushort)(header[1] & 0b1111_1110 >> 1);
        return (flag, uid, rid);
    }
    
    public static int GetGlobalUserId(in ushort userId, in ushort roomId)
    {
        return userId | roomId << 5;
    }
}

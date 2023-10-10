namespace VIRCE_server.PacketUtil;

public static class DataParser
{
    public enum Flag
    {
        PositionData = 0,
        AvatarData = 1,
        RoomEntry = 2,
        RoomExit = 3,
        Reaction = 4,
        ChatData = 7,
        DisplayNameData = 8
    }

    private const int HeaderSize = 2;

    public static (byte[] header, byte[] body) Split(in byte[] data)
    {
        return (data[..HeaderSize], data[HeaderSize..]);
    }

    public static (Flag flag, byte userID, byte roomID) AnalyzeHeader(in byte[] header)
    {
        var flag = (Flag)(header[0] & 0b0000_1111);
        var uid = (byte)(((header[0] & 0b1111_0000) >> 4) | ((header[1] & 0b0000_0001) << 4));
        var rid = (byte)((header[1] & 0b1111_1110) >> 1);
        return (flag, uid, rid);
    }

    public static ushort GetGlobalUserId(in byte userId, in byte roomId)
    {
        return (ushort)(userId | (roomId << 5));
    }

    public static byte[] CreateHeader(in Flag flag, in byte userId, in byte roomId)
    {
        var data = new byte[2];
        data[0] = (byte)((byte)flag | ((userId & 0b0000_1111) << 4));
        data[1] = (byte)(((userId & 0b0001_0000) >> 4) | ((roomId & 0b0111_1111) << 1));
        return data;
    }
}
using System.IO;


public static class gameExt
{
    public static void Write(this BinaryWriter writer, Side side)
    {
        writer.Write((sbyte) side.Sign);
    }
    
    
    public static Side ReadSide(this BinaryReader reader)
    {
        return new Side(reader.ReadSByte());
    }
}

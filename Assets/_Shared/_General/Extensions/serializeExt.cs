using System.IO;
using UnityEngine;


public static class serializeExt 
{
    public static void Write(this BinaryWriter writer, Vector2 vector)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
    }
    
    public static Vector2 ReadVector2(this BinaryReader reader)
    {
        return new Vector2(reader.ReadSingle(), reader.ReadSingle());
    }
}

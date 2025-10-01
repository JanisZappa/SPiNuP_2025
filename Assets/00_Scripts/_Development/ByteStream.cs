using System.IO;


public static class ByteStream
{
	public static readonly MemoryStream stream = new MemoryStream(new byte[102400]);
	public static readonly BinaryReader reader = new BinaryReader(stream);
	public static readonly BinaryWriter writer = new BinaryWriter(stream);
	
	
	public static void Set(byte[] bytes, int start, int end)
	{
		stream.SetLength(0);

		int length = end - start;
		for (int i = 0; i < length; i++)
			writer.Write(bytes[i + start]);
	}
	
	
	public static void Set(byte[] bytes, int length)
	{
		stream.SetLength(0);

		for (int i = 0; i < length; i++)
			writer.Write(bytes[i]);

		stream.Position = 0;
	}
	
	
	public static void Set(byte[] bytes)
	{
		stream.SetLength(0);

		writer.Write(bytes);

		stream.Position = 0;
	}
}

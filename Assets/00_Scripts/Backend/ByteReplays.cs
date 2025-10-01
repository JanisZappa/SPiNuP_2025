using System;
using System.IO;
using Anim;
using Clips;
using UnityEngine;


public class ByteReplays : Singleton<ByteReplays>
{
	public ByteReplay[] replays;
	private const int howMany = 3;

	public static int Count { private set; get; }

	
	public static void ResetCount()
	{
		Count = 0;
	}
	
	
	public static bool AddReplay()
	{
		if (Count < Inst.replays.Length)
		{
			Inst.replays[Count++].ParseBytes(ByteStream.reader);
			return true;
		}

		return false;
	}


	public static void GameUpdate()
	{
		if(GameManager.Running && Input.GetKeyDown(KeyCode.H))
			LoadRandomReplayForAll();	
	}


	private static void LoadRandomReplayForAll()
	{
		for (int i = 0; i < howMany; i++)
			LoadRandomReplay(Spinner.Get(i + 1));
	}
	
	
	public static void LoadRandomReplayFor(Spinner spinner)
	{
		LoadRandomReplay(spinner);
	}
	
	
	private static void LoadRandomReplay(Spinner spinner)
	{
		spinner.Disable();

		int activeCount = Spinner.active.Count;
		while (true)
		{
			ByteReplay replay = Inst.replays[UnityEngine.Random.Range(0, Count)];

			bool isFine = true;
			for (int i = 0; i < activeCount; i++)
				if (Spinner.active[i].name == replay.charName)
				{
					isFine = false;
				}

			if (isFine)
			{
				spinner.Enable(replay);
				break;
			}
		}
	}
}


[Serializable]
public class ByteReplay
{
	public string charName, hex;
	public int score, clipCount, byteCount;
	
	[Space(5)]
	public Costume costume;
	public Vector3 charSize;
	
	private readonly byte[] bytes = new byte[10240];
	

	public static byte[] Serialize(int clipCount)
	{
		Spinner spinner = Spinner.GetPlayer;
		
		MemoryStream m = new MemoryStream();
		using (BinaryWriter writer = new BinaryWriter(m))
		{
			writer.Write(ReplaySaveLoad.IDHex);
			writer.Write(spinner.name);
			writer.Write((ushort) Score.PlayerOneScore);
				
		//  Char Info  //
			Costume.Serialize(spinner.costume, writer);
				
			writer.Write(spinner.size.x);
			writer.Write(spinner.size.y);
			writer.Write(spinner.size.z);
			
		//  ClipCount  //
			writer.Write((ushort)clipCount);
			
			m.Dispose();        
		}

		return m.ToArray();
	}
	
	
	public void ParseBytes(BinaryReader reader, bool debug = false)
	{
		hex      = reader.ReadString();
		charName = reader.ReadString();
		score    = reader.ReadUInt16();
		
	//  Char Info  //
		costume   = Costume.Deserialize(reader);
		charSize  = new Vector3 (reader.ReadSingle(), 
			                     reader.ReadSingle(), 
			                     reader.ReadSingle());
	//  ClipCount  //
		clipCount = reader.ReadUInt16();
		
		if(debug)
			Debug.Log(("Name: "       + charName).PadRight(20) + 
			          ("|Score: "     + score.ToString().PadLeft(3)).PadRight(15) + 
			          ("|ClipCount: " + clipCount.ToString().PadLeft(3)).PadRight(20) + 
			           "|Hex: "       + hex);

		long startPos = reader.BaseStream.Position;
		for (int i = 0; i < clipCount; i++)
		{
			ClipType type = (ClipType) Mathf.Abs(reader.ReadSByte());
			Clip.SkipClip(reader, type);
		}

		byteCount = (int)(reader.BaseStream.Position - startPos);

		reader.BaseStream.Position = startPos;
		for (int i = 0; i < byteCount; i++)
			bytes[i] = reader.ReadByte();
	}


	public void DeserializeClips(Spinner spinner, bool shift = false)
	{
		ByteStream.Set(bytes, byteCount);

		float timeShift = 0;
		if (shift)
		{
		//  Skip SpawnClip  //
			ByteStream.stream.Position = 1;
			Clip.SkipClip(ByteStream.reader, ClipType.Spawn);
			
		//  First Action  //
			sbyte    typeAndSide = ByteStream.reader.ReadSByte();
			ClipType clipType    = (ClipType) Mathf.Abs(typeAndSide);
			Side     side        = new Side(typeAndSide < 0);
	
			Clip clip = Clip.Deserialize(ByteStream.reader, spinner, clipType, side);
			if (clip != null)
			{
				float start = clip.startTime;
				
				while (true)
				{
					if (start + timeShift < GTime.Now)
						timeShift += GTime.LoopTime;
					else
						break;
				}
				
				clip.Reset();
			}

			ByteStream.stream.Position = 0;
		}
		
		
		Tape tape = spinner.tape;
		tape.Clear();
		for(int i = 0; i < clipCount; i++)
		{
			sbyte    typeAndSide = ByteStream.reader.ReadSByte();
			ClipType clipType    = (ClipType) Mathf.Abs(typeAndSide);
			Side     side        = new Side(typeAndSide < 0);

			Clip clip = Clip.Deserialize(ByteStream.reader, spinner, clipType, side, timeShift);

			if (clip != null)
			{
				tape.SetClip(clip);
				if(i == 0)
					spinner.startTime = clip.startTime;
			}
			else
			{
				tape.Clear();
				return;
			}
		}
	}
}

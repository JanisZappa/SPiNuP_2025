using System;
using System.IO;
using Clips;
using UnityEngine;


public partial class Tape
{
    public Tape(bool createSerializer)
    {
        if (createSerializer)
            tapeSerializer = new TapeSerializer();
    }

    private const int arrayLength = 50;
    
    private readonly Clip[] clips = new Clip[arrayLength];
    private int min, max;
    private int first  => min % arrayLength;
    private int last   => (max - 1) % arrayLength;
    private int length => max - min;

    private readonly TapeSerializer tapeSerializer;

    
    public void Clear()
    {
        while (length > 0)
        {
            clips[last].Reset();
            max--;
        }

        tapeSerializer?.Reset();
    }

    
    public Clip GetClip(float time)
    {
        int index = -1;
        for (int i = min; i < max; i++)
        {
            int modIndex = i % arrayLength;
            
            if (clips[modIndex].startTime <= time)
                index = modIndex;
            else
                break;
        }
            
        if (index == -1)
            return None.Get;

        if (length > 0 && index == last)
        {
            Clip lastClip = clips[index];

            if (lastClip.duration > 0 && time >= lastClip.EndTime)
            {
                Debug.Log("I don't get it".B_Green());
                return None.Get;
            }  
        }

        return clips[index];
    }
    
    
    public void SetClip(Clip newClip)
    {
        AddToList(newClip);
        
        if (newClip.Type.IsAnyJump())
            AddToList(Clip.GetJumpEvent((Jump)newClip));
    }
    
    
    private void AddToList(Clip newClip)
    {
        if(newClip == null)
            return;
        
        if (length == arrayLength)
        {
            Debug.Log("Damn Tape clips too short");
            return;
        }
        
        Clip before = length > 0 ? clips[last] : null;
        newClip.SetClipBefore(before);
        before?.SetClipAfter(newClip);

        max++;
        clips[last] = newClip;
    }
    
    
    public void Trimm(float time, bool after = true)
    {
        if (after)
        {
            while (length > 0 && clips[last].startTime >= time)
            {
                clips[last].Reset();
                max--;
            }

            if (length > 0)
                clips[last].SetClipAfter(null);
        }
        else
        {
            while (length > 3)
            {
                Clip thirdClip = clips[(min + 2) % arrayLength];
                if (thirdClip.startTime + thirdClip.duration < time)
                {
                    SerializeFirst();
                    
                    if (length > 0)
                        clips[first].before = null;
                
                    continue;
                }

                break;
            }
        }
    }


    public byte[] GetReplayBytes()
    {
        while (length > 0)
            SerializeFirst();
        
        return tapeSerializer.GetBytes();
    }
    
    
    private void SerializeFirst()
    {
        Clip firstClip = clips[first];
        tapeSerializer?.SerializeClip(firstClip);
        firstClip.Reset();
        min++;
    }
}


public partial class Tape
{
    private class TapeSerializer
    {
        private int                   clipCount;
        private readonly MemoryStream clipStream;
        private readonly BinaryWriter writer;
        
        public TapeSerializer()
        {
         clipStream = new MemoryStream(10240);
         writer     = new BinaryWriter(clipStream);
        }
        
        
        public void Reset()
        {
            clipCount = 0;
            clipStream.SetLength(0);
        }


        public void SerializeClip(Clip clip)
        {
            if (!clip.Type.IsSerializable())
                return;
            
            writer.Write((sbyte) ((int) clip.Type * clip.startSide.Sign));
            clip.Serialize(writer);
            clipCount++;
        }


        public byte[] GetBytes()
        {
            byte[] infoArray   = ByteReplay.Serialize(clipCount);
            byte[] clipBytes   = clipStream.ToArray();
            byte[] returnBytes = new byte[infoArray.Length + clipBytes.Length];
            Buffer.BlockCopy(infoArray, 0, returnBytes, 0, infoArray.Length);
            Buffer.BlockCopy(clipBytes, 0, returnBytes, infoArray.Length, clipBytes.Length);

            return returnBytes;
        }
    }
}
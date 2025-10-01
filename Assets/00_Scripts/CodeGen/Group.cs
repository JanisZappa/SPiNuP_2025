using System.Collections.Generic;
using LevelElements;
using UnityEngine;


public struct elementTypeComparer : IEqualityComparer<elementType>
{
    public bool Equals(elementType x, elementType y)
    {
        return x == y;
    }

    public int GetHashCode(elementType obj)
    {
        return (int)obj;
    }
}


public class Group
{
    private readonly elementType[] elements;
    private readonly Dictionary<elementType, int> pos = new Dictionary<elementType, int>(new elementTypeComparer());
    public readonly int Length;

    public Group(elementType[] elements)
    {
        this.elements = elements;
        Length = elements.Length;
        
        for (int i = 0; i < Length; i++)
            pos.Add(elements[i], i);
    }
    
    public elementType Next(elementType value) { return Get((Pos(value) + 1).Repeat(Length)); }
    
    public elementType Prev(elementType value) { return Get((Pos(value) - 1).Repeat(Length)); }
    
    public elementType GetRandom { get { return elements[Random.Range(0, Length)]; }}
    
    public elementType Get(int index)
    {
        return elements[index];
    }

    private int Pos(elementType value)
    {
        return pos[value];
    }

    public bool Contains(elementType value)
    {
        return pos.ContainsKey(value);
    }
}


public static class AliasTest
{
    public static Vector2 Anim(this elementType elementType, float time, int ID, Vector2 pos)
    {
        switch (elementType)
        {
            default:
                return pos;
            
            case elementType.Probe: 
                return new Vector2(pos.x, Mth.SmoothPP(-.5f, .5f, (time + ID * .115f) / GTime.LoopTime * 8) + pos.y);
            
            case elementType.Probe_Orange: 
                return new Vector2(pos.x, Mth.SmoothPP(-.5f, .5f, (time + ID * .115f) / GTime.LoopTime * 9) + pos.y);
            
            case elementType.Coin: 
                Vector2 rot = Rot.Z((time + ID * 12.13f) * 360 * 4 / GTime.LoopTime) * new Vector2(0, .1f);
                return new Vector2(pos.x + rot.x, pos.y + rot.y);
        }
    }
}


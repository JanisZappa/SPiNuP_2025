using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public abstract class PoolObject : IEquatable<PoolObject>
{
	private static int poolIndex;

	protected PoolObject()
	{
		poolID = poolIndex++;
	}

	
	public readonly int poolID;

	public bool Equals(PoolObject other)
	{
		return other != null && poolID == other.poolID;
	}

	public override bool Equals(object obj)
	{
		return obj != null && ((PoolObject) obj).poolID == poolID;
	}

	public override int GetHashCode()
	{
		return poolID;
	}
}


public class Pool<T> : IPoolInfo where T : PoolObject
{
	private readonly string name;
	private int size;

	public T[] all; 
	private readonly Stack<T> inactive;

	private readonly Func<T> _creator;


	public int ActiveElementCount { get { return size - inactive.Count; }}
	
    
	public Pool(Func<T> instanceCreator, int size)
	{
		_creator  = instanceCreator;
		this.size = size;
        
		inactive = new Stack<T>(size);
        all      = new T[size];
		
		for (int i = 0; i < size; i++)
		{
			T thing = _creator.Invoke();
			
			inactive.Push(thing);
			all[i] = thing;
		}
		
		name = "• " + all[0].ToString().Split('.').Last();
        
		PoolInfo.infos.Add(this);
	}
    
    
	public T GetFree()
	{
		T thing;
		if (inactive.Count > 0)
			thing = inactive.Pop();
		else
		{
		//	Create new Thing
			Debug.LogFormat("\"{0}\"-Pool needs to be resized! Current Size: {1}", name, size);
			thing = _creator.Invoke();
			
			Array.Resize(ref all, 1);
			size++;
			all[size - 1] = thing;
		}
		
		return thing;
	}
	
    
	public void Reset()
	{
		inactive.Clear();
		
		int count = all.Length;
		for (int i = 0; i < count; i++)
			inactive.Push(all[i]);
	}


	public void Return(T thing)
	{
		inactive.Push(thing);
	}
	
	
	public void AddDebugInfo(StringBuilder stringBuilder, bool last)
	{
		if (AlmostAtCapacity)
			stringBuilder.Append(FancyString.B_Start("yellow"));
		
		stringBuilder.
			Append(name.PadRight(18)).
			Append(ActiveElementCount.PrepString().PadLeft(6)).
			Append(" / ").
			Append(size.PrepString().PadRight(6)).
			Append(" kb");
        
		if(!last)
			stringBuilder.Append("\n");
		
		if (AlmostAtCapacity)
			stringBuilder.Append(FancyString.B_End);
	}
    
    
	public bool AlmostAtCapacity { get { return (float) ActiveElementCount / size > .9f; } }
}


public interface IPoolInfo
{
	void AddDebugInfo(StringBuilder stringbuilder, bool last);
	bool AlmostAtCapacity { get; }
}


public static class PoolInfo
{
	public static readonly List<IPoolInfo> infos = new List<IPoolInfo>();
}
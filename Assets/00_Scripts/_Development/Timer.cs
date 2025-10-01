using System;
using System.Text;
using UnityEngine;


public static class Timer 
{
    public enum Entry
    {
        CreateRects,
        DetectOccupiedCells,
        Piecer,
        Occlusion,
        MakePillars,
        TellLevelAboutOcclusion,
        FindUsedCells
    }
    
    
    private static readonly float[] times;
    
    private static readonly StringBuilder builder;

    
    public static void Start(Entry entry)
    {
        times[(int) entry] = Time.realtimeSinceStartup;
    }
    
    
    public static void End(Entry entry)
    {
        times[(int) entry] = Time.realtimeSinceStartup - times[(int) entry];
    }

    
    public static float Get(Entry entry)
    {
        return times[(int) entry];
    }

    
    public static string Log(Entry entry)
    {
        builder.Length = 0;
        return builder.Append(entry.ToString()).Append(" : ").Append(times[(int) entry]).Append(" | ").ToString();
    }
    
    
    public static string Log(Entry entry, ref float add)
    {
        builder.Length = 0;

        float value = times[(int) entry];
        add += value;
        
        return builder.Append(value.ToString("F4")).Append(" : ").Append(entry.ToString()).ToString();
    }
    
    
    static Timer()
    {
        builder = new StringBuilder(100);
        times   = new float[Enum.GetValues(typeof(Entry)).Length];
    }
}

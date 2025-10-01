using UnityEngine;


public abstract class WrapList<T> where T : ITimeSlice
{
    private int arrayLength = 40;
    public T[] all; 
    public int min, max;
    private int first  { get { return min % arrayLength; }}
    private int last   { get { return (max - 1) % arrayLength; }}
    private int length { get { return max - min; }}
    
    
    public void Add(T newThing)
    {
        if (length == arrayLength)
        {
            Debug.LogFormat("Wraplist of {0} too short", typeof(T));
            return;
        }
                
        max++;
        all[last] = newThing;
    }

    
    public void ResetAll()
    {
        while (length > 0)
        {
            all[last].Reset();
            max--;
        }
    }

    public void ClearAfter(float time)
    {
        while (length > 0 && all[last].StartTime >= time)
        {
            all[last].Reset();
            max--;
        }
    }
            
    public void ClearBefore(float time)
    {
        while (length > 0 && all[first].EndTime < time)
        {
            all[first].Reset();
            min++;
        }
    }
    
    
    public T GetAt(float time, T None)
    {
        int index = -1;
        for (int i = min; i < max; i++)
        {
            int modIndex = i % arrayLength;
            
            if (all[modIndex].StartTime <= time)
                index = modIndex;
            else
                break;
        }
            

        if (index == -1)
            return None;

        /*if (length > 0 && index == last)
        {
            T lastT = all[index];
            
            if(lastT.duration > 0 && time >= lastT.EndTime)
                return None;
        }*/

        return all[index];
    }
}


public interface ITimeSlice
{
    float StartTime {get; }
    float EndTime   {get; }
    //bool IsValid(float time);
    void Reset();
}

using UnityEngine;


public partial class Spinner
{
    public class Squashes
    {
        private const int arrayLength = 40;
            
        private readonly SquashAnim[] squashes = new SquashAnim[arrayLength];
        private int min, max;
        private int first  { get { return min % arrayLength; }}
        private int last   { get { return (max - 1) % arrayLength; }}
        private int length { get { return max - min; }}
            
            
        public void AddSquash(float time, float amount, float frequency, float duration)
        {
            if (length == arrayLength)
            {
                Debug.Log("Damn squashes too short");
                return;
            }
                
            max++;
            squashes[last] = SquashAnim.GetNew(time, amount, frequency, duration);
        }
            
        public float GetSquash(float time)
        {
            return 1;
            float squash = 1;
            for (int i = min; i < max; i++)
                squash += squashes[i % arrayLength].GetSquash(time);
                
            return squash;
        }

        public void ResetAll()
        {
            while (length > 0)
            {
                squashes[last].Reset();
                max--;
            }
        }

        public void Trimm(float time, bool after = true)
        {
            if (after)
            {
                while (length > 0 && squashes[last].startTime >= time)
                {
                    squashes[last].Reset();
                    max--;
                }
            }
            else
            {
                while (length > 0 && squashes[first].EndTime < time)
                {
                    squashes[first].Reset();
                    min++;
                }
            }
        }
    }
}


public class SquashAnim : PoolObject, ITimeSlice
{
    static SquashAnim()
    {
        squashPool = new Pool<SquashAnim>(() => new SquashAnim(), 500);
    }
    
    private static readonly Pool<SquashAnim> squashPool;
    
    
    public static SquashAnim GetNew(float startTime, float amount, float frequency, float duration)
    {
        return squashPool.GetFree().Setup(startTime, amount, frequency, duration);
    }
    
    
    public  float startTime;
    private float amount, frequency, duration;
    
    public float StartTime { get { return startTime; }}
    public float EndTime   { get { return startTime + duration; }}


    private SquashAnim Setup(float startTime, float amount, float frequency, float duration)
    {
        this.startTime = startTime;
        this.frequency = frequency;
        this.duration  = duration;
        this.amount    = amount;
        
        return this;
    }
    
    
    public void Reset()
    {
        squashPool.Return(this);
    }
    
    
    public float GetSquash(float time)
    {
        if (time < startTime || time - startTime > duration)
            return 0;

        return GPhysics.NewOscillate(time - startTime, frequency, duration) * amount;
    }   
}
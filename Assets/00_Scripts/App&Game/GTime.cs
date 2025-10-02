using UnityEngine;


public static class GTime 
{
    private const float BPM       = 132f;
    private const float BPS       = BPM / 60f;
    private const float Beat      = 1f / BPS;
    public  const float LoopTime  = 16 * Beat;
    public  const float LoopMulti = 1 / LoopTime;
    
    public  const float RewindTime = LoopTime;
    
    public static float Now;
    public static float LastFrame;
    
    public  static float Speed = 1;
    private static float pausedTime;
    
    private const float slowMoFactor     = 1f / 4;
    private const float megaSlowMoFactor = 1f / 16;

    private static bool resetLoop;

    public static float StartTime;


    public static bool Paused;
    private static float LoopCount    { get { return Now / LoopTime; }}
    public static float LoopFraction  { get { return LoopCount % 1;  }}

    public static float LastLoopPoint { get { return Mathf.Floor(LoopCount) * LoopTime;  }}
    public static float NextLoopPoint { get { return Mathf.Ceil(LoopCount) * LoopTime;  }}

    public delegate void  OnPaused(bool paused);
    public static   event OnPaused onPaused;

    
    private static void CheckKeyboardInput()
    {
        if (!Paused && Input.GetKeyDown(KeyCode.Tab))
            Speed = f.Same(Speed, 1) ? 0 : 1;
        
        if (!Paused && Input.GetKeyDown(KeyCode.LeftShift))
            Speed = f.Same(Speed, 1) ? slowMoFactor : f.Same(Speed, slowMoFactor) ? megaSlowMoFactor : 1;

        if (!GameManager.IsCreator)
        {
            if(!Paused && KeyMap.Down(Key.Pause))
                SetPause(true);
            else
            if(Paused && KeyMap.Down(Key.Unpause))
                SetPause(false);
        }
    }

    
    public static void TimeStep()
    {
        if(!Application.isMobilePlatform)
            CheckKeyboardInput();
        
        if (GameManager.Running)
        {
            LastFrame = Now;
            
            Now += Time.deltaTime * Speed;
            if (Now < 0)
                Now = 0;
            
            if (resetLoop)
                Now = Mathf.Repeat(Now, LoopTime);
        }
    }

    
    public static float StartNewGame_GetTime()
    {
        float before = Now;
        
        StartTime = Now = ServerTime.GetGameStartTime();
        Speed     = 1;

        return Now - before;
    }
    

    public static void SetPause(bool pauseIt)
    {
        Paused = pauseIt;
        
        if (Paused)
            pausedTime = Now;
        else
            Now = pausedTime;
        
        Speed = Paused? 0 : 1;
        onPaused?.Invoke(Paused);
        SetVolume.UpdateVolume();
    }

    
    /*public static void ResetLoop(bool resetLoop)
    {
        GTime.resetLoop = resetLoop;
    }

    
    public static void SlowMoPlease(float howLong)
    {
        if (waitTime > howLong)
            return;

        if (waitTime > 0)
        {
            waitTime = howLong;
            return;
        }

        waitTime = howLong;
        Run.Inst.StartCoroutine(SlowMo());
    }


    private static float waitTime;
    
    private static IEnumerator SlowMo()
    {
        Speed = .15f; 
        float count = 0;

        while (count < waitTime && !Paused)
        {
            count += Time.deltaTime;
            yield return null;
        }

        if(!Paused)
            Speed = 1;
    }*/
}
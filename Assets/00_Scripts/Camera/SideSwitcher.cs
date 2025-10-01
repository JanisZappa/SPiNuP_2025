using Clips;
using UnityEngine;
using UnityEngine.Profiling;


public class SideSwitcher : MonoBehaviour 
{
    private Side     currentSide, forcedSide, clipSide;
    private SideSway sideSwitch;

    
//  Output  //
    public float sideLerp;

    private bool forceUpdate;
    public bool animating;
    
    
    public void Reset()
    {
        currentSide = clipSide = forcedSide = GameCam.StartSide;
        sideSwitch  = new SideSway(currentSide, true);

        forceUpdate = true;
        CamUpdate();
    }

    
    public void CamUpdate()
    {
        Profiler.BeginSample("SideSwitcher.UpdateAngle()");

        
    //  Get Switch Lerp  //
        Clip clip = Spinner.CurrentFocusClip;
        if (clip != null)
        {
            Side newClipSide = clip.GetSide(GTime.Now);
            
            if (clip.Type != 0 && clipSide != newClipSide)
                clipSide = forcedSide = newClipSide;
        }  
    
        if (!GTime.Paused && KeyMap.Down(Key.Dev_SideSwitch))
            ForcedSideSwitch();
    
        if (currentSide != forcedSide)
        {                
            sideSwitch  = new SideSway(forcedSide);
            currentSide = forcedSide;
            animating = true;
        }
            
        float lerp = sideSwitch.GetLerp();
        bool  front = f.Same(Mathf.Round(lerp), 0);
        
        const float max = 1.2f, pow = 2.65f;
        sideLerp = front ?     max * Mathf.Pow(    lerp, pow) : 
                           1 - max * Mathf.Pow(1 - lerp, pow);

        if (animating && (lerp >= 1 || lerp <= 0))
            animating = false;

        if (animating || forceUpdate)
        {
            animating   = true;
            forceUpdate = false;
        }

        
        Profiler.EndSample();
    }


    public void ForcedSideSwitch()
    {
        forcedSide = !forcedSide;
    }
    
    
    private struct SideSway
    {
        private readonly float start, end;
        private float time;

        public SideSway(Side goalSide, bool stayOnSide = false)
        {
            start = stayOnSide? goalSide.front? 0 : 1 : 
                                goalSide.front? 1 : 0;
            
            end   = goalSide.front? 0 : 1;
            time  = 0;
        }

        public float GetLerp()
        {
            time += Time.deltaTime * GTime.Speed;
            return Mathf.Lerp(start, end, time * 3.25f);
        }
    }
}

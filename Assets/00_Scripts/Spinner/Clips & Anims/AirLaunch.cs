using UnityEngine;


namespace Clips
{
    public class AirLaunch : Jump 
    {
        public AirLaunch () : base(ClipType.AirLaunch) {}
        
        
        protected override Clip Setup(Spinner spinner, float startTime, Side startSide)
        {
            base.Setup(spinner, startTime, startSide);
            
            next       = Prep.nextClip;
            startAngle = Prep.startAngle;
            jumpV      = Prep.jumpV;
            startSpin  = Prep.startSpin;
            duration   = Prep.duration;
            startPos   = Prep.startPos;
    
            if (WillConnect)
                FindNextStick();
    
            return this;
        }
    }


    public abstract partial class Clip
    {
        public static Clip Get_AirLaunch(Spinner spinner, float startTime, float startAngle, Vector2 jumpV, float startSpin, float duration, Vector2 startPos, ClipType nextClip, Side side)
        {
            Prep.startAngle = startAngle;
            Prep.jumpV      = jumpV;
            Prep.startSpin  = startSpin;
            Prep.duration   = duration;
            Prep.startPos   = startPos;
            Prep.nextClip   = nextClip;

            return PoolClip(ClipType.AirLaunch, spinner, startTime, side);
        }
    }
}

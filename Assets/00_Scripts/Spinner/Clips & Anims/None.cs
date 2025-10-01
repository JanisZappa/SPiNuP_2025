using UnityEngine;


namespace Clips
{
    public class None : Clip
    {
        public static readonly None Get = new None();

        protected None(ClipType type = 0) : base(type) {}
        
        
        public override Placement BasicPlacement(float time, bool adjustForOffsets = false)
        {
            return Placement.OutOfSight;
        }


        public override Placement FinalPlacement(float time)
        {
            return Placement.OutOfSight;
        }


        public override Vector2 GetMV(float time)
        {
            return V2.zero;
        }
    }
}

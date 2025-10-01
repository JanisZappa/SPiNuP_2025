using UnityEngine;


namespace Anim
{
    public abstract class SpinnerAnim : PoolObject
    {
        public static readonly SpinnerAnim None = new Anim_Default();
        
        private readonly AnimLerp[][] lerps;
        private readonly int[] lerpCount;
        protected readonly Pose startPose = new Pose();
        
        
        static SpinnerAnim()
        {
             Anim_Jump.pool = new Pool<Anim_Jump> (() => new Anim_Jump(),  700);
            Anim_Swing.pool = new Pool<Anim_Swing>(() => new Anim_Swing(), 700);
        }

        protected SpinnerAnim()
        {
            lerpCount = new int[Pose.ValueCount];
            lerps = new AnimLerp[Pose.ValueCount][];
            for (int i = 0; i < Pose.ValueCount; i++)
                lerps[i] = new AnimLerp[10];
        }
        
        public SpinnerAnim Reset()
        {
            for (int i = 0; i < Pose.ValueCount; i++)
            {
                int count = lerpCount[i];
                AnimLerp[] partlerps = lerps[i];
                for (int e = 0; e < count; e++)
                    partlerps[e].Reset();

                lerpCount[i] = 0;
            }

            PoolReturn();

            return null;
        }
        
        protected abstract void PoolReturn();
        
        
        protected void Add(int part, AnimLerp animLerp)
        {
            lerps[part][lerpCount[part]++] = animLerp;
        }
        
        protected void Add(int part, AnimLerp.Bundle bundle)
        {
            lerps[part][lerpCount[part]++] = bundle.a;
            lerps[part][lerpCount[part]++] = bundle.b;
        }
        
        public abstract Pose SampleAnimation(float time, float localTime);
        
        protected float GetLerp(int part, float time)
        {
            int count = lerpCount[part];
            bool valid = false;
            float value = 0;
            AnimLerp lastLerp = null;
            AnimLerp[] partlerps = lerps[part];
            for (int i = 0; i < count; i++)
            {
                AnimLerp lerp = partlerps[i];
                if (lerp.startTime <= time)
                {
                    if (lerp.endTime >= time)
                    {
                        value += lerp.GetValue(time);
                        valid = true;
                    }
                    
                    lastLerp = lerp;
                }   
            }

            return valid? value : lastLerp?.GetValue(time) ?? startPose[part];
        }
        
        
        private static readonly BoolSwitch wiggleIt = new("Anim/Wiggle Test", false);

        protected void SetTurns(Pose pose, float time)
        {
            pose.Set(Pose.Turn, GetLerp(Pose.Turn, time));
        }
        
        protected void SetWiggle(Pose pose, float time, float localTime)
        {
            float bendT = GetLerp(Pose.Bend, time);
            
            if (wiggleIt)
                pose.Set(Pose.Bend, bendT + Mth.SmoothPP(localTime * 3.5f) * -.25f);
            
            pose.Set(Pose.Bend, bendT);
        }
    }
    
    
    public abstract class AnimLerp : PoolObject
    {
        public float startTime, endTime;
        protected float startValue, endValue;
        
        private AnimLerp Setup(float startTime, float endTime, float startValue, float endValue)
        {
            this.startTime  = startTime;
            this.endTime    = endTime;
            this.startValue = startValue;
            this.endValue   = endValue;
            return this;
        }
        
        public abstract void Reset();
        
        public abstract float GetValue(float time);
        
        
    #region static
        static AnimLerp()
        {
            poolFL = new Pool<AnimLerp>(() => new AnimLerpFL(), 7000);
            poolLF = new Pool<AnimLerp>(() => new AnimLerpLF(), 7000);
            poolFF = new Pool<AnimLerp>(() => new AnimLerpFF(), 7000);
        }

        protected static readonly Pool<AnimLerp> poolFL, poolLF, poolFF;

        private static AnimLerp LF(float startTime, float endTime, float startValue, float endValue)
        {
            return poolLF.GetFree().Setup(startTime, endTime, startValue, endValue);
        }

        private static AnimLerp FL(float startTime, float endTime, float startValue, float endValue)
        {
            return poolFL.GetFree().Setup(startTime, endTime, startValue, endValue);
        }
        
        public static AnimLerp FF(float startTime, float endTime, float startValue, float endValue)
        {
            return poolFF.GetFree().Setup(startTime, endTime, startValue, endValue);
        }

        
        private static readonly Bundle bundle = new Bundle();
        
        public static Bundle LFL(float startTime, float midTime, float endTime, float startValue, float endValue)
        {
            return bundle.Set(LF(startTime, midTime, startValue, endValue),
                              FL(midTime,   endTime, endValue, startValue));
        }
        
        public static Bundle LFF(float startTime, float midTime, float endTime, float startValue, float endValue)
        {
            return bundle.Set(LF(startTime, midTime, startValue, endValue),
                              FF(midTime,   endTime, endValue, startValue));
        }
        
        public static Bundle FFL(float startTime, float midTime, float endTime, float startValue, float endValue)
        {
            return bundle.Set(FF(startTime, midTime, startValue, endValue),
                              FL(midTime,   endTime, endValue, startValue));
        }
        
        public static Bundle FFF(float startTime, float midTime, float endTime, float startValue, float endValue)
        {
            return bundle.Set(FF(startTime, midTime, startValue, endValue),
                              FF(midTime,   endTime, endValue, startValue));
        }
        #endregion
        
        
        public class Bundle
        {
            public AnimLerp a, b;

            public Bundle Set(AnimLerp a, AnimLerp b)
            {
                this.a = a;
                this.b = b;

                return this;
            }
        }
    }


    public class AnimLerpFL : AnimLerp
    {
        public override void Reset() { poolFL.Return(this); }

        public override float GetValue(float time)
        {
            return Mathf.Lerp(startValue, endValue, Ease.FL((time - startTime) / (endTime - startTime)));
        }
    }
    
    public class AnimLerpLF : AnimLerp
    {
        public override void Reset() { poolLF.Return(this); }

        public override float GetValue(float time)
        {
            return Mathf.Lerp(startValue, endValue, Ease.LF((time - startTime) / (endTime - startTime)));
        }
    }
    
    public class AnimLerpFF : AnimLerp
    {
        public override void Reset() { poolFF.Return(this); }

        public override float GetValue(float time)
        {
            return Mathf.Lerp(startValue, endValue, Ease.FF((time - startTime) / (endTime - startTime)));
        }
    }
}

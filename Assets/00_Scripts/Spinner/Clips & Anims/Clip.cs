using System.IO;
using Anim;
using UnityEngine;
using Pose = Anim.Pose;


namespace Clips
{
    public abstract partial class Clip : PoolObject, ITimeSlice
    {
        protected Clip(ClipType Type) { this.Type = Type; }
        
        public readonly ClipType Type;
        public Spinner  spinner;
        public Clip     before, after;
        
        public float startTime, duration;
        public Side  startSide;
        
        private SpinnerAnim anim;
        private bool activated;
        
        public float StartTime => startTime;
        public float EndTime => startTime + duration;


        public abstract Placement BasicPlacement(float time, bool adjustForOffsets = false);
        
        
        public abstract Placement FinalPlacement(float time);
        
        
        protected virtual Clip Setup(Spinner spinner, float startTime, Side startSide)
        {
            this.spinner   = spinner;
            this.startTime = startTime;
            this.startSide = startSide;

            return this;
        }

        
        public abstract Vector2 GetMV(float time);


        public virtual void Activate()
        {
            if(!activated)
                Interaction.ActivateClip(this);
            
            activated = true;
        }


        public void SetClipBefore(Clip before)
        {
            this.before = before;
        
            if(before != null)
                before.ReadPose(startTime);
            else
                Pose.Reader.Zero();

            if (Type.IsAnyJump())
                anim = Anim_Jump.Get(this, Pose.Reader);

            if (Type.IsAnySwing())
                anim = Anim_Swing.Get(this, Pose.Reader);

            anim ??= SpinnerAnim.None;
        }


        public void SetClipAfter(Clip after)
        {
            this.after = after;

            if (Type.IsAnySwing())
                duration = after?.startTime - startTime ?? 0;
        }

        
        public Pose ReadPose(float time)
        { 
            return anim.SampleAnimation(time, time - spinner.startTime);
        }
        
        
        public virtual Side GetSide(float time) { return startSide; }
    }

    
    public abstract partial class Clip
    {
        static Clip()
        {
            pools = new []
            {
                null,
                new Pool<Clip>(() => new Jump(),      700),
                new Pool<Clip>(() => new Swing(),     700),
                new Pool<Clip>(() => new Bump(),      100),
                new Pool<Clip>(() => new AirLaunch(), 100),
                new Pool<Clip>(() => new Dead(),      100),
                new Pool<Clip>(() => new Spawn(),     100),
                null
            };
        }
        
        public void Reset()
        {
            anim = anim?.Reset();
            pools[(int)Type].Return(this);
            duration = 0;

            before  = null;
            after   = null;
            spinner = null;
            activated = false;
        }

        
    //  Same order as enum!!!  //
        private static readonly Pool<Clip>[] pools;
        
        
        public virtual void Serialize(BinaryWriter writer) 
        { 
            Debug.Log(Type + " has no serializer");
        }

        
        public static Clip Deserialize(BinaryReader reader, Spinner spinner, ClipType clipType, Side side, float timeShift = 0)
        {
            return clipType switch
            {
                ClipType.Jump  => Jump.DeserializeClip(reader, spinner, side, timeShift),
                ClipType.Dead  => Dead.DeserializeClip(reader, spinner, side, timeShift),
                ClipType.Spawn => Spawn.DeserializeClip(reader, spinner, side, timeShift),
                _ => null
            };
        }
        
        
        public static void SkipClip(BinaryReader reader, ClipType clipType)
        {
            int byteCount = clipType switch
            {
                ClipType.Jump  => 15,
                ClipType.Dead  => 2,
                ClipType.Spawn => 4,
                _ => 0
            };

            reader.BaseStream.Seek(byteCount, SeekOrigin.Current);
        }


        public static Clip GetJumpEvent(Jump jump)
        {
            Prep.Jump = jump;
            return jump.next == 0? null : PoolClip(jump.next, jump.spinner, jump.HitTime, jump.startSide);
        }


        public static Clip PoolClip(ClipType clipType, Spinner spinner, float startTime, Side side)
        {
            return pools[(int)clipType].GetFree().Setup(spinner, startTime, side);
        }
        
        
        protected static class Prep
        {
            public static float startAngle, startSpin;
        
        //  Jump  //
            public static float    startTime, duration;
            public static Vector2  jumpV, startPos;
            public static StickID  stickID;
            public static ClipType nextClip;
            public static float    weightLerp;
        
        //  Swing  //
            public static Jump Jump;
            
            
            private static readonly float[] values = new float[100];
            private static int pointer;

            
        //  Float  //
            public static void Set(float value, bool reset = false)
            {
                if(reset)
                    pointer = 0;
                
                values[pointer++] = value;
            }
            public static float GetFloat(bool reset = false)
            {
                if(reset)
                    pointer = 0;
                
                return values[pointer++];
            }
        //  Vector2  //
            public static void Set(Vector2 value, bool reset = false)
            {
                if(reset)
                    pointer = 0;
                
                values[pointer++] = value.x;
                values[pointer++] = value.y;
            }
            public static Vector2 GetVector2(bool reset = false)
            {
                if(reset)
                    pointer = 0;
                
                return new Vector2(values[pointer++], values[pointer++]);
            }
        //  ClipType  //
            public static void Set(ClipType value, bool reset = false)
            {
                if(reset)
                    pointer = 0;
                
                values[pointer++] = (float)value;
            }
            public static ClipType GetClipType(bool reset = false)
            {
                if(reset)
                    pointer = 0;
                
                return (ClipType)values[pointer++];
            }
        }
    }
}

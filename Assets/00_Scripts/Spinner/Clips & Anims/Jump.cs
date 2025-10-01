using System.IO;
using ActorAnimation;
using Anim;
using LevelElements;
using UnityEngine;


namespace Clips
{
    public partial class Jump : Clip
    {
        public Jump (ClipType type = ClipType.Jump) : base(type) {}
        
        public  ClipType next;
        public  StickID  stick, nextStick;
        public  float    startAngle, startSpin;
        private float    weightLerp;
        public  Vector2  jumpV, startPos;

        public FlyPath flyPath;
        public readonly JumpInfo info = new JumpInfo();
        
    //  INFO  //
        public bool  WillHit, WillConnect;
        public float HitTime;
        
        private readonly Offsetter offsetter = new Offsetter();
       


        protected override Clip Setup(Spinner spinner, float startTime, Side startSide)
        {
            base.Setup(spinner, startTime, startSide);
            
            next       = Prep.nextClip;
            startAngle = Prep.startAngle;
            jumpV      = Prep.jumpV;
            startSpin  = Prep.startSpin;
            stick      = Prep.stickID;
            weightLerp = Prep.weightLerp;
            duration   = Prep.duration;
            
        //  INFO  //
            WillHit     = next != 0;
            WillConnect = next.IsAnySwing();
            HitTime     = startTime + duration;
            
            startPos = CalculateStartPos(stick, spinner.ConnectRadius(stick.Item), startAngle, startTime);

            flyPath = new FlyPath(startPos, jumpV);
            
            if (WillConnect)
                FindNextStick();
            
            info.Analyze(this);

            return this;
        }
        
        
        private static Vector2 CalculateStartPos(StickID stickID, float radius, float angle, float startTime)
        {
            Vector2 bodyVector = V2.up.Rot(angle, -stickID.Sign * radius);
            Vector2 stickPos   = stickID.Item.GetLagPos(startTime);

            return new Vector2(bodyVector.x + stickPos.x, bodyVector.y + stickPos.y);
            //TODO? Ask yourself if this is important:
            //+ Clip_Swing.GetWeightForce(angle, spin, weightLerp, stickID.hands);
        }


        protected void FindNextStick()
        {
            float   connectTime  = startTime + duration;
            Vector2 charPosition = BasicPos(connectTime);
            Item    closestStick = Search.ClosestItem(charPosition, connectTime, startSide, Mask.CanBeGrabbed);

            if (closestStick != null)
            {
                float   angleNow = GPhysics.Get_SpinAngle_Deg(startSpin, duration);
                Vector2 stickPos = closestStick.GetLagPos(connectTime);
                
            
            //  Adjust SpinSpeed  //
                Vector2 bodyVector = V2.up.Rot(startAngle + angleNow);
                Vector2 toStick    = (stickPos - charPosition).normalized;
                bool    usingHands = Vector2.Dot(toStick, bodyVector) >= 0;
                float   newAngle   = angleNow - (usingHands ? bodyVector : -bodyVector).Angle_Sign(toStick);

                startSpin = GPhysics.GetStartSpin(newAngle, duration);
                
                
                angleNow = newAngle;
                int trimmSteps = 0;
                while (HasToBeTrimmedCrunch(startSpin, 3))
                {
                    usingHands = !usingHands;
                    newAngle   = angleNow - 180 * Mathf.Sign(angleNow);
                    startSpin  = GPhysics.GetStartSpin(newAngle, duration);
                    angleNow   = newAngle;

                    trimmSteps++;
                }

                if (trimmSteps > 0 && Application.isEditor)
                    Debug.LogFormat("Trimmed Spin {0} times. Spinner turns {1}° less".B_Salmon(), trimmSteps, trimmSteps * 180);
                
                nextStick = new StickID(closestStick, usingHands);
            }
            else
            {
                Debug.LogFormat("Spinner (Height {0}) failed to grab a Stick!".B_Lime(), spinner.size.y);
                Debug.Log(stick.Item.ID.ToString().B_Red());
                DRAW.Circle(charPosition, spinner.ConnectRadius()).SetColor(COLOR.orange.coral).SetDepth(Level.GetPlaneDist(startSide)).HoldFor(5);
                DRAW.Circle(startPos,     spinner.ConnectRadius()).SetColor(COLOR.yellow.fresh).SetDepth(Level.GetPlaneDist(startSide)).HoldFor(5);
            }
        }
        
        
        private static bool HasToBeTrimmedCrunch(float value, int digits)
        {
            return short.MaxValue / Mathf.Round(Mathf.Abs(value) * Mth.IntPow(10, digits)) < 1;
        }


        private Vector3 BasicPos(float time)
        {
            return flyPath.GetPos(time - startTime);
        }


        private Quaternion BasicRot(float time)
        {
            float angle = startAngle + GPhysics.Get_SpinAngle_Deg(startSpin, time - startTime);
            return Rot.Z(angle);
        }


        public override Placement BasicPlacement(float time, bool adjustForOffsets = false)
        {
            Vector3    pos = BasicPos(time);
            Quaternion rot = BasicRot(time);

            if (adjustForOffsets)
                pos += offsetter.LerpOffset(time, false).pos;
            
            return new Placement(pos, rot);
        }


        public override Placement FinalPlacement(float time)
        {
            Placement offset        = offsetter.LerpOffset(time, true);
            Placement basePlacement = BasicPlacement(time);

            return new Placement(offset.pos + basePlacement.pos, offset.rot * basePlacement.rot);
        }
        
        
        public override Vector2 GetMV(float time)
        {
            return flyPath.GetMV(time - startTime);
        }


        #region Events


        public override void Activate()
        {
            base.Activate();

            offsetter.SetupOffsets(this);
        }


        #endregion

        
        public void UpdateOffset()
        {
            offsetter.UpdateOffset();
        }

        
        private class Offsetter
        {
            private Jump clip;
            private readonly Placement[] mainOffsets = new Placement[3], 
                                       shadowOffsets = new Placement[3];
            
            private Vector2 lastLean;
            private float   blendStart;

        
            public Placement LerpOffset(float time, bool main)
            {
                return main? Placement.Lerp(  mainOffsets[0],   mainOffsets[1], GetLerp(time)) : 
                             Placement.Lerp(shadowOffsets[0], shadowOffsets[1], GetLerp(time));
            }


            private float GetLerp(float time)
            {   
                float endTime = clip.WillHit ? clip.HitTime : clip.startTime + 3;
                float range   = endTime - clip.startTime - blendStart;
                return (time - clip.startTime - blendStart) / range;
            }

            
            public void SetupOffsets(Jump clip)
            {
                this.clip  = clip;
                blendStart = 0;
                lastLean   = Vector2.zero;
                
                GetSwingOffsets(true);
            }

        
            public void UpdateOffset()
            {    
                GetSwingOffsets(false);
                blendStart = GTime.Now - clip.startTime;
            }
            
            
            private void GetSwingOffsets(bool init)
            {
            //  StartOffsets //
                if (init)
                {
                //  No Offsets  //
                      mainOffsets[2] = new Placement(Level.GamePlane(clip.startSide), Rot.Zero);
                    shadowOffsets[2] = Placement.Zero;
                    
                    mainOffsets[2].DebugIfNaN("MainOffsets2");
                    
                //  StartOffsets //
                    Placement basic = clip.BasicPlacement(clip.startTime);
                    Swing     swing = clip.before as Swing;

                      mainOffsets[0] = swing.FinalPlacement(clip.startTime) - basic;
                    shadowOffsets[0] = swing.BasicPlacement(clip.startTime, true) - basic;
                    
                    mainOffsets[0].DebugIfNaN("MainOffsets0");
                }
                else
                {
                      mainOffsets[0] = LerpOffset(GTime.Now, true);
                    shadowOffsets[0] = LerpOffset(GTime.Now, false);
                }
                
                
            //  EndOffsets //
                {
                    if (clip.WillConnect)
                    {
                        float endTime = clip.HitTime;
                        Swing swing = clip.after  as Swing;
                        Vector2 stickLean = ActorAnim.GetLean(swing.GetStick(endTime).Item, endTime);
                        if (init || stickLean != lastLean)
                        {
                            lastLean = stickLean;
                            
                            Placement basic = clip.BasicPlacement(endTime);
                              mainOffsets[1] = swing.FinalPlacement(endTime) - basic;
                            shadowOffsets[1] = swing.BasicPlacement(endTime, true) - basic;
                            
                            mainOffsets[1].DebugIfNaN("MainOffsets1");
                        }
                    }
                    else
                    {
                          mainOffsets[1] =   mainOffsets[2];
                        shadowOffsets[1] = shadowOffsets[2];
                    }
                }
            }
        }
    }
    
    
    
    
    public abstract partial class Clip
    {
        public static Clip Get_Jump(Spinner spinner, float startTime, float startAngle, Vector2 jumpV, float startSpin, float duration, StickID stickID, float weightLerp, ClipType nextClip, Side side)
        {
            Prep.startAngle = startAngle;
            Prep.jumpV      = jumpV;
            Prep.startSpin  = startSpin;
            Prep.duration   = duration;
            Prep.stickID    = stickID;
            Prep.weightLerp = weightLerp;
            Prep.nextClip   = nextClip;

            return PoolClip(ClipType.Jump, spinner, startTime, side);
        }
    }
    
    
    public partial class Jump
    {
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(startTime);
            writer.Write((short) stick.SerializeValue);
            writer.Write((byte) Mathf.Round(weightLerp * 100));
            writer.Write((sbyte) next);
            writer.Write(next == 0 ? 0f : duration);
            writer.Write(jumpV.x);
            writer.Write(jumpV.y);
            writer.Write(startAngle);
            writer.Write(startSpin);
        }
        

        private static void DeserializeClip(BinaryReader reader, float timeShift)
        {
            Prep.startTime  = reader.ReadSingle() + timeShift;
            int   itemID = reader.ReadInt16();
            Item  item       = Item.Get(Mathf.Abs(itemID) - 1, timeShift);
            Prep.stickID    = new StickID(item, itemID > 0);
            Prep.weightLerp = (float)reader.ReadByte() / 100;
            Prep.nextClip = (ClipType) reader.ReadSByte();  
            Prep.duration = reader.ReadSingle();  
            Prep.jumpV = new Vector3(reader.ReadSingle(), reader.ReadSingle());
            Prep.startAngle = reader.ReadSingle();
            Prep.startSpin  = reader.ReadSingle();
        }


        public static Clip DeserializeClip(BinaryReader reader, Spinner spinner, Side side, float timeShift)
        {
            DeserializeClip(reader, timeShift);
            
            return PoolClip(ClipType.Jump, spinner, Prep.startTime, side);
        }
    }
}
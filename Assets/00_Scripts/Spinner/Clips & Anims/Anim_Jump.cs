using Clips;
using Future;
using LevelElements;
using UnityEngine;


namespace Anim
{
    public class Anim_Jump : SpinnerAnim
    {
        private static readonly bool failedGrabs = new BoolSwitch("Anim/Failed Grabs", true);
        
        public static Pool<Anim_Jump> pool;
        private Jump jump;
        
            
        public static Anim_Jump Get(Clip clip, Pose startPose)
        {
            return pool.GetFree().Setup(clip, startPose);
        }

        
        private Anim_Jump Setup(Clip clip, Pose startPose)
        {
            jump = (Jump) clip;
            this.startPose.Copy(startPose);

            float jumpStart    = jump.startTime;
            float jumpDuration = jump.WillHit ? jump.duration: 5;
            float jumpEnd      = jumpStart + jumpDuration;
            

        //  Jump Squash  //
            //Spinner.Squash.AddSquash(_jump.spinner, _jump.startTime, _jump.jumpV.magnitude * .01f);
            
        //  Zero out Tumble  //
            if(startPose[Pose.Tumble] > 0)
                Add(Pose.Tumble, AnimLerp.FF(jumpStart, jumpStart + Mathf.Min(jumpDuration, .3f), startPose[Pose.Tumble], 0));
            
            
    //  LeanAnim  //
        {
            float inTime  = 1.25f, outTime = .75f;
    
        //  If LerpTime bigger then FlightTime tweak In-Outs  //
            if (inTime + outTime > jumpDuration)
            {
                float multi = jumpDuration / (inTime + outTime);
    
                inTime  *= multi;
                outTime *= multi;
            }
    
        //  Setup LerpParameters  //
            float startLean = startPose[Pose.Lean];
    
        //  Lean away from SpinDirection  |  More Lean when slower  |  Less Lean when short jump  //
            float maxLean = -Mathf.Sign(jump.startSpin) * 
                             Mathf.Lerp(1, 0, Mathf.Abs(jump.startSpin) / 11) * 
                             Mathf.Lerp(0, 1, jumpDuration * .7f);
    
            float endLean = jump.next == ClipType.Bump ? 0 : maxLean * .1f;
    
            Add(Pose.Lean, AnimLerp.FF(jumpStart, jumpStart + inTime, startLean, maxLean));
            Add(Pose.Lean, AnimLerp.FF(jumpStart + jumpDuration - outTime, jumpStart + jumpDuration, maxLean, endLean));
        }
        
    //  Push Bend  //
        //if(false)
        {
            const float pushA   = .31f,
                        pushB   = pushA * 1.25f,
                        howMuch = .45f, 
                        fractA  = .355f, 
                        fractB  = .645f;
        
            bool hands = jump.stick.hands;

            float sign      = hands ? Mathf.Sign(jump.startSpin) : -Mathf.Sign(jump.startSpin);
            float pushMulti = Mathf.Clamp01(Mathf.Abs(jump.startSpin) / 25f);
            float amount    = howMuch * sign * pushMulti;
            
        //  Push Limbs  //
            {  
                float duration   = Mathf.Min(jumpDuration, pushA * (1 + pushMulti * .4f));
                float pushAmount = amount * (duration / pushA);
                
                float inEnd  = jumpStart + duration * fractA, 
                      outEnd = inEnd + duration * fractB;
                
                Add(Pose.Bend, AnimLerp.LFF(jumpStart, inEnd, outEnd, 0, pushAmount));
            }
            
        //  Swing Limbs  //
            /*{
                float duration    = Mathf.Min(jumpDuration, pushB * (1 + pushMulti * .4f));
                float swingAmount = amount * (duration / pushB) * -.8f;
                
                float inEnd  = jumpStart + duration * fractA, 
                      outEnd = inEnd + duration * fractB;
                
                Add(Pose.Bend, AnimLerp.LFF(jumpStart, inEnd, outEnd, 0, swingAmount));
            }*/
        }
    
        
    //  Failed Grabs  //
        if(failedGrabs)
        {
            const float flip = .45f, grab = flip * 1.05f, delay = .01f;
            
            JumpInfo info = jump.info;

            float minTime = jumpStart + delay;
            float maxTime = jumpEnd - flip - delay;
            
            int proxyCount = info.proxCount;

            Spinner spinner = jump.spinner;
            float spinnerRadius = spinner.size.y * .5f;
            const float reachDist = 3f;
            float checkDist = Mathf.Pow(spinnerRadius + reachDist, 2);

            float turn = startPose[Pose.Turn];

           
            for (int i = 0; i < proxyCount; i++)
            {
                ProxItem proxItem = info.proxItems[i];
                Item item = proxItem.item;

                if (!Mask.CanBeGrabbed.Fits(item.elementType) ||
                    item == jump.stick.Item || 
                    item == jump.nextStick.Item ||
                    proxItem.closestDist > checkDist)
                    continue;
                
                float end       = proxItem.closestTime + JumpInfo.StepLength;
                float checkTime = Mathf.Max(jumpStart, proxItem.closestTime - JumpInfo.StepLength * 2);

                if (ItemCheck.FirstHit(checkTime, end, JumpInfo.StepLength, jump, item, checkDist, out checkTime) &&
                    checkTime > minTime && checkTime + grab < maxTime)
                {
                    //Spinner.Squash.AddNewSquash(jump.spinner, checkTime, .8f, 1.5f, .7f);

                    float newTurn = turn + .5f;
                    
                    Add(Pose.Turn, AnimLerp.FF(checkTime - delay, checkTime + flip - delay, turn, newTurn));
                    Add(Pose.Tumble, AnimLerp.FFF(checkTime - delay, checkTime + grab * .5f, checkTime + grab, 0, 1f));
                    
                    turn = newTurn;
                    minTime = checkTime + grab + delay;
                }
            }
        }
        
        
        //  PreGrab  //
            if (jump.WillConnect)
            {
                const float grab    = .35f,
                            howMuch = .55f, 
                            fractB  = .455f, 
                            fractA  = .545f;
        
                bool hands = jump.nextStick.hands;

                float endSpin = GPhysics.Get_SpinSpeed_After(jump.startSpin, jumpDuration);

                float sign      = hands ? -Mathf.Sign(endSpin) : Mathf.Sign(endSpin);
                float pushMulti = Mathf.Clamp01(Mathf.Abs(endSpin) / 25f);
                float amount    = howMuch * sign * pushMulti;
            
            //  Grab Limbs  //
                {  
                    float duration   = Mathf.Min(jumpDuration, grab * (1 + pushMulti * .4f));
                    float grabAmount = amount * (duration / grab);
                
                    float inEnd  = jumpEnd - duration + duration * fractA, 
                          outEnd = inEnd + duration * fractB;
                
                    Add( Pose.Bend, AnimLerp.FFL(jumpEnd - duration, inEnd, outEnd, 0, grabAmount));
                }
            }
            
            
        //  Pivot  //
            const float pivotAnim = 1f;
            if (jump.WillConnect)
            {
                float duration = Mathf.Min(jumpDuration * .5f, pivotAnim);
                Add( Pose.Pivot, AnimLerp.FF(jumpStart, jumpStart + duration, jump.stick.hands? 1 : 0, 0));
                Add( Pose.Pivot, AnimLerp.FF(jumpEnd - duration, jumpEnd, 0, jump.nextStick.hands? 1 : 0));
            }
            else
                Add( Pose.Pivot, AnimLerp.FF(jumpStart, jumpStart + pivotAnim, jump.stick.hands? 1 : 0, 0));
            
            return this;
        }


        protected override void PoolReturn()
        {
            pool.Return(this);
        }

        
        public override Pose SampleAnimation(float time, float localTime)
        {
            Pose.Reader.Zero().Set(Pose.Lean, GetLerp(Pose.Lean, time));

            float tumble = GetTumble(time);
            Pose.Reader.Set(Pose.Tumble, Mathf.Max(GetLerp(Pose.Tumble, time), tumble * 1.5f));
            Pose.Reader.Set(Pose.Pivot, GetLerp(Pose.Pivot, time));
            
            SetWiggle(Pose.Reader, time, localTime);

            SetTurns(Pose.Reader, time);

            return Pose.Reader;
        }


        private float GetTumble(float time)
        {
            float flightTime = time - jump.startTime;
            float spinSpeed  = GPhysics.Get_SpinSpeed_After(jump.startSpin, flightTime);
            float speed      = Mathf.Clamp01(jump.GetMV(time).magnitude * .0135f);
            
            const float lerpDuration = .15f;
            if (jump.WillHit)
            {
                float actualLerpDuration = lerpDuration * 2 > jump.duration ? jump.duration * .5f : lerpDuration;

                float inLerp  = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0, actualLerpDuration, flightTime));
                float outLerp = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(jump.duration, jump.duration - actualLerpDuration, flightTime));
                float tumble  = Mathf.Max(speed, Mathf.InverseLerp(7.4f, 3.5f, Mathf.Abs(spinSpeed)));
                
                return inLerp * outLerp * tumble;
            }
            
            {
                float inLerp = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0, lerpDuration, flightTime));
                float tumble = Mathf.Max(speed, Mathf.InverseLerp(9.8f, 3.2f, Mathf.Abs(spinSpeed)));
                
                return inLerp * tumble;
            }
        }
    }
}
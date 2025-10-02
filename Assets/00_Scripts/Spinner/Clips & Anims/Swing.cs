using ActorAnimation;
using Anim;
using Future;
using LevelElements;
using UnityEngine;
using UnityEngine.Profiling;


namespace Clips
{
    public class Swing : Clip
    {
        private const float SpinSpeed    = 14.5f;  
        private const float SpinRadSpeed = SpinSpeed * Mathf.Deg2Rad;  
        
        static Swing()
        {
            swingCurve = new AnimationCurve(new Keyframe(  0,   0, 1.6275f, 1.6275f), 
                                            new Keyframe(.5f, .5f,    .78f,    .78f), 
                                            new Keyframe(  1,   1, 1.6275f, 1.6275f))
                         { preWrapMode = WrapMode.Loop, postWrapMode = WrapMode.Loop };
            
            const int division = 3600;
            angleMap = new float[3600];
            for (int i = 0; i < division; i++)
            {
                float startRot   = i * .1f;
            //  Sampling AnimationCurve every 10 Degrees - Then Step 10 Degrees back  //
                float sampleTime = Mathf.Repeat(ClosestSampleRotationTime(startRot, 0, 36, 1f / 36) - 1f / 36, 1);
            //  More Precise Sampling every 0.1f Degrees - For 20 Degrees  //
                angleMap[i] = ClosestSampleRotationTime(startRot, sampleTime, 200, .01f / 36);
            }
        }
        
        
        public Swing (ClipType type = ClipType.Swing) : base(type) {}
        
        public StickID startStick;
        public Vector2 startMotion;
        public float   startAngle, startSpinSpeed, spinSpeed;
   
        
    //  Fucking Bad Connection Calc  //
        private float c_ResultSpeed, c_Accel2New, c_Time2New, c_Accel2Basic, c_Time2Basic;
        
        
    //  0 = BodyDir  <->  1 = RotDir  //
        private const float flyDirection = .35f;
        private bool Hands { get { return startStick.hands; }}
        public  bool DebugCrash;


        private static readonly AnimationCurve swingCurve, inverseCurve;
        private static readonly float[] angleMap;
        
        
    //  Used in StickSwing_Anim  //
        public Quaternion BasicRot(float time)
        {
            float animLerp = GetAnimLerp(time);
            float angle    = Mathf.Repeat(swingCurve.Evaluate(animLerp) * 360f + (Hands ? 0 : 180), 360);
            return Rot.Z(angle);
        }
        
        
        public override Placement BasicPlacement(float time, bool adjustForOffsets = false)
        {
            StickID    stick     = GetStick(time);
            Vector2    stickPos  = stick.Item.GetLagPos(time);
            Quaternion rot2D     = BasicRot(time);

            Vector2 bodyV;
            if (adjustForOffsets)
            {
                stickPos += ActorAnim.GetLean(stick.Item, time);
                bodyV = rot2D * new Vector2(0, -stick.Sign * spinner.size.y * .5f * spinner.squashes.GetSquash(time));
            }
            else
                bodyV = rot2D * new Vector2(0, -stick.Sign * spinner.ConnectRadius(stick.Item));
            
            return new Placement(stickPos + bodyV, rot2D);
        }
    
    
        public override Placement FinalPlacement(float time)
        {
            StickID stick = GetStick(time);
            Side    side  = GetSide(time);
            
            Vector2 pos      = stick.Item.GetPos(time);
            Vector2 leanPos  = stick.Item.GetLagPos(time);
            Vector2 animLean = ActorAnim.GetLean(stick.Item, time);
            
            Quaternion stickRot = (animLean + (leanPos - pos)).LeanRotGlobal(stick.Item);

           
            Placement poseOffset = spinner.rig.poser.GetSwingOffset(ReadPose(time), spinner.squashes.GetSquash(time), stick.hands, stick.Item.radius);

            Quaternion rot2D      = poseOffset.rot * BasicRot(time);
            Vector3    bodyOffset = rot2D * poseOffset.pos;
            
            
            float   alongStick = (Level.PlaneOffset - stick.Item.depth + spinner.GetZShift(time)) * side.Sign;
            Vector3 resultPos  = pos.V3((Level.WallDepth + stick.Item.depth) * side.Sign)
                                + stickRot * bodyOffset.AddZ(alongStick);

            return new Placement(resultPos, stickRot * rot2D);
        }

        
        public override Vector2 GetMV(float time)
        {
            return GetStick(time).Item.GetMV(time) * .55f;
        }
    
    
        public Vector2 GetForceV(float time)
        {
            StickID Stick        = GetStick(time);
            float   actualSpin   = GetActualSpeed(time);
            float   spinSign     = Mathf.Sign(actualSpin);
            Quaternion rot2D     = BasicRot(time);
            
            bool    toBack       = !f.Same(spinSign, Stick.Sign);
            Vector2 rotateVector = rot2D * new Vector2(toBack ? 1 : -1, 0);
            Vector2 bodyVector   = rot2D * new Vector2(0, Stick.Sign);
            
            Vector2 forceV       = Vector2.Lerp(bodyVector, rotateVector, flyDirection);

            return forceV.SetLength(actualSpin * -spinSign * GPhysics.JumpForce);
        }
        
        
        public Vector2 GetWeightDir(float time)
        {
            bool  hands      = GetStick(time).hands;
            float weightLerp = GetWeightLerp(time);
            float spin       = GetJumpSpin(time);
            
            Quaternion rot = BasicRot(time);
            
            return GetWeightForce(rot, spin, weightLerp, hands);
        }


        public static Vector2 GetWeightForce(Quaternion rot, float spinSpeed, float weightLerp, bool hands)
        {
            const float playerWeight = .012f;    //.0305f;
         
            float   handFeetSign = hands? -1 : 1;
            Vector2 forceDir     = rot * new Vector2(0, handFeetSign * Mathf.Abs(spinSpeed));
            
            return new Vector2(forceDir.x * weightLerp * playerWeight, forceDir.y * weightLerp * playerWeight);
        }


        private float GetWeightLerp(float time)
        {
            const float weightLerpSpeed = 6;
            return Mathf.SmoothStep(0, 1, (time - startTime) * weightLerpSpeed);
        }
    
        
        public float GetActualSpeed(float time)
        {
            float stickTime = time - startTime;
    
            if (stickTime >= c_Time2Basic)
                return SpinSpeed * Mathf.Sign(spinSpeed);
            
            return spinSpeed + .5f * c_Accel2Basic * (stickTime * stickTime) * Mathf.Sign(spinSpeed);
        }
        

        private bool    warpTriggered;
        private StickID warpStick;
        private float   warpTime;
        
        
        private bool HasWarped(float time)
        {
            if (time >= warpTime)
            {
                if(!warpTriggered)
                {
                    warpTriggered = true;
                
                    if (spinner.isFocus && startStick.Item.side == warpStick.Item.side)
                        MoveCam.SetWarp(time, startStick.Item.GetPos(time), warpStick.Item.GetPos(time));

                    ActorAnimator.OnWarp(this, time);
            
                    Sound.Get(Audio.Sound.StickWarp).PlayerMulti(spinner).SetItem(startStick.Item).Play();    
                }

                return true;
            }

            return false;
        }


        public StickID GetStick(float time)
        {
            return HasWarped(time) ? warpStick: startStick;
        }
        
        
        public override Side GetSide(float time)
        {
            return HasWarped(time) ? warpStick.Item.side : startSide;
        }

    
        #region PoseLerp
    
        private float GetAnimLerp(float time)
        {
            float onStickTime = time - startTime;
            
            float spunSoFar;
            if (onStickTime < c_Time2Basic)
                spunSoFar = AccelSpin(onStickTime);
            else
                spunSoFar = AccelSpin(c_Time2Basic) + SpinAngle_Deg(SpinSpeed * Mathf.Sign(spinSpeed), onStickTime - c_Time2Basic);
    
            const float curveMapFactor = .7265f;
            spunSoFar *= curveMapFactor;
            
            float angle = startAngle + spunSoFar + (Hands ? 0 : 180);
            return Mathf.Repeat(angle % 360f / 360f, 1);
        }
    
        
        private static float SpinAngle_Deg(float startSpin, float time)
        {
            return startSpin * time * 35f;
        }
    
        
        private float AccelSpin(float time)
        {
            float returnAngle;
    
            if (time < c_Time2New)
                returnAngle = c_ResultSpeed * time + .5f * c_Accel2New * time * time;
    
            else
            {
                returnAngle  = c_ResultSpeed * c_Time2New + .5f * c_Accel2New * c_Time2New * c_Time2New;
                returnAngle += spinSpeed * (time - c_Time2New) + .5f * c_Accel2Basic * (time - c_Time2New) * (time - c_Time2New) * Mathf.Sign(spinSpeed);
            }
    
            return returnAngle * 35f;
        }
    
        #endregion
    
        
        protected override Clip Setup(Spinner spinner, float startTime, Side startSide)
        {
            base.Setup(spinner, startTime, startSide);
            
        //  Reset warpTime  //
            warpTime = float.MaxValue;
            warpTriggered = false;
            
            if (Prep.Jump == null)
            {
                startStick     = new StickID(Level.StartStick, true);
                startSpinSpeed = SpinSpeed * .95f;
                spinSpeed      = SpinSpeed;
            }
            else
            {
                startStick = Prep.Jump.nextStick;
                    
                Placement placement = Prep.Jump.BasicPlacement(startTime);
                float     angleNow  = placement.rot.eulerAngles.z;
                Vector2   charPos   = placement.pos;
                
                
            //  Item Spinner Motionvector  //
                Vector2 charMotion = new FlyPath(Vector2.zero, Prep.Jump.jumpV).GetMV(Prep.Jump.duration);
                if (charMotion.magnitude >= FlyPath.CrashSpeed)
                    DebugCrash = true;
        
                
            //  Combine Spinner MovementVector with StickMovementVector  //
                if (startStick.Item == null)
                    Debug.Log("Swing StartItem Is Null ...");
                
                
                Vector2 stickPosition  = startStick.Item.GetLagPos(startTime);
                Vector2 stickMovement  = startStick.Item.GetMV(startTime);
                Vector2 combinedMotion = charMotion - stickMovement;
        
                
            //  Check Angle Between MotionVector and Spinner  //
                float angle = Vector2.Angle(combinedMotion, V2.up.Rot(angleNow));
        
                if (angle > 90)
                    angle = 90 - (angle - 90);

                const float divMulti = 1f / 90;
                angle *= divMulti;
        
        
            //  Determin SpinDirection  //
            //  If the Spinner approaches with the Stick on the Left the MomentumSpin is Negative  //
            //  If on the Right Positive  //
                const float c_spinConnectionFactor  = .3f;
                const float c_speedConnectionFactor = .9f;
                
                Vector2 stickDir  = stickPosition + combinedMotion.normalized - stickPosition;
                Vector2 stickPos  = charPos - stickPosition;
                int     stickSide = stickDir.Side_Sign(stickPos);
        
                float momentumSpin = combinedMotion.magnitude * c_speedConnectionFactor * (stickSide >= 0 ? 1 : -1);
                float spinSpin     = (f.Same(stickSide, Mathf.Sign(startSpinSpeed)) ? angle : -angle) * Mathf.Abs(startSpinSpeed) * c_spinConnectionFactor;
        
        
            //  Lerp Between MomentumSpin and SpinSpin  //
                float combinedSpinSpeed = Mathf.Lerp(momentumSpin, spinSpin, .5f);
        
        
            //  Connection Data  //
                startMotion = combinedMotion;
        
                float gravityLerp = Vector2.Angle(V2.down, combinedMotion) / 180;
                float gravitySpin = Mathf.Lerp(10.75f, 7.25f, gravityLerp);
        
                spinSpeed  = gravitySpin * Mathf.Sign(combinedSpinSpeed) + combinedSpinSpeed;
                startAngle = GetStartRot(angleNow, startStick.Sign);
        
        
                float a = Mathf.Repeat(angleNow, 360);
                float b = Mathf.Repeat(BasicRot(startTime).eulerAngles.z, 360);
        
                if (Mathf.Abs(a - b) > 2)
                    Debug.Log("Before: " + a + "  After: " + b);
            }
        
        
            c_ResultSpeed = Mathf.Lerp(startSpinSpeed, spinSpeed, .5f);
            float speedDifference = spinSpeed - c_ResultSpeed;
            c_Time2New  = Mathf.Abs(speedDifference) * .0475f / (1 + startMotion.magnitude * .515f);
            c_Accel2New = speedDifference / c_Time2New;
    
    
        //  GameAcceleration  //
            float absSpeed  = Mathf.Abs(spinSpeed);
            float speedDiff = Mathf.Abs(absSpeed - SpinSpeed);
    
            c_Accel2Basic = absSpeed > SpinSpeed ? -2.2f : 20;
            c_Time2Basic  = speedDiff / Mathf.Abs(c_Accel2Basic);
            c_Time2Basic += c_Time2New;
            
            return this;
        }


        public void SetWarp(Item warpBuddy)
        {
            warpTime  = startTime + Warp.warpTime;
            warpStick = new StickID(warpBuddy, startStick.hands);
        }
    
        
        private static float GetStartRot(float startRot, float spinPoint)
        {
            startRot = Mathf.Repeat(startRot + (spinPoint < 0 ? 180 : 0), 360f);
            
            return angleMap[(int) Mathf.Repeat(Mathf.Round(startRot * 10), 3600)] * 360 + (spinPoint < 0 ? 180 : 0);
        }
    
        
        private static float ClosestSampleRotationTime(float startRot, float gotTime, int steps, float stepLength)
        {
            float difference = float.MaxValue;
            float returnTime = 0;
    
            for (int i = 0; i < steps; i++)
            {
                float checkTime   = gotTime + stepLength * i;
                float sampleAngle = swingCurve.Evaluate(checkTime) * 360;
                float checkDiff   = Mathf.Abs(sampleAngle - startRot);
    
                if (checkDiff < difference)
                {
                    difference = checkDiff;
                    returnTime = checkTime;
                }
            }
    
            return returnTime;
        }


        public Clip GetJumpData(float eventTime, Spinner spinner)
        {
            Placement basicPlacement = BasicPlacement(eventTime);
    
            Profiler.BeginSample("Stick_JumpFrom Check");
    
            Clip jumpClip = Prediction.JumpCheck(eventTime, spinner, basicPlacement.pos, GetWeightLerp(eventTime), basicPlacement.rot, GetForceV(eventTime), GetJumpSpin(eventTime), GetStick(eventTime));
    
            Profiler.EndSample();
    
            return jumpClip;
        }
    
        
        private float GetJumpSpin(float time)
        {
            float spinAnimLerp = GetAnimLerp(time);

            float first        = swingCurve.Evaluate(Mathf.Repeat(spinAnimLerp, 1));
            float second       = swingCurve.Evaluate(Mathf.Repeat(spinAnimLerp + .01f, 1));
    
            Vector3 tangentVector = new Vector3(spinAnimLerp + .01f, second, 0) - new Vector3(spinAnimLerp, first, 0);
    
            float scaleFactor = 1f / tangentVector.x;
            float multiplier = tangentVector.y * scaleFactor;

            if (multiplier < 0)
                multiplier += 100;
    
            return GetActualSpeed(time) * multiplier * .85f;
        }
    }   
}
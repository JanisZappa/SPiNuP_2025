using Clips;
using UnityEngine;


namespace Anim
{
    public class Anim_Default : SpinnerAnim
    {
        protected override void PoolReturn(){}

        public override Pose SampleAnimation(float time, float localTime)
        {
            Pose.Reader.Zero();
            SetTurns(Pose.Reader, time);
            SetWiggle(Pose.Reader, time, localTime);

            return Pose.Reader;
        }
    }
    
    
    
    public class Anim_Swing : SpinnerAnim
    {
        public static Pool<Anim_Swing> pool;
        private Swing swing;

        public static Anim_Swing Get(Clip clip, Pose startPose)
        {
            return pool.GetFree().Setup(clip, startPose);
        }


        private Anim_Swing Setup(Clip clip, Pose startPose)
        {
            this.startPose.Copy(startPose);
            swing = (Swing) clip;
            
            Vector2 impactV = new Vector2(swing.startMotion.x, swing.startMotion.y + GPhysics.Gravity * -.75f);
            float impactV_M = impactV.magnitude;
            
        //  Connection Squash  //
            {
                Vector2 pivotUp = V2.up.Rot(swing.startAngle, swing.startStick.Sign);
               
                float impact    = impactV_M > 0? Vector3.Dot(impactV * (1f / impactV_M), pivotUp) * impactV_M : 0;
                float absImpact = Mathf.Abs(impact);
                float amount    = Mathf.Clamp01(absImpact * .015f) * -Mathf.Sign(impact);
                float frequency = absImpact / clip.spinner.size.y / 5.5f;
                float duration  = absImpact / 60f * 2.5f;
                
                swing.spinner.squashes.AddSquash(swing.startTime, amount, frequency, duration);
            }
            
            CalculateConnectionSwing();
            
        //  Tumble Other Limbs //
            float tumbleAmount = Mathf.Clamp(impactV_M * .035f, 0, 2);
            Add(Pose.Tumble, AnimLerp.LFF(swing.startTime, swing.startTime + .2f, swing.startTime + 1.3f, 0, tumbleAmount));
            
            return this;
        }


        protected override void PoolReturn()
        {
            pool.Return(this);
        }

        
        public override Pose SampleAnimation(float time, float localTime)
        {
            Pose.Reader.Zero().Set(Pose.Lean, GetLean(time));
            
            SetTurns(Pose.Reader, time);
            SetWiggle(Pose.Reader, time, localTime);
            
            Pose.Reader.Set(Pose.Tumble, GetLerp(Pose.Tumble, time));
            Pose.Reader.Set(Pose.Pivot, swing.startStick.hands? 1 : 0);
            
            return Pose.Reader;
        }
        

        private void CalculateConnectionSwing()
        {
        //  Calculate SpeedChange  //
            float speedChange;

            if (f.Same(Mathf.Sign(swing.startSpinSpeed), Mathf.Sign(swing.spinSpeed)))
            {
                if (Mathf.Sign(swing.startSpinSpeed) > Mathf.Sign(swing.spinSpeed))
                    speedChange = Mathf.Abs(swing.startSpinSpeed) - Mathf.Abs(swing.spinSpeed);
                else
                    speedChange = -(Mathf.Abs(swing.spinSpeed) - Mathf.Abs(swing.startSpinSpeed));
            }
            else
                speedChange = -(Mathf.Abs(swing.spinSpeed) + Mathf.Abs(swing.startSpinSpeed));


        //  SpinOverlap  //
            float overlap = speedChange * swing.spinSpeed * .0025f * swing.startStick.Sign;


        //  Impact Overlap  //
            Vector2 up = V2.up.Rot(swing.startAngle);

            Vector2 impactVector = (swing.startMotion != V2.zero) ? swing.startMotion + new Vector2(0, GPhysics.Gravity * -0.45f) : V2.zero;

            float impactAngle = Vector3.Angle(impactVector, up);
            if (impactAngle > 90)
                impactAngle = 180 - impactAngle;

            float impactAngleFactor = impactAngle / 90;

            float magnitude = 1 + swing.startMotion.magnitude;


        //  !!!!!!!!!!!!  //
            float impactAngleLean = magnitude * impactAngleFactor * .1f; //    0.05f

            if (f.Same(Mathf.Sign(swing.spinSpeed), 1) && swing.startStick.hands
                || f.Same(Mathf.Sign(swing.spinSpeed), -1) && !swing.startStick.hands)
                impactAngleLean *= -1;


        //  Combine Overlaps  //
            overlap += impactAngleLean;

            float lerpFactor = Mathf.Abs(swing.spinSpeed) * .0185f;
            overlap = Mathf.Lerp(overlap, 0, lerpFactor);


        //  Create Lerps  //
            float lerpInLength = 2.6f / (magnitude * 1.2f);
            float lerpOutLength = lerpInLength * .25f + .025f * magnitude;

            Add(Pose.Lean, AnimLerp.LFF(swing.startTime, swing.startTime + lerpInLength,swing.startTime + lerpInLength + lerpOutLength, 0, overlap));
        }

        
        private float GetLean(float time)
        {
            if (swing == null)
                return 0;


        //  SpinLean  //
            float lerp    = swing.BasicRot(time).eulerAngles.z / 360;
            float leandir = 1;


        //  Please explain this  //
            switch ((int) Mathf.Sign(swing.spinSpeed))
            {
                case 1:
                    if (!swing.startStick.hands)
                    {
                        lerp = Mathf.Repeat(lerp - .5f, 1);
                        leandir = -1;
                    }
                    break;

                case -1:
                    if (swing.startStick.hands)
                    {
                        lerp = 1 - lerp;
                        leandir = -1;
                    }
                    else
                        lerp = 1 - Mathf.Repeat(lerp - .5f, 1);
                    
                    break;
            }

            float curveLean = Curves.SwingLeanAnim.Evaluate(lerp);
            float spinLean  = curveLean * 2f * leandir;

            float leanLerp  = Mathf.Clamp((time - swing.startTime) * .25f, 0, .25f) * 4;
            float stickLean = Mathf.Lerp(Mathf.Lerp(spinLean, 0, Mathf.Abs(swing.GetActualSpeed(time)) * .0255f), startPose[Pose.Lean], 1 - leanLerp);

            return stickLean + GetLerp(Pose.Lean, time);
        }
    }
}
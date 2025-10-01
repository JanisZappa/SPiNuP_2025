using GeoMath;
using LevelElements;
using UnityEngine;


namespace Clips
{
    public class Bump : Clip
    {
        public Bump() : base(ClipType.Bump) {}

        private Vector2 startPos;
        public  Vector2 mV;
        private float   startAngle, startSpin;
        public  Item    bumpItem;
        private FlyPath flyPath;


        protected override Clip Setup(Spinner spinner, float startTime, Side startSide)
        {
            base.Setup(spinner, startTime, startSide);
            
            flyPath = new FlyPath(Prep.Jump.startPos, Prep.Jump.jumpV);
    
            startPos   = flyPath.GetPos(Prep.Jump.duration);
            startAngle = Prep.Jump.startAngle + GPhysics.Get_SpinAngle_Deg(Prep.Jump.startSpin, Prep.Jump.duration);
            
            bumpItem = Search.ClosestItem(startPos, startTime, startSide, Mask.IsItem);
            if (bumpItem == null)
                return this;
            
        //  CharMV and ItemPos  //
            Vector2 charMV  = flyPath.GetMV(Prep.Jump.duration);
            Vector2 itemPos = bumpItem.GetLagPos(startTime);
            
        //  Closest Point On Char BodyLine  //
            Vector2 bodyDir     = V2.up.Rot(startAngle, spinner.size.y - spinner.size.x);
            Vector2 pointOnLine = Line.NewMidDirLine(startPos, bodyDir).ClosestPoint(itemPos);
            
        //  Calculate Hitpoint  //
            Vector2 hitNormal = (itemPos - pointOnLine).normalized;
            Vector2 hitPoint  = itemPos - hitNormal * bumpItem.radius;
            
        //  Calculate Velocitys at Hitpoint  //
            float   charRadSpin    = GPhysics.Get_SpinSpeed_After(Prep.Jump.startSpin, Prep.Jump.duration) * 35f * Mathf.Deg2Rad;
            Vector2 charToHitpoint = hitPoint - startPos;
            Vector2 charAngularVel = charRadSpin.Cross(charToHitpoint);
        
            Vector2 charHitpointVel  = charMV + charAngularVel;
            Vector2 itemHitpointVel  = bumpItem.GetMV(startTime); // + 0f.Cross(hitPoint - itemPos); stickAngularVel;
            Vector2 relativeVelocity = charHitpointVel - itemHitpointVel;
        
            float velAlongNormal = Vector2.Dot(relativeVelocity, hitNormal);
        
        //  CollisionResponse  //
            float hitDirCross = charToHitpoint.Cross(hitNormal);
        
            const float e            = .55f;
            const float mass         = 10;
            const float colliderMass = 100000;
            const float inertia      = 8;
            
            float impulse = -(1 + e) * velAlongNormal / 
                            ( 
                                1 / mass + 1 / colliderMass 
                                + Mth.IntPow(hitDirCross, 2) / inertia 
                                /* + Mathf.Pow(Extensions.CrossProduct(stickRadiusVector, Tri.HitNormalInverse), 2) / inertia */  //Stick inertia is infinite
                            );
    
            Vector2 impulseV = hitNormal * impulse;
            
            mV        = charMV + impulseV / mass;
            startSpin = (charRadSpin + hitDirCross / inertia * impulse) * Mathf.Rad2Deg / 35f;
        
            flyPath = new FlyPath(startPos, mV);
            
            return this;
        }
    
        
        public override Placement BasicPlacement(float time, bool adjustForOffsets = false)
        {
            Vector3 pos = flyPath.GetPos(time - startTime);
            float angle = startAngle + GPhysics.Get_SpinAngle_Deg(startSpin, time - startTime);
            
            return new Placement(pos, angle);
        }
        
        
        public override Placement FinalPlacement(float time)
        {
            float zShift = spinner.GetZShift(time);
            float dist   = (Level.WallDepth + Level.PlaneOffset + zShift) * startSide.Sign;
            Placement basePlacement = BasicPlacement(time);
            
            return new Placement(basePlacement.pos.SetZ(dist), basePlacement.rot);
        }


        public override Vector2 GetMV(float time)
        {
            return flyPath.GetMV(time - startTime);
        }
        
        
        /*public override bool GetClipBounds(ClipBoundPool pool)
        {
            flyPath = new FlyPath(startPos, mV);
            
            float start = Mathf.Max(startTime, pool.min);
            float end   = Mathf.Min(startTime + 6, pool.max);
            
            for (float timeA = start; timeA < end; timeA += ClipBounds.SearchStep)
            {
                float timeB = Mathf.Min(timeA + ClipBounds.SearchStep, end);
                
                ClipBounds cBounds = pool.Get.Set(timeA, timeB, Type, startSide);
       
            //  Set Bounds  //
                float flightA = timeA - startTime, flightB = timeB - startTime;
                cBounds.bounds.Set(flyPath.GetPos(flightA)).Add(flyPath.GetPos(flightB));

                if (flyPath.apexTime > flightA && flyPath.apexTime <= flightB)
                    cBounds.bounds.Add(flyPath.GetPos(flyPath.apexTime));

                cBounds.bounds.Pad(spinner.size.y * .5f);
            }

            return true;
        }*/
    }
}
using GeoMath;
using UnityEngine;


namespace ShapeStuff
{
    public partial class Shape
    {
        public readonly Segment[] segments;
        private readonly float[]   lerps;
        
        public int   segmentCount;
        public float length;
        public bool  loop, clockwise;
        
        public Bounds2D bounds;

        public Vector2 intersectionPoint;
        
        public Shape()
        {
            const int maxSegments = 30;
            
            segments = new Segment[maxSegments];
            for (int i = 0; i < maxSegments; i++)
                segments[i] = new Segment();
            
            lerps = new float[maxSegments];
        }
        
        
        #region Setup
        
        public void FinishSetup()
        {
            length = 0;
            for (int i = 0; i < segmentCount; i++)
                length += segments[i].length;

            
            float dist = 0;
            for (int i = 0; i < segmentCount; i++)
            {
                float lerpA = dist / length;
                lerps[i] = lerpA;
                
                dist += segments[i].length;
                float lerpB =  dist / length;
                segments[i].SetShapeLerpRange(lerpA, lerpB, i == segmentCount - 1);
            }

            lerps[segmentCount] = 1;

            UpdateBounds();
            
            loop = segmentCount > 0 && GetPoint(0).Same(GetPoint(1));
            
            clockwise = GetRad(0) > GetRad(1);
        }


        private void UpdateBounds()
        {
            bounds = segments[0].bounds;
            for (int i = 1; i < segmentCount; i++)
                bounds = bounds.Add(segments[i].bounds);

            bounds = bounds.Pad(1f);
        }
        
        
        public void Move(Vector2 dir)
        {
            for (int i = 0; i < segmentCount; i++)
                segments[i].Move(dir);

            UpdateBounds();
        }
        
        
        public void RotateAround(Vector2 point, float rad)
        {
            for (int i = 0; i < segmentCount; i++)
                segments[i].RotateAround(point, rad);

            UpdateBounds();
        }
        
        
        public float PossibleThickness
        {
            get
            {
                float thickness = float.MaxValue;
                for (int i = 0; i < segmentCount; i++)
                    segments[i].MaxShapeThickness(ref thickness);
                
                return thickness;
            }
        }
        
        #endregion

        
        #region ShapeLerp Get
        
        public int GetSegmentIndex(float shapeLerp)
        {
        //  TODO? ... why  //
            for (int i = 0; i < segmentCount; i++)
                if (lerps[i] > shapeLerp)
                    return i - 1;
           
            return Mathf.Max(0, segmentCount - 1);
        }


        public float GetSegmentLerp(int segmentIndex, float shapeLerp)
        {
            return Mathf.InverseLerp(lerps[segmentIndex], lerps[segmentIndex + 1], shapeLerp);
        }


        public Vector2 GetPoint(float shapeLerp, float offset = 0)
        {
            int   segmentIndex = GetSegmentIndex(shapeLerp);
            float segmentLerp  = GetSegmentLerp(segmentIndex, shapeLerp);
            return segments[segmentIndex].LerpPos(segmentLerp, offset);
        }


        private float GetRad(float shapeLerp)
        {
            int   segmentIndex = GetSegmentIndex(shapeLerp);
            float segmentLerp  = GetSegmentLerp(segmentIndex, shapeLerp);
            return segments[segmentIndex].LerpRad(segmentLerp);
        }
        
        
        public Vector2 GetDir(float shapeLerp)
        {
            int   segmentIndex = GetSegmentIndex(shapeLerp);
            float segmentLerp  = GetSegmentLerp(segmentIndex, shapeLerp);
            return segments[segmentIndex].LerpDir(segmentLerp);
        }
        
        
        public Vector2 GetNormal(float shapeLerp)
        {
            int   segmentIndex = GetSegmentIndex(shapeLerp);
            float segmentLerp  = GetSegmentLerp(segmentIndex, shapeLerp);
            return segments[segmentIndex].LerpRight(segmentLerp) * (clockwise? -1 : 1);
        }
        
        #endregion

        
        #region Point Info
        
        public Vector2 GetClosestPoint(Vector2 point)
        {
            Vector2 closestPoint = point;
            float minDist = float.MaxValue;

            for (int i = 0; i < segmentCount; i++)
            {
                Vector2 closestSegmentPoint = segments[i].GetClosestPoint(point);
                float dist = (point - closestSegmentPoint).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    closestPoint = closestSegmentPoint;
                }
            }

            return closestPoint;
        }
        
        
        public bool IsInside(Vector2 point)
        {
            if (!loop)
                return false;

            for (int i = 0; i < segmentCount; i++)
                if (!segments[i].OnInsideSide(point, clockwise))
                {
                    float   radius  = segments[i].arc.radius;
                    Vector2 dir     = segments[i].arc.center - point;
                    bool    isClose = dir.sqrMagnitude < radius * radius;
                    if (segments[i] == RaycastSegment(point, isClose? -dir : dir))
                        return false;
                }

            return true;
        }
        
        #endregion


        #region Shapes

        public float GetClosestDistance(Shape otherShape, out Vector2 p1, out Vector2 p2)
        {
            p1 = p2 = V2.zero;
            Vector2 segmentP1 = V2.zero, segmentP2  = V2.zero;
            
            float closestDist = float.MaxValue;

            
            for (int i = 0; i < segmentCount; i++)
            for (int e = 0; e < otherShape.segmentCount; e++)
            {
                float dist = segments[i].DistanceSqr(otherShape.segments[e], closestDist, ref segmentP1, ref segmentP2);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    p1 = segmentP1;
                    p2 = segmentP2;
                }

                if (f.Same(dist, 0))
                    return closestDist;
            }
            
            return closestDist;
        }


        public bool Intersects(Shape otherShape)
        {
            if (!bounds.Intersects(otherShape.bounds))
                return false;
            
            bool self = otherShape == this;
            
            int step = 0;
            for (int i = 0; i < segmentCount; i++)
            {
                for (int e = step; e < otherShape.segmentCount; e++)
                {
                    bool ignore = self && (e == i || e == i.IndexDown(segmentCount) || e == i.IndexUp(segmentCount));

                    if (!ignore && segments[i].Overlap(otherShape.segments[e], ref intersectionPoint))
                        return true;   
                }

                if (self)
                    step++;
            }

            return false;
        }

        #endregion


        #region Raycast

        public bool Raycast(Vector2 root, Vector2 dir, out Vector2 hitPoint)
        {
            bool hit = false;
            float hitDistance = float.MaxValue;
            hitPoint = Vector2.zero;

            for (int i = 0; i < segmentCount; i++)
            {
                Vector2 segmentPoint;
                if (segments[i].RayCast(root, dir, out segmentPoint))
                {
                    hit = true;
                    float dist = (segmentPoint - root).sqrMagnitude;
                    if (dist < hitDistance)
                    {
                        hitDistance = dist;
                        hitPoint = segmentPoint;
                    }
                }
            }

            return hit;
        }


        private Segment RaycastSegment(Vector2 root, Vector2 dir)
        {
            Segment hitSegment = null;
            float hitDistance = float.MaxValue;

            for (int i = 0; i < segmentCount; i++)
            {
                Vector2 segmentPoint;
                if (segments[i].RayCast(root, dir, out segmentPoint))
                {
                    float dist = (segmentPoint - root).sqrMagnitude;
                    if (dist < hitDistance)
                    {
                        hitDistance = dist;
                        hitSegment = segments[i];
                    }
                }
            }

            return hitSegment;
        }

        #endregion
        

        public int GetTesselation(float tesselation)
        {
            int steps = 0;
            for (int i = 0; i < segmentCount; i++)
                steps += segments[i].GetTesselation(tesselation);

            return steps + (loop? 0 : 1);
        }
        
        
        public int GetFacingDirections(float topRatio, float bottomRatio, ref Segment.FacingLerp[] points)
        {
            //Debug.Log((clockwise? "CLOCKWISE" : "ANTI CLOCKWISE").B());
            
            float   normalRad   = GetNormal(0).ToRadian();
            float   bendLerp4   = normalRad / Mathf.PI * 2;
            int     dirStep     = Mathf.FloorToInt(bendLerp4);
            float   lerp        = bendLerp4 - dirStep;
            
            int dir = -1;
            switch (dirStep)
            {
                case 0: dir = lerp < 1f - topRatio ?    0 : 1; break;
                case 1: dir = lerp < topRatio ?         1 : 2; break;
                case 2: dir = lerp < 1f - bottomRatio ? 2 : 3; break;
                case 3: dir = lerp < bottomRatio ?      3 : 0; break;
            }
            
            int index = 0;
            for (int i = 0; i < segmentCount; i++)
                index = segments[i].GetFacingChanges(clockwise, topRatio, bottomRatio, ref points, ref dir, index);
            
            return index;
        }
    }
}




   




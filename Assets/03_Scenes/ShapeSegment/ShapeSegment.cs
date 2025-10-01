using GeoMath;
using UnityEngine;


namespace ShapeStuff
{
    public partial class Shape
    {
        public class Segment
        {
            public  Vector2 pointA, pointB;
            public  float   radA, length;
            private float   shapeLerpA, shapeLerpB, radB;
            
            private bool straight, end;

            private int maximaCount;
            private readonly float[]  maximaLerps = new float[4];
            public Bounds2D bounds;

            private Line line;
            public  Arc  arc;
            
            
            public float Bend { get { return straight ? 0 : arc.bend; } }

            
            #region Setup


            public Segment Randomize()
            {
                pointA = Random.insideUnitCircle * 3;
                radA   = Random.Range(-Mathf.PI, Mathf.PI);

                return this;
            }
            

            public Segment SetPointDir(Vector2 pointA, float radA)
            {
                this.pointA = pointA;
                this.radA   = radA;

                return this;
            }


            public Segment Connect(Segment before)
            {
                pointA = before.pointB;
                radA   = before.radB;

                return this;
            }


            public void SetLengthBend(float length, float bend)
            {
                this.length = length;
                

                UpdateSegment(bend);
            }
            
            
            public void SetShapeLerpRange(float shapeLerpA, float shapeLerpB, bool end)
            {
                this.shapeLerpA = shapeLerpA;
                this.shapeLerpB = shapeLerpB;
                this.end   = end;
            }


            private void UpdateSegment(float bend)
            {
                straight = f.Same(bend, 0);
                
            //  Line and Arc  //
                line = new Line(pointA, pointA + Vector2.up.RotRad(radA).SetLength(length));
                if(!straight)
                    arc = new Arc(line, bend, 0);
                
                radB         = radA + arc.bend * 2 * Mathf.PI;
                pointB       = LerpPos(1);
                
            //  Check for included Maxima  //
                maximaCount = 0;

                if (!straight)
                {
                    float maxLerp = Mathf.Abs(arc.bend);
                    for (int i = 0; i < 4; i++)
                    {
                        float bendLerp = ToRadLerp(.5f * Mathf.PI * i);
                        if (bendLerp <= maxLerp)
                        {
                            float mLerp = Mathf.InverseLerp(0, maxLerp, bendLerp);
                            maximaLerps[maximaCount++] = mLerp;
                        }
                    }
                }

            //  Update Bounds  //
                bounds = new Bounds2D(pointA).Add(pointB);
                for (int i = 0; i < maximaCount; i++)
                    bounds = bounds.Add(LerpPos(maximaLerps[i]));
            }
            
            
            public void Move(Vector2 dir)
            {
                pointA += dir;
                
                UpdateSegment(Bend);
            }


            public void RotateAround(Vector2 point, float rad)
            {
                pointA = (pointA - point).RotRad(rad) + point;
                radA  += rad;
                
                UpdateSegment(Bend);
            }
            
            
            #endregion


            #region ShapeLerp Get

            
            public Vector2 LerpPos(float segmentLerp)
            {
                return straight ? line.LerpPos(segmentLerp) : arc.LerpPos(segmentLerp);
            }
            
            
            public Vector2 LerpPos(float segmentLerp, float offset)
            {
                return straight ? line.LerpPos(segmentLerp) + line.dir.normalized.Rot90(false)      * offset 
                                :  arc.LerpPos(segmentLerp) + arc.LerpDir(segmentLerp).Rot90(false) * offset;
            }


            public Vector2 LerpDir(float segmentLerp)
            {
                return straight ? line.dir.normalized : arc.LerpDir(segmentLerp);
            }


            public Vector2 LerpRight(float segmentLerp)
            {
                return straight ? line.dir.normalized.Rot90(false) : arc.LerpDir(segmentLerp).Rot90(false);
            }

            
            #endregion


            #region Misc

            
            public float LerpRad(float segmentLerp)
            {
                return Mathf.Lerp(radA, radB, segmentLerp);
            }


            public void FillBounds(float boundLerpA, float boundLerpB, ref Bounds2D boundsToFill)
            {
            //  FitsShapeLerpRange  //
                if (shapeLerpA > boundLerpB && (end ? shapeLerpB < boundLerpA : shapeLerpB <= boundLerpA))
                    return;
                
            //  Add Min and Max Points to Bounds  //
                float min = Mathf.InverseLerp(shapeLerpA, shapeLerpB, boundLerpA);
                float max = Mathf.InverseLerp(shapeLerpA, shapeLerpB, boundLerpB);
                
                boundsToFill = boundsToFill.Add(LerpPos(min)).Add(LerpPos(max));

            //  Also add maxima that fall inside range  //
                for (int i = 0; i < maximaCount; i++)
                    if (maximaLerps[i] >= min && maximaLerps[i] < max)
                        boundsToFill = boundsToFill.Add(LerpPos(maximaLerps[i]));
            }


            public int GetTesselation(float tesselation)
            {
                return straight? 1 : Mathf.Max(1, Mathf.CeilToInt(Mathf.Abs(arc.bend) * tesselation));
            }

            
            #endregion


            #region Private Rad Functions
            
            
            private float ToRadLerp(float rad)
            {
                if (arc.bend < 0)
                    rad += Mathf.PI;
                
                float full     = radA + Mathf.Sign(arc.bend) * 2 * Mathf.PI;
                float rangeRad = Mth.Repeat(radA, full, rad);

                return Mathf.InverseLerp(radA, full, rangeRad);
            }


            private bool PointLerpInRange(Vector2 point)
            {
                float pointLerp = straight ? line.GetClosestLerp(point) : arc.GetClosestLerp(point);
                return pointLerp >= 0 && pointLerp <= 1;
            }
            
            
            #endregion
            
            
            #region Facing Directions
            
            public int GetFacingChanges(bool clockwise, float topRatio, float bottomRatio, ref FacingLerp[] points, ref int dir, int index)
            {
                float maxLerp = Mathf.Abs(arc.bend);
                int   startDir = dir;

                bool  flip = arc.bend > 0 && !clockwise || arc.bend < 0 && clockwise;
                float tR   = flip? topRatio : bottomRatio;
                float bR   = flip? bottomRatio : topRatio;

                for (int i = 0; i < 4; i++)
                {
                    int checkDir = arc.bend > 0 ? (startDir + 1 + i) % 4 : (startDir - i) % 4;

                    const float FullRad = 2 * Mathf.PI;
                    
                    float radLerp;
                    switch (checkDir)
                    {
                        default: radLerp = ToRadLerp( .25f * (1f - tR)         * FullRad);  break;
                        case 2:  radLerp = ToRadLerp((.25f + .25f * tR)        * FullRad);  break;
                        case 3:  radLerp = ToRadLerp((.5f  + .25f * (1f - bR)) * FullRad);  break;
                        case 0:  radLerp = ToRadLerp((.75f + .25f * bR)        * FullRad);  break;
                    }
                    
                    if(radLerp <= maxLerp)
                    {
                        int newDir = arc.bend > 0 ? checkDir : (checkDir + 1) % 4;
                        
                        if(clockwise)
                            newDir = (newDir + 2) % 4;

                        float shapeLerp = Mathf.Lerp(shapeLerpA, shapeLerpB, radLerp / maxLerp);
                        if (index == 0 || shapeLerp > points[index - 1].lerp)
                        {
                            points[index] = new FacingLerp(Mathf.Lerp(shapeLerpA, shapeLerpB, radLerp / maxLerp), newDir);
                            index++;
                            dir = newDir;
                        }
                    }
                }
                return index;
            }
            
            
            public struct FacingLerp
            {
                public readonly float lerp;
                public readonly int   sideID;

                public FacingLerp(float lerp, int sideID)
                {
                    this.lerp   = lerp;
                    this.sideID = sideID;
                }
            }
            
            #endregion

            
            #region Point Info

            public Vector2 GetClosestPoint(Vector2 point)
            {
                return straight ? line.ClosestPoint(point) : arc.GetClosestPoint(point);
            }
            
            
            private bool PointProjectionHit(Vector2 point, out Vector2 rangePoint)
            {
                Vector2 dir = point - arc.center;
                
                if (PointLerpInRange(point))
                {
                    rangePoint = arc.center + dir.SetLength(arc.radius);
                    return true;
                }

                if(PointLerpInRange(arc.center - dir))
                {
                    rangePoint = arc.center - dir.SetLength(arc.radius);
                    return true;
                }

                rangePoint = point;
                return false;
            }
            
            
            public bool OnInsideSide(Vector2 point, bool clockwise)
            {
                if (PointLerpInRange(point))
                    if (straight)
                    {
                        float radAngle = line.dir.RadAngle(point - line.l1);
                        
                        if (Mathf.Approximately(radAngle, 0))
                            return true;

                        return clockwise ? radAngle < 0 : radAngle > 0;

                    }
                    else
                    {
                        float pointSqrDist = (point - arc.center).sqrMagnitude;
                        float radiusSqr = arc.radius * arc.radius;
                        
                        if (Mathf.Approximately(pointSqrDist, radiusSqr))
                            return true;
                        
                        bool isClose        = pointSqrDist < radiusSqr;
                        bool needsToBeClose = !clockwise && arc.bend > 0 || clockwise && arc.bend < 0;

                        return isClose == needsToBeClose; 
                    }

                return true;
            }
            
            #endregion


            #region Segment

            public bool Overlap(Segment otherSegment, ref Vector2 iPoint)
            {
                if (!bounds.Intersects(otherSegment.bounds))
                    return false;
                
                Vector2 c1 = arc.center, c2 = otherSegment.arc.center;
                float   r1 = arc.radius, r2 = otherSegment.arc.radius;
                
                Vector2 p1, p2;
                int hits;
                if(!new Circle(c1, r1).Contact(new Circle(c2, r2), out hits, out p1, out p2))
                    return false;


                if (PointLerpInRange(p1) && otherSegment.PointLerpInRange(p1))    //    p1    //
                {
                    iPoint = p1;
                    return true;
                }
                
                if (hits == 2 && PointLerpInRange(p2) && otherSegment.PointLerpInRange(p2))    //    p2    //
                {
                    iPoint = p2;
                    return true;
                }

                return false;
            }


            public float DistanceSqr(Segment otherSegment, float closestDist, ref Vector2 pA, ref Vector2 pB)
            {
                if (bounds.DistanceSqr(otherSegment.bounds) > closestDist)
                    return float.MaxValue;
                
                
            //  Overlap  //
                Vector2 refVar = V2.zero;
                if (Overlap(otherSegment, ref refVar))
                    return 0;
                
                
            //  EndPoints Check  //
                {
                    float x    = pointA.x - otherSegment.pointA.x;
                    float y    = pointA.y - otherSegment.pointA.y;
                    float dist = x * x + y * y;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        pA = pointA;
                        pB = otherSegment.pointA;
                    }
                }
                {
                    float x    = pointA.x - otherSegment.pointB.x;
                    float y    = pointA.y - otherSegment.pointB.y;
                    float dist = x * x + y * y;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        pA = pointA;
                        pB = otherSegment.pointB;
                    }
                }
                {
                    float x    = pointB.x - otherSegment.pointA.x;
                    float y    = pointB.y - otherSegment.pointA.y;
                    float dist = x * x + y * y;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        pA = pointB;
                        pB = otherSegment.pointA;
                    }
                }
                {
                    float x    = pointB.x - otherSegment.pointB.x;
                    float y    = pointB.y - otherSegment.pointB.y;
                    float dist = x * x + y * y;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        pA = pointB;
                        pB = otherSegment.pointB;
                    }
                }
                
                
            //  EndPoint To Center  //
                Vector2 rangePoint;
                if (PointProjectionHit(otherSegment.pointA, out rangePoint))
                {
                    float x    = otherSegment.pointA.x - rangePoint.x;
                    float y    = otherSegment.pointA.y - rangePoint.y;
                    float dist = x * x + y * y;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        pA = rangePoint;
                        pB = otherSegment.pointA;
                    }
                }
                if (PointProjectionHit(otherSegment.pointB, out rangePoint))
                {
                    float x    = otherSegment.pointB.x - rangePoint.x;
                    float y    = otherSegment.pointB.y - rangePoint.y;
                    float dist = x * x + y * y;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        pA = rangePoint;
                        pB = otherSegment.pointB;
                    }
                }
                if (otherSegment.PointProjectionHit(pointA, out rangePoint))
                {
                    float x    = pointA.x - rangePoint.x;
                    float y    = pointA.y - rangePoint.y;
                    float dist = x * x + y * y;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        pA = rangePoint;
                        pB = pointA;
                    }
                }
                if (otherSegment.PointProjectionHit(pointB, out rangePoint))
                {
                    float x    = pointB.x - rangePoint.x;
                    float y    = pointB.y - rangePoint.y;
                    float dist = x * x + y * y;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        pA = rangePoint;
                        pB = pointB;
                    }
                }


            //  Center To Center  //
                {
                    Vector2  c1 = arc.center;
                    Vector2  c2 = otherSegment.arc.center;
                    float    r1 = arc.radius;
                    float    r2 = otherSegment.arc.radius;
                    
                    bool aFront = PointLerpInRange(c2);
                    bool aBack  = PointLerpInRange(c1 + (c1 - c2));
                    bool bFront = otherSegment.PointLerpInRange(c1);
                    bool bBack  = otherSegment.PointLerpInRange(c2 + (c2 - c1));
                    
                    
                    

                    if (aFront && bFront)
                    {
                        Vector2 a = c1 + (c2 - c1).SetLength(r1);
                        Vector2 b = c2 + (c1 - c2).SetLength(r2);
                        float dist = (b - a).sqrMagnitude;
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            pA = a;
                            pB = b;
                        }
                    }
                    if (aFront && bBack)
                    {
                        Vector2 a = c1 + (c2 - c1).SetLength(r1);
                        Vector2 b = c2 + (c2 - c1).SetLength(r2);
                        float dist = (b - a).sqrMagnitude;
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            pA = a;
                            pB = b;
                        }
                    }
                    if (aBack && bBack)
                    {
                        Vector2 a = c1 + (c1 - c2).SetLength(r1);
                        Vector2 b = c2 + (c2 - c1).SetLength(r2);
                        float dist = (b - a).sqrMagnitude;
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            pA = a;
                            pB = b;
                        }
                    }
                    if (aBack && bFront)
                    {
                        Vector2 a = c1 + (c1 - c2).SetLength(r1);
                        Vector2 b = c2 + (c1 - c2).SetLength(r2);
                        float dist = (b - a).sqrMagnitude;
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            pA = a;
                            pB = b;
                        }
                    }
                }

                return closestDist;
            }


            public bool RayCast(Vector2 root, Vector2 dir, out Vector2 hitPoint)
            {
                return straight? line.RayCast(root, dir, out hitPoint) : 
                                  arc.RayCast(root, dir, out hitPoint);
            }

            #endregion


            public void MaxShapeThickness(ref float thickness)
            {
                if (!straight && arc.radius < thickness)
                    thickness = arc.radius;
            }


        //  DEV DRAW  //
            public void DrawMaxima()
            {
                for (int i = 0; i < maximaCount; i++)
                    DRAW.Circle(LerpPos(maximaLerps[i]), .03f, 12).SetColor(COLOR.yellow.fresh).Fill(.1f);
            }


            public void DrawLine()
            {
                return;
                if (straight)
                    line.Draw().SetColor(Color.cyan);
                else
                    arc.DrawShell(.2f).SetColor(Color.cyan);
            }


            public void DrawShell(float thickness)
            {
                if (straight)
                {
                    Vector2 right = line.dir.Rot90().SetLength(thickness * .5f);
                    line.Move( right).Draw().SetColor(COLOR.green.lime);
                    line.Move(-right).Draw().SetColor(COLOR.blue.cornflower);
                }
                else
                {
                    arc.Shift(-thickness * .5f).Draw(100).SetColor(COLOR.green.lime);
                    arc.Shift( thickness * .5f).Draw(100).SetColor(COLOR.blue.cornflower);
                }
            }
        }
    }
}
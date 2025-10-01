using UnityEngine;


public class CircleIntersection : MonoBehaviour
{
    public float offset;
    public float segmentLength;
    public float circleDistance;
    public float circleRadius;
    
    public float angle;
    public bool left;
    public bool max;

    private const int numberOfPoints = 100;
    
    private void Awake()
    {
        DRAW.Enabled = DRAW.EditorDraw = true;
    }

    //    http://csharphelper.com/blog/2014/09/determine-where-two-circles-intersect-in-c/
   /* private static int FindCircleCircleIntersections(Vector2 m1, float r1,Vector2 m2, float r2)
    {
        float dist = Vector2.Distance(m1, m2);

        // See how many solutions there are.
        if (dist > r1 + r2 || dist < Mathf.Abs(r1 - r2))
            return 0;
      
        // Find a and h.
        float a = (Mathf.Pow(r1,2) - Mathf.Pow(r2,2) + Mathf.Pow(dist,2)) / (2 * dist);
        float h = Mathf.Sqrt(r1 * r2 - a * a);

        Vector2 c = m1 + a * (m2 - m1) / dist;
        

        Vector2 p1 = new Vector2(c.x + h * (m2.y - m1.y) / dist, c.y - h * (m2.x - m1.x) / dist);
        Vector2 p2 = new Vector2(c.x - h * (m2.y - m1.y) / dist, c.y + h * (m2.x - m1.x) / dist);
        
        return 2;
    }*/


    public static Vector2 GetPointAlongPath(Vector2 startPoint, Vector2 up, float segmentLength, float circleDistance, float circleRadius, bool left, bool max)
    {
        Vector2 circleMidPoint = startPoint + up * circleDistance + up.Rot90(left) * circleRadius;
        Vector2 lineBetween    = circleMidPoint - startPoint;
        Vector2 lineDir        = lineBetween.normalized;
        float   distance       = lineBetween.magnitude;
        
        DRAW.Vector(startPoint,up* circleDistance).SetColor(COLOR.grey.mid);
        DRAW.Line(numberOfPoints, i => circleMidPoint.V3() + V3.up.RotZ(360f / (numberOfPoints - 1) * i, circleRadius)).SetColor(COLOR.grey.mid);

        if (max)
            segmentLength = distance + circleRadius;
                
        if (segmentLength <= circleDistance)
            return startPoint + up * segmentLength;
        
     
        
        float between = (Mth.IntPow(segmentLength, 2) - Mth.IntPow(circleRadius, 2) + Mth.IntPow(distance, 2))  / (2 * distance);
        float offset  = Mathf.Sqrt( Mth.IntPow(segmentLength, 2) - Mth.IntPow(between, 2));
        
        
        return startPoint + lineDir * between + lineDir.Rot90(!left) * offset;
    }

    
    private void Update()
    {
        circleDistance = Mth.SmoothPP(6, 0, Time.realtimeSinceStartup * .2f);
        circleRadius   = Mth.SmoothPP(2.45f, 2.25f, Time.realtimeSinceStartup * 2f);
        
        Vector2 up = V2.up.Rot(angle);
        Vector2 startPos = up * offset;
        Vector2 point = GetPointAlongPath(startPos, up, segmentLength, circleDistance - offset, circleRadius, left, max);
        
        DRAW.Vector(startPos, point - startPos).SetColor(COLOR.green.lime).SetDepth(-1);
    }
}

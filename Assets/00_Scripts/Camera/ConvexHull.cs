using GeoMath;
using UnityEngine;


public static class ConvexHull
{
    private static readonly int[] validPoints = new int[8];
    private static int querryIndex;
    
    private static readonly Vector2[] checkPoints = new Vector2[8];
    private static readonly float[] checkAngles = new float[8];
    
    
    
    public static int GetHull(Vector2[] points, Vector2[] hull)
    {
        querryIndex++;
        
        int pointCount = points.Length;
        int hullCount = 0;

        int startPoint = 0;
        float startX = float.MaxValue;
        
        for (int e = 0; e < pointCount; e++)
            if (points[e].x < startX)
            {
                startX     = points[e].x;
                startPoint = e;
            }
        
        hull[hullCount++] = points[startPoint];

        Vector2 compareDir = Vector2.up;
        
        int currentPoint = startPoint;
        while (true)
        {
            int   nextPoint = currentPoint;
            float bestAngle = float.MaxValue;

            Vector2 currentP = points[currentPoint];
            for (int e = 1; e < pointCount; e++)
            {
                int index = (currentPoint + e) % pointCount;
                
                if(validPoints[index] == querryIndex)
                    continue;
                
                //  GetAngle  //
                Vector2 checkDir = points[index] - currentP;
                
                float dot   = compareDir.x * checkDir.x + compareDir.y * checkDir.y;
                float det   = compareDir.x * checkDir.y - compareDir.y * checkDir.x;
                float angle = -Mathf.Atan2(det, dot);

                if (angle > 0 && angle < bestAngle)
                {
                    bestAngle = angle;
                    nextPoint = index;
                }
            }

            if (nextPoint == startPoint)
                break;

            Vector2 nextP = points[nextPoint];
            compareDir    = nextP - currentP;
            currentPoint  = nextPoint;
            hull[hullCount++] = nextP;

            validPoints[currentPoint] = querryIndex;
        }

        return hullCount;
    }
    
    
    public static int GetHull(Quad a, Quad b, Vector2[] hull)
    {
        querryIndex++;

        int count = 0;
        for (int i = 0; i < 4; i++)
            if (!b.Contains(a[i]))
                checkPoints[count++] = a[i];
        
        for (int i = 0; i < 4; i++)
            if (!a.Contains(b[i]))
                checkPoints[count++] = b[i];

        Vector2 center = Vector3.zero;
        for (int i = 0; i < count; i++)
            center += checkPoints[i];

        center /= count;

        for (int i = 0; i < count; i++)
            checkAngles[i] = (checkPoints[i] - center).ToRadian();

        querryIndex++;
        
       
        for (int i = 0; i < count; i++)
        {
            int best = 0;
            float max = float.MinValue;
            
            for (int e = 0; e < count; e++)
                if (validPoints[e] != querryIndex && checkAngles[e] > max)
                {
                    best = e;
                    max = checkAngles[e];
                }

            validPoints[best] = querryIndex;
            hull[i] = checkPoints[best];
        }
        
        return count;
    }
}
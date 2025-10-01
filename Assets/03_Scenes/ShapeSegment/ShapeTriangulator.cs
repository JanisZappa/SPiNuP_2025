using System.Collections.Generic;
using ShapeStuff;
using UnityEngine;


public static class ShapeTriangulator
{
    public static int pointCount;
    public static readonly Vector3[] pnts = new Vector3[1000];
    public static readonly List<int> triangles = new List<int>(1000);
    
    
    public static void FillShape(Shape shape, float offset)
    {
        const int tesselation = 8;
        pointCount = shape.GetTesselation(tesselation);

        offset *= shape.clockwise ? 1 : -1;

        int pointIndex = 0;
        for (int i = 0; i < shape.segmentCount; i++)
        {
            int sIndex = shape.clockwise ? shape.segmentCount - 1 - i : i;
            Shape.Segment segment = shape.segments[sIndex];

            int segmentSteps = segment.GetTesselation(tesselation);
            float lerpStep = 1f / segmentSteps;
            for (int e = 0; e < segmentSteps; e++)
            {
                float segmentLerp = shape.clockwise ? 1 - lerpStep * e : lerpStep * e;
                pnts[pointIndex++] = segment.LerpPos(segmentLerp, offset);
            }
        }
        
        triangles.CopyFrom(Triangulator.Fill(pnts, pointCount));
    }
}

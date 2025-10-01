using GeoMath;
using UnityEngine;


public static class DrawHelp
{
    private const int shellSteps = 12, arcSteps = 50;
    
    
//  Line //
    public static DRAW.Shape Draw(this Line line)
    {
        return DRAW.Vector(line.l1, line.dir);
    }
    
    
    public static DRAW.Shape DrawTips(this Line line)
    {
        return DRAW.Circle(line.l1, .075f, 20).SetSub(DRAW.Arrow(line.GetL2(), line.dir.normalized * .15f, .2f));
    }
    
    
    public static DRAW.Shape DrawShell(this Line line, float thickness, bool drawLine = false)
    {
        Vector2 pointer = line.dir.Rot90().SetLength(thickness);

        DRAW.Shape shape = DRAW.Shape.Get(shellSteps * 2 + 1);

        const float angle = 180f / (shellSteps - 1);
        for (int i = 0; i < shellSteps; i++)
            shape.Set(i, line.l1 + line.dir + pointer.Rot(180 + angle * i));
        
        for (int i = 0; i < shellSteps; i++)
            shape.Set(i + shellSteps, line.l1 + pointer.Rot(angle * i));

        shape.Copy(0, shellSteps + shellSteps);

        if (drawLine)
            shape.SetSub(line.Draw());
        
        return shape;
    }
    
    
//  Circle  //
    public static DRAW.Shape Draw(this Circle circle, int points)
    {
        return DRAW.Circle(circle.center, circle.radius, points);
    }
    
    
//  Arc  //
    public static DRAW.Shape Draw(this Arc arc, int points)
    {
        return DRAW.Arc(arc.center, arc.radius, arc.bend, arc.angle, points);
    }
    

    public static DRAW.Shape DrawTips(this Arc arc)
    {
        return DRAW.Circle(arc.LerpPos(0), .075f, 20).SetSub(DRAW.Arrow(arc.LerpPos(1), arc.LerpDir(1) * .15f, .2f));
    }
    
    
    public static DRAW.Shape DrawShell(this Arc arc, float thickness, bool drawArc = false)
    {
        int pointIndex = 0;
        DRAW.Shape shape = DRAW.Shape.Get(shellSteps * 2 + arcSteps * 2 + 1);
        const float angleStep = 180f / (shellSteps - 1);

        float signedRadius = arc.SignedRadius;
        
        
        Quaternion rotA = Rot.Z(arc.angle);
        Vector2 pointer = rotA * Vector2.right.SetLength(thickness);
        Vector2 p1 = arc.center + (rotA * Vector2.up.SetLength(signedRadius)).V2();

        float sign = Mathf.Sign(signedRadius);
        for (int i = 0; i < shellSteps; i++)
            shape.Set(pointIndex++, p1 + pointer.Rot(-90 + angleStep * i * sign));

        
        float arcAngleStep = Mathf.Abs(arc.bend) * 360 / arcSteps;
        pointer = Vector2.up.SetLength(signedRadius + thickness);
        for (int i = 0; i < arcSteps; i++)
            shape.Set(pointIndex++, arc.center + (Rot.Z(arc.angle + arcAngleStep * i) * pointer).V2());
        
        
        Quaternion rotB = Rot.Z(arc.angle + Mathf.Abs(arc.bend) * 360);
        pointer = rotB * Vector2.right.SetLength(thickness);
        Vector2 p2 = arc.center + (rotB * Vector2.up.SetLength(signedRadius)).V2();
        for (int i = 0; i < shellSteps; i++)
            shape.Set(pointIndex++, p2 + pointer.Rot(-270 + angleStep * i * sign));
        
        
        pointer = Vector2.up.SetLength(signedRadius - thickness);
        for (int i = 0; i < arcSteps; i++)
            shape.Set(pointIndex++, arc.center + (Rot.Z(arc.angle + Mathf.Abs(arc.bend) * 360 - arcAngleStep * i) * pointer).V2());
        
        shape.Copy(0, shellSteps * 2 + arcSteps * 2);

        if (drawArc)
            shape.SetSub(arc.Draw(100));
        
        return shape;
    }


//  FlyPath  //
    public static DRAW.Shape Draw(this FlyPath flypath, float start, float end, int steps = 100, bool extra = false)
    {
        float step = (end - start) / (steps - 1);
        
        DRAW.Shape shape = DRAW.Shape.Get(steps);

        for (int i = 0; i < steps; i++)
            shape.Set(i, flypath.GetPos(start + i * step));

        if (extra)
        {
            DRAW.Shape parent = shape;
            int min = Mathf.CeilToInt(start);
            int max = Mathf.CeilToInt(end);
            for (int i = min; i < max; i++)
            {
                Vector2 dir = flypath.GetMV(i).Rot90().normalized;
                DRAW.Shape line = DRAW.Vector(flypath.GetPos(i) - dir * .1f, dir * .2f);
                parent.SetSub(line);
                parent = line;
            }
            
            
            DRAW.Shape tip = DRAW.Arrow(flypath.GetPos(end), flypath.GetMV(end).normalized * .2f, .2f);
            parent.SetSub(tip);
            
            if(flypath.apexTime >= start && flypath.apexTime <= end)
                tip.SetSub(DRAW.Circle(flypath.GetPos(flypath.apexTime), .1f, 20));
        }

        return shape;
    }
    
    
//  Bounds2D  //
    public static DRAW.Shape Draw(this Bounds2D bounds, float scale = 1)
    {
        return DRAW.Rectangle(bounds.Center, bounds.Size * scale);
    }
    
    public static DRAW.Shape DrawSubDiv(this Bounds2D bounds, int xSplits, int ySplits, float scale = 1)
    {
        Vector2 min = bounds.BL, size = bounds.Size;
        
        xSplits++;
        ySplits++;

        float xStep = size.x / xSplits, yStep = size.y / ySplits;
        size = new Vector2(xStep, yStep);
        
        Side side = GameCam.CurrentSide;
        DRAW.Shape shape = null;
        for (int y = 0; y < ySplits; y++)
            for (int x = 0; x < xSplits; x++)
            {
                Vector2 center = min + new Vector2((x + .5f) * xStep, (y + .5f) * yStep);
                
                Bounds2D b = new Bounds2D(center - size * .5f).Add(center + size * .5f);
                Search.Items(b, side, GTime.Now, Mask.Debug);
                bool foundSomething = Search.itemCount > 0;
                if (!foundSomething)
                {
                    Search.Tracks(b, side);
                    for (int i = 0; i < Search.trackCount; i++)
                        if (Search.boundTracks[i].AnyItemMatchesMask(Mask.Debug))
                        {
                            foundSomething = true;
                            break;
                        }
                }
                
                if(foundSomething)
                    shape = DRAW.Rectangle(center, size * scale).SetSub(shape);
            }    
            
        return shape ?? DRAW.NullShape;
    }
}

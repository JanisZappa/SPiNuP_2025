using GeoMath;
using UnityEngine;


namespace ShapeStuff
{
    public class ShapeLineDrawer
    {
        public ShapeLineDrawer(int pointCount = 200)
        {
            shapePointCount = pointCount;
            shapePoints     = new Vector2[shapePointCount];
            shapeNormals    = new Vector2[shapePointCount];
			
            fractionBounds = new Bounds2D[10];
            for (int i = 0; i < fractionBounds.Length; i++)
                fractionBounds[i] = new Bounds2D();
        }
		
		
        private readonly Vector2[] shapePoints;
        private readonly int shapePointCount;

        private readonly Vector2[] shapeNormals;
		
        private bool loop, clockwise, active;

        private Color lineColor;
		
        private readonly Bounds2D[] fractionBounds;
		
		
        public void SetShape(Shape shape, Color lineColor)
        {
            this.lineColor = lineColor;
            active = shape.segmentCount > 0;


            float divider = 1f / (shapePointCount - 1);
            for (int i = 0; i < shapePointCount; i++)
            {
                shapePoints[i]  = shape.GetPoint (Mathf.Clamp(i * divider, 0, .99999f));
                shapeNormals[i] = shape.GetNormal(Mathf.Clamp(i * divider, 0, .99999f));
            }

            loop      = shape.loop;
            clockwise = shape.clockwise;
			
			
            float fraction = 1f / fractionBounds.Length;
            for (int i = 0; i < fractionBounds.Length; i++)
            {
                float startLerp = i * fraction;
                float endLerp   = startLerp + fraction;

                Bounds2D bounds = fractionBounds[i];
                for (int e = 0; e < shape.segmentCount; e++)
                    shape.segments[e].FillBounds(startLerp, endLerp, ref bounds);

                fractionBounds[i] = bounds.Pad(.1f);
            }
        }
		
		
        public void DrawLine()
        {
            if(active)
                DRAW.Line(shapePoints).SetColor(lineColor);
        }


        public void DrawArrows()
        {
            if (loop)
            {
                Color c = clockwise ? COLOR.green.spring.A(.5f) : COLOR.red.tomato.A(.5f);
                for (int i = 0; i < shapePointCount; i += 4)
                    DRAW.Arrow(shapePoints[i] + shapeNormals[i] * .15f, shapeNormals[i] * .2f, .05f).SetColor(c);
            }
        }
		
		
        public static void DrawPoints(Shape shape, bool simple = false, ShapeLineDrawer drawer = null)
        {
            if (simple)
            {
                Color c = drawer != null ? drawer.lineColor.A(.5f) : Color.white.A(.6f);
				
                for (int i = 0; i < shape.segmentCount; i++)
                {
                    DRAW.Circle(shape.segments[i].pointA, .03f).SetColor(c).Fill(.6f);
                    if(i >= shape.segmentCount - 1 && !shape.loop)
                        DRAW.Circle(shape.segments[i].pointB, .03f).SetColor(c).Fill(.6f);
                }
            }
            else
                for (int i = 0; i < shape.segmentCount; i++)
                {
                    
                    DRAW.ZappCircle(shape.segments[i].pointA, .08f, .05f, 10).SetColor(i == 0? COLOR.green.lime : COLOR.yellow.fresh).Fill(.1f);
	
                    shape.segments[i].DrawMaxima();

                    if (i >= shape.segmentCount - 1 && !shape.loop)
                        DRAW.ZappCircle(shape.segments[i].pointB, .08f, .05f, 10).SetColor(COLOR.yellow.fresh).Fill(.1f);
                }
        }


        public static void DrawCenters(Shape shape)
        {
            for (int i = 0; i < shape.segmentCount; i++)
            {
                Vector2 c = shape.segments[i].arc.center;
                DRAW.Circle(c, .02f, 12).SetColor(Color.white).Fill(1);
                DRAW.Vector(c, shape.segments[i].LerpPos(.5f) - c).SetColor(Color.white.A(.25f));
                DRAW.Vector(c, shape.segments[i].LerpPos(0) - c).SetColor(Color.white.A(.1f));
                DRAW.Vector(c, shape.segments[i].LerpPos(1) - c).SetColor(Color.white.A(.1f));
            }
        }
		
		
        public void DrawFractions(float lerp)
        {
            float fraction = 1f / fractionBounds.Length;
            for (int i = 0; i < fractionBounds.Length; i++)
            {
                float startLerp = i * fraction;
                float endLerp   = startLerp + fraction;
                Color c = startLerp <= lerp && endLerp >= lerp ? Color.white : Color.white.A(.2f);
                DRAW.Rectangle(fractionBounds[i].Center, fractionBounds[i].Size).SetColor(c);
            }
        }


        public static void DrawBounds(Shape shape)
        {
            for (int i = 0; i < shape.segmentCount; i++)
                DRAW.Rectangle(shape.segments[i].bounds.Center, shape.segments[i].bounds.Size).SetColor(Color.white.A(.2f));
        }


        public void SetLineColor(Color color)
        {
            lineColor = color;
        }
    }
}
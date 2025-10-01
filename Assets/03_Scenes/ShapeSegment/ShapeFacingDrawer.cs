using ShapeStuff;
using UnityEngine;


public class ShapeFacingDrawer
{
	private Shape.Segment.FacingLerp[] facings = new Shape.Segment.FacingLerp[200];
	private int facingCount;

	private float topRatio = .5f, bottomRatio = .5f;
	
	
	public bool Active { get { return facingCount > 0; } }
	
	

	
	public void UpdateRatios(Shape shape)
	{
		bool input = false;
		if (Input.GetKey(KeyCode.Alpha1))
		{
			topRatio = Mathf.Clamp(topRatio - Time.deltaTime * .5f, .001f, .999f);
			input = true;
		}
		if (Input.GetKey(KeyCode.Alpha3))
		{
			topRatio = Mathf.Clamp(topRatio + Time.deltaTime * .5f, .001f, .999f);
			input = true;
		}
		if (Input.GetKey(KeyCode.Alpha4))
		{
			bottomRatio = Mathf.Clamp(bottomRatio - Time.deltaTime * .5f, .001f, .999f);
			input = true;
		}
		if (Input.GetKey(KeyCode.Alpha6))
		{
			bottomRatio = Mathf.Clamp(bottomRatio + Time.deltaTime * .5f, .001f, .999f);
			input = true;
		}
		if (Input.GetKey(KeyCode.Alpha2))
		{
			topRatio = .5f;
			input = true;
		}
		if (Input.GetKey(KeyCode.Alpha5))
		{
			bottomRatio = .5f;
			input = true;
		}
		if(input)
			UpdateFacings(shape);
	}

	
	public void UpdateFacings(Shape shape)
	{
		facingCount = shape.GetFacingDirections(topRatio, bottomRatio, ref facings);
	}
	
	
	public void DrawFacings(Shape shape)
	{
		if(!shape.loop)
			return;
		
		for (int i = 0; i < facingCount; i++)
		{
			Color c;
			switch (facings[i].sideID)
			{
				default: c = Color.Lerp(Color.red, Color.blue, .1f); break;
				case 1:  c = Color.green; break;
				case 2:  c = Color.yellow; break;
				case 3:  c = Color.Lerp(Color.cyan, Color.blue, .4f); break;
			}

			float length     = (i == facingCount - 1 ? facings[0].lerp + 1 : facings[i + 1].lerp) - facings[i].lerp;
			int   steps      = Mathf.Max(2, Mathf.CeilToInt(length * 300));
			float stepLength = length / (steps - 1);
			float startLerp  = facings[i].lerp;
			const float pad  = 0;
			float padding    = shape.clockwise ? -pad : pad;
			DRAW.Line(steps, e => shape.GetPoint(Mathf.Repeat(startLerp + e * stepLength, 1))).SetColor(c);


			float midLerp = Mathf.Repeat(startLerp + length * .5f, 1);
			Vector2 midPoint = shape.GetPoint(midLerp);
			Vector2 midNormal = shape.GetNormal(midLerp);
			Vector2 pointDir;
			switch (facings[i].sideID)
			{
				default: pointDir = V2.right; break;
				case 1:  pointDir = V2.up;    break;
				case 2:  pointDir = V2.left;  break;
				case 3:  pointDir = V2.down;  break;
			}

			DRAW.Arrow(midPoint + midNormal * pad, pointDir * .19f, .05f).SetColor(c).Fill(1);
		}

		/*for (int i = 0; i < shape.segmentCount; i++)
			shape.segments[i].DrawFacingChanges(shape.clockwise, .5f, .5f);*/
	}


	public void DrawStartDir(Shape shape)
	{
		Vector2 point  = shape.GetPoint(0);
		Vector2 dir    = shape.GetDir(0);
		Vector2 normal = shape.GetNormal(0);
		
		DRAW.Arrow(point, dir * .25f, .1f).SetColor(Color.white).Fill(1);
		DRAW.Arrow(point, normal * .2f, .1f).SetColor(Color.white).Fill(1);
	}
}

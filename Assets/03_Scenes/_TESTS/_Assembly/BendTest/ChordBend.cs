using UnityEngine;


public class ChordBend : Singleton<ChordBend> 
{
	[Range(0,1)]
	public float squash;
	
	[Space(10)]
	public float lineLength;

	[Space(10)] [Range(0, 1)] public float maxSquash;
	public bool right;
	private Vector3[] segmentPoints = new Vector3[5];
	
	
	public void Awake()
	{
		DRAW.Enabled    = true;
		DRAW.EditorDraw = true;
	}	
	
	
	private void Update()
	{
		squash = (Mathf.Sin(Time.realtimeSinceStartup * 4) * .5f + .5f) * .998f * maxSquash + .001f;

		float angle = Time.realtimeSinceStartup * 10;
		
		float squashedLength = lineLength * (1 - squash);
		Vector3 lineStart = V2.down.Rot(angle, squashedLength * .5f);
		Vector3 lineEnd   = V2.up.Rot(angle,  squashedLength * .5f);
		DRAW.Vector(lineStart, lineEnd - lineStart).SetColor(Color.cyan);
		DRAW.Circle(lineStart, .02f).SetColor(Color.cyan);
		DRAW.Circle(lineEnd, .02f).SetColor(Color.cyan);

		if (f.Same(squash, 0))
			return;
		
		Bend(lineStart, lineEnd, lineLength, right, ref segmentPoints);
		
		
		//	DRAW	//
		for (int i = 0; i < segmentPoints.Length - 1; i++)
			DRAW.Vector(segmentPoints[i], segmentPoints[i+1] - segmentPoints[i]).
				SetColor(Color.Lerp(COLOR.yellow.fresh, COLOR.purple.maroon, (float)i/(segmentPoints.Length - 1)));
	}


	private static void Bend(Vector3 root, Vector3 tip, float lineLength, bool rightSide, ref Vector3[] points)
        {
            float squashedLength = Vector3.Distance(root, tip);
            float sign = rightSide ? 1 : -1;
		
            if (squashedLength >= lineLength)
            {
                Vector3 dir = (tip - root).normalized;
                for (int i = 0; i < points.Length; i++)
                {	
				
                    points[i] = root + dir * lineLength * ((float)i / (points.Length - 1));
                }
                return;
            }
		
		
            float bend = 1 - squashedLength / lineLength;
            float radius = Mth.Chord.GetRadius(squashedLength, lineLength);
		
		
            //	Put On Circle	//
            float segmentAngle = 360f * bend / (points.Length - 1);
            for (int i = 0; i < points.Length; i++)
                points[i] = GetPointOnCircle(segmentAngle * i, sign, radius);
		
            //	Point Up	//
            Vector3 pointDir = (points[points.Length - 1] - points[0]).normalized;
            Quaternion rot = Quaternion.FromToRotation(pointDir, V3.up);
            for (int i = 0; i < points.Length; i++)
                points[i] = rot * points[i];
		
            //	Fit Length	//
            float factor = squashedLength / Vector3.Distance(points[points.Length - 1], points[0]);
            for (int i = 0; i < points.Length; i++)
                points[i] = points[i].Scale(y: factor);
		
            //	Allign	//
            Vector3 lineDir = (tip - root).normalized;
		
            rot = Quaternion.FromToRotation(V3.up, lineDir);
            for (int i = 0; i < points.Length; i++)
                points[i] = rot * points[i];

            Vector3 offset = root - points[0];
            for (int i = 0; i < points.Length; i++)
                points[i] += offset;
        }
        
        
    private static Vector3 GetPointOnCircle(float angle, float sign, float radius)
        {
            return new Vector3(radius * -sign, 0, 0).RotZ(angle * -sign);
        }
}

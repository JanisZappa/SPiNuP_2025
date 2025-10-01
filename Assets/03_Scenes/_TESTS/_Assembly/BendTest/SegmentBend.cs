using UnityEngine;


public class SegmentBend : MonoBehaviour {

	[Range(0,1)]
	public float squash;
	
	[Space(10)]
	public float lineLength;

	public int segmentCount;

	private Vector3[] segmentPoints;
	private int count;
	
	
	public void OnEnable()
	{
		DRAW.Enabled = true;
		DRAW.EditorDraw = true;
	}	
	
	
	private void Update ()
	{
		//	Straight	//
		float   squashedLength = lineLength * (1 - squash);
		Vector3 lineStart      = V2.down * squashedLength * .5f;
		Vector3 lineEnd        = V2.up * squashedLength * .5f;
		
		DRAW.Vector(lineStart, lineEnd - lineStart).SetColor(Color.cyan);
		
		
		// Segments	//
		if (count != segmentCount)
		{
			count = segmentCount;
			segmentPoints = new Vector3[segmentCount + 1];
		}

		if (segmentCount == 0)
			return;
		
		float angle         = Mathf.Lerp(0, 360f, squash);
		float segmentAngle  = angle / segmentCount;
		float segmentLength = lineLength / segmentCount;
		
		Debug.Log(segmentAngle);

		segmentPoints[0] = lineStart;

		for (int i = 1; i < segmentPoints.Length; i++)
			segmentPoints[i] = segmentPoints[i - 1] + V2.down.Rot(i * segmentAngle, segmentLength).V3();
		
		
		//	Adjust	//
		Vector3 pointDir = (segmentPoints[segmentPoints.Length - 1] - segmentPoints[0]).normalized;
		Vector3 lineDir  = (lineEnd - lineStart).normalized;
		
		Quaternion rot = Quaternion.FromToRotation(pointDir, lineDir);
		for (int i = 0; i < segmentPoints.Length; i++)
			segmentPoints[i] = rot * segmentPoints[i];

		Vector3 offset = lineStart - segmentPoints[0];
		for (int i = 0; i < segmentPoints.Length; i++)
			segmentPoints[i] += offset;
		
 
		for (int i = 0; i < segmentCount; i++)
			DRAW.Vector(segmentPoints[i], segmentPoints[i+1] - segmentPoints[i]).SetColor(Color.yellow);
	}
}

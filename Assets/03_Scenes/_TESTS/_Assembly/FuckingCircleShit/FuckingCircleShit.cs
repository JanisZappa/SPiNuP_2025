using UnityEngine;


public class FuckingCircleShit : MonoBehaviour
{
	public float radius;
	public int segments;
	public float limbLength;
	[Range(0,1)]
	public float bendyness;

	[Space(10)] 
	public bool anim;
	

	private readonly Vector3[] points = new Vector3[100];

	
	private void OnEnable()
	{
		DRAW.Enabled = true;
	}


	private void Update ()
	{
		if (anim)
			bendyness = Mth.SmoothPP(0, 1, Time.realtimeSinceStartup * .4f);

		
	//  Get Angle for segment on radius	 //
		float limb      = limbLength / (segments - 1);
		float limbAngle = Mathf.Asin(limb * .5f / radius) * -Mathf.Rad2Deg * 2;

		
	//  Position Points  //
		Vector3 chainPos = V3.zero;
		for (int i = 0; i < segments; i++)
		{
			float      segmentAngle = (limbAngle * i - limbAngle * .5f) * bendyness;
			Quaternion boneRot      = Rot.Z(segmentAngle);
			           chainPos    += boneRot * V3.up.RotZ(segmentAngle, i > 0 ? limb : 0);
			
			points[i] = chainPos;
		}
		
		
	//  Calculate Counter Angle to Get Tip on Circle  //
		Vector3    rootTip   = points[segments - 1] - points[0];
		float      rootAngle = Mathf.Asin(rootTip.magnitude * .5f / radius) * -Mathf.Rad2Deg * 2;
		Vector3    aimPoint  = V3.right * radius + V3.left.RotZ(rootAngle, radius);
		Quaternion partRot   = Quaternion.FromToRotation(rootTip, aimPoint);
		
		
	//  Apply  //
		for (int i = 0; i < segments; i++)
			points[i] = partRot * points[i];
		

	//  Drawing  //
		DRAW.Circle(V3.right * radius, radius, 100).SetColor(Color.yellow);
		
		
		for (int i = 0; i < segments - 1; i++)
		{
			Color c = Color.Lerp(COLOR.blue.cornflower, COLOR.red.tomato, i / (segments - 1f));
			
			if(i < segments - 2)
				DRAW.DotVector(points[i], points[i + 1] - points[i], .05f, .05f).SetColor(c).Fill(1);
			else
				DRAW.Arrow(points[i], points[i + 1] - points[i], .2f).SetColor(c).Fill(1);
		}
	}


	private void OnValidate()
	{
		segments   = Mathf.Max(2, segments);
		limbLength = Mathf.Min(limbLength, radius * 2);
		radius     = Mathf.Max(.1f, radius);
	}
}

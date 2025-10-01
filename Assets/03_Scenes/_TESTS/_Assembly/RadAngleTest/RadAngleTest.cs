using UnityEngine;


public class RadAngleTest : MonoBehaviour
{
	[Range(0, 1)]
	public float forwardLerp, range;

	public float radius;
	

	private void Update ()
	{
		forwardLerp = Mathf.Repeat(forwardLerp + Time.deltaTime * .2f, 1);
		
		
		DRAW.Circle(Vector3.zero, radius, 100).SetColor(Color.red);

		Vector2 forward = Quaternion.AngleAxis(forwardLerp * 360, Vector3.forward) * Vector3.up;
		Vector2 point1  = Quaternion.AngleAxis((forwardLerp - range * .5f) * 360, Vector3.forward) * Vector3.up * radius;
		Vector2 point2  = Quaternion.AngleAxis((forwardLerp + range * .5f) * 360, Vector3.forward) * Vector3.up * radius;
		
		DRAW.Circle(point1, .1f, 30).SetColor(Color.red).Fill(1);
		DRAW.Circle(point2, .1f, 30).SetColor(Color.red).Fill(1);

		
		Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition).SetZ(0);
		DRAW.Circle(point, .1f, 30).SetColor(Color.cyan).Fill(1);


		DRAW.Circle(forward * radius, .2f, 30).SetColor(Color.yellow);
		DRAW.Vector(Vector2.zero, forward * radius).SetColor(Color.yellow);

		Vector2 dir = point.normalized;


		float angle = forward.RadAngle(dir);
		
		DRAW.Text(angle.ToString(), point, Color.cyan, 3, offset: Vector2.up * 4);

		float clampAngle = Mathf.Min(Mathf.Abs(angle), range * Mathf.PI) * Mathf.Sign(angle);
		Vector2 point3  = Quaternion.AngleAxis(forwardLerp * 360 + clampAngle * Mathf.Rad2Deg, Vector3.forward) * Vector3.up * radius;
		DRAW.Circle(point3, .3f, 30).SetColor(Color.cyan);
	}
}

using UnityEngine;


public class LerpTests : MonoBehaviour
{
	public Transform linear, test;
	
	[Range(0, 1)]
	public float accel;

	[Space] [Range(0, 1)] public float lerp;
	
	private void Update ()
	{
		lerp = Mathf.PingPong(Time.realtimeSinceStartup * .5f, 1);
		float clampedLerp = Mathf.Clamp01((lerp - .1f) * (1f / .8f));
		
		float testLerp = GetAccelLerp(clampedLerp, accel);
		
		const float min = -8, max = 8;
		linear.position = new Vector3(Mathf.Lerp(min, max, clampedLerp), 1.5f);
		test.position   = new Vector3(Mathf.Lerp(min, max, testLerp), -1.5f);

		const float size = 8;

		DRAW.Rectangle(Vector3.zero, Vector2.one * size).SetColor(Color.cyan.A(.4f));

		DRAW.Shape shape = DRAW.Shape.Get(100);
		for (int i = 0; i < 100; i++)
		{
			float sampleLerp = i / 99f;

			shape.Set(i, new Vector3(sampleLerp - .5f, GetAccelLerp(sampleLerp, accel) - .5f) * size);
		}

		shape.SetColor(COLOR.red.tomato);

		DRAW.Circle(new Vector3(-.5f + clampedLerp, -.5f + testLerp) * size, .1f, 100).SetColor(COLOR.red.tomato).Fill(1, true);
	}


	private static float GetAccelLerp(float lerp, float accel)
	{
		if (lerp > 0)
		{
			if (lerp < 1)
			{
				Vector2 mid = Vector2.one * .5f;
				float radius = .5f * accel;
				Vector2 c1 = Vector2.up * radius;

				float dx = c1.x - mid.x;
				float dy = c1.y - mid.y;
				float D_squared = dx * dx + dy * dy;
				float L = Mathf.Sqrt(D_squared - radius * radius);

				Vector2 pA = accel < 1? Contact(c1, radius, mid, L) : mid;

				if (lerp <= pA.x)
					return -(Mathf.Sqrt(1 - Mathf.Pow(lerp / radius, 2)) * radius - radius);

				if (lerp <= 1 - pA.x)
					return Mathf.Lerp(pA.y, 1 - pA.y, (lerp - pA.x) * (1f / (1 - pA.x - pA.x)));
				
				return 1 + (Mathf.Sqrt(1 - Mathf.Pow((1 - lerp) / radius, 2)) * radius - radius);
			}
			
			return 1;
		}

		return 0;
	}


	private static Vector2 Contact(Vector2 c1, float r1, Vector2 c2, float r2)
	{
		float dist = Vector2.Distance(c1, c2);
      
	//  Find a and h.  //
		float a = (r1 * r1 - r2 * r2 + dist * dist) / (2 * dist);
		float h = Mathf.Sqrt(r1 * r1 - a * a);

		Vector2 c = c1 + a * (c2 - c1) / dist;

		return new Vector2(c.x + h * (c2.y - c1.y) / dist, c.y - h * (c2.x - c1.x) / dist);
	}
}

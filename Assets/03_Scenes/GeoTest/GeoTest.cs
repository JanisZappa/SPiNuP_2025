using GeoMath;
using UnityEngine;


public class GeoTest : MonoBehaviour
{
	private float r;

	private void Awake()
	{
		r = Random.Range(0, 400f);
	}
	
	
	private void Update ()
	{
		float t = r + Time.realtimeSinceStartup;

		Circle c1 = new Circle(PerlinPos(t * .1f) * 5,       Mathf.Lerp(.1f, 4, Mathf.PerlinNoise(t * .3f + 6883f, .453f)));
		Circle c2 = new Circle(PerlinPos(t * .1f + 463) * 5, Mathf.Lerp(.1f, 4, Mathf.PerlinNoise(t * .3f + 583f, .453f)));
		
		Color c = c1.Contact(c2) ? Color.red : Color.yellow;
		
		c1.Draw(100).SetColor(c);
		c2.Draw(100).SetColor(c);
		
		
		Line line = new Line(PerlinPos(t * .4f + 6612) * 4, PerlinPos(t * .4f + 112.3f) * 4);
		
		Color lineC = c1.Contact(line) || c2.Contact(line) ?  Color.magenta : Color.cyan;
		line.Draw().SetColor(lineC);
		
		
		Arc arc = new Arc(PerlinPos(t * .4f + 754) * 2,
			Mathf.Lerp(.1f, 4, Mathf.PerlinNoise(t * .3f + 583f, .453f)), 
			Mathf.Lerp(0, 360, Mathf.PerlinNoise(t * .2f + 23f, .453f)),
			Mathf.Lerp(-1, 1, Mathf.PerlinNoise(t * .43f + 883f, .453f)));

		arc.Draw(100).SetColor(Color.white);
	}


	private static Vector2 PerlinPos(float t)
	{
		return new Vector2(Mathf.Lerp(-1, 1, Mathf.PerlinNoise(t, .453f)), Mathf.Lerp(-1, 1, Mathf.PerlinNoise(t + 864, .453f)));
	}
}

using GeoMath;
using UnityEngine;

namespace Test
{
	public class LineCircleAllign : MonoBehaviour
	{
		public float radius, lineLength;
		
		
		private void Update ()
		{
			Circle circle = new Circle(Vector2.zero, radius);
			       circle.Draw(100).SetColor(Color.white);

			float angle = circle.GetArcAngle(lineLength);
			if (!Mathf.Approximately(angle, 0))
			{
				Vector2 p1 = Vector2.up.RotRad(-angle * .5f).SetLength(radius);
				Vector2 p2 = Vector2.up.RotRad( angle * .5f).SetLength(radius);

				DRAW.Vector(p1, p2 - p1).SetColor(Color.yellow);
				
				DRAW.Vector(Vector3.zero, p1).SetColor(Color.yellow);
				DRAW.Vector(Vector3.zero, p2).SetColor(Color.yellow);
			}
		}
	}
}

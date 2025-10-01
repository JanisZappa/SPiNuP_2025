using GeoMath;
using UnityEngine;

namespace Test
{
	public class ArcDebug : MonoBehaviour
	{
		public Line line;
		[Range(-1, 1)] public float bend;


		private void Update()
		{
			Arc arc = new Arc(line, bend, 0);
			arc.Draw(100).SetColor(Color.white * .5f);

			Vector2 a = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			DRAW.Circle(a, .04f, 20).SetColor(Color.red).Fill(1, true);

			Vector2 b = arc.GetClosestPoint(a);
			DRAW.Circle(b, .04f, 20).SetColor(Color.red).Fill(1, true);
		}
	}
}

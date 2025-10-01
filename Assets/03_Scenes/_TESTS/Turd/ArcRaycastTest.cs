using GeoMath;
using UnityEngine;

namespace Test
{
	public class ArcRaycastTest : MonoBehaviour
	{
		public Line line;
		[Range(-1, 1)]
		public float bend;

		[Space(10)] 
		public Line ray;
		
		
		private void Update ()
		{
			Arc arc = new Arc(line, bend, 0);
			arc.Draw(100).SetColor(Color.red);

			Vector2 hit;
			if (arc.RayCast(ray.l1, ray.dir, out hit))
			{
				DRAW.Vector(ray.l1, hit - ray.l1).SetColor(Color.yellow);

				Vector2 reflect = Vector2.Reflect(ray.dir, arc.LerpDir(arc.GetClosestLerp(hit)).Rot90());
				DRAW.Vector(hit, reflect * 100).SetColor(Color.yellow.A(.25f));
			}
			else
				DRAW.Vector(ray.l1, ray.dir * 10000).SetColor(Color.yellow);
		}
	}
}
using GeoMath;
using UnityEngine;


public class IntersectionTest : MonoBehaviour
{
	public Transform center;
	public float radius;
	
	[Space(10)]
	
	public Transform root;
	public Transform aim;

	[Space(10)] public ShapeTest shapeTest;
	[Space(10)] public bool useShape;


	private void Update ()
	{
		if (!useShape)
		{
			Vector2 p1, p2;
			int intersection = new Circle(center.position, radius).Contact(new Line(root.position, aim.position), out p1, out p2);
			bool intersect = intersection > 0;

			if (intersection > 0)
				DRAW.Circle(p1, .1f, 16).SetColor(Color.white).Fill(1);
			if (intersection == 2)
				DRAW.Circle(p2, .1f, 16).SetColor(Color.white).Fill(1);

			DRAW.Circle(center.position, radius, 50).SetColor(COLOR.blue.cornflower);
			DRAW.Ray(root.position, aim.position).SetColor(intersect ? Color.white : COLOR.red.tomato);
		}
		else
		{
			Vector2 p1;
			bool hit = shapeTest.shape.Raycast(root.position, aim.position - root.position, out p1);
			
			DRAW.Ray(root.position, aim.position).SetColor(hit ? Color.white : COLOR.red.tomato);
			
			if(hit)
				DRAW.Circle(p1, .1f, 16).SetColor(Color.white).Fill(1);
		}
	}
}

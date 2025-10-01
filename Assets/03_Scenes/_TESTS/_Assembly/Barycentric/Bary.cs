using GeoMath;
using UnityEngine;


public class Bary : MonoBehaviour
{
	public Vector2 v1, v2, v3;


	private void Awake()
	{
		Cursor.visible = false;
	}


	private void Update ()
	{
		DRAW.Circle(v1, .1f, 20).SetColor(Color.red).Fill(1, true);
		DRAW.Circle(v2, .1f, 20).SetColor(Color.green).Fill(1, true);
		DRAW.Circle(v3, .1f, 20).SetColor(Color.blue).Fill(1, true);
		
		

		Vector2 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 w = Tri.BaryWeight(v1, v2, v3, p);

		if (w.x >= 0 && w.y >= 0 && w.z >= 0)
		{
			Color mix = Color.red * w.x + Color.green * w.y + Color.blue * w.z;
			DRAW.Circle(p, .1f, 20).SetColor(mix).Fill(1, true);
		}
		else
			DRAW.Circle(p, .1f, 20).SetColor(Color.white).Fill(1, true);
	}
}

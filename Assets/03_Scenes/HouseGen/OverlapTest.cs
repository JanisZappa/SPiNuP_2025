using GeoMath;
using UnityEngine;


public class OverlapTest : MonoBehaviour
{
	private Vector2 s1, s2;
	private Vector2 pos;

	private Bounds2D b1, b2;

	
	private void Start ()
	{
		SizeUpdate();
	}

	
	private void SizeUpdate()
	{
		s1 = new Vector2(Random.Range(2.0f, 15.0f), Random.Range(2.0f, 15.0f)) * 10;
		s2 = new Vector2(Random.Range(2.0f, 15.0f), Random.Range(2.0f, 15.0f)) * 10;

		Vector2 p = Vector3.up * 200;
		b2 = new Bounds2D(p + s2 * -.5f).Add(p + s2 * .5f);
	}
	
	
	private void Update () 
	{
		pos += new Vector2(Time.deltaTime * Input.GetAxis("Horizontal"), Time.deltaTime * Input.GetAxis("Vertical")) * 100;
		
		if(Input.GetKeyDown(KeyCode.Space))
			SizeUpdate();

		b1 = new Bounds2D(pos + s1 * -.5f).Add(pos + s1 * .5f);
		b1.Draw().SetColor(Color.red);
		b2.Draw().SetColor(Color.red);

		DRAW.GUI_Text(b1.IntersectLerp(b2).ToString("F2"), pos, Color.red, 3);
	}
}

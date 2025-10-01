using UnityEngine;


public class StaticBounceLight : MonoBehaviour 
{
	public Color color;
	public float multi;

	private SpriteRenderer sR;


	private void Awake()
	{
		sR = GetComponent<SpriteRenderer>();
	}
	
	
	private void LateUpdate () 
	{
		if(GameManager.Running)
			sR.color = (color * LightingSet.SunColor * multi * (GameCam.CurrentSide.front? 1 : .5f)).A(0);
	}
}

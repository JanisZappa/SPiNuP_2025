using UnityEngine;


public class FloorColor : MonoBehaviour
{
	public Color floorColor;
	public float sunColorLerp;
	
	private static readonly int FloorC = Shader.PropertyToID("FloorColor");
	
	private void LateUpdate () 
	{
		Shader.SetGlobalColor(FloorC, Color.Lerp(floorColor, floorColor * LightingSet.SunColor, sunColorLerp));
	}
}

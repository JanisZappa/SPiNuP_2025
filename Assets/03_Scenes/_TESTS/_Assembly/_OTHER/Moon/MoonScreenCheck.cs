using UnityEngine;


public class MoonScreenCheck : MonoBehaviour
{
	public RectTransform textRect;
	public Camera cam;

	[Space] public Vector2 textScales, camScales;

	private void Update ()
	{
		float lerp = Screen.width < Screen.height? 1 : 0;
		textRect.localScale = Vector3.one * Mathf.Lerp(textScales.x, textScales.y, lerp);
		cam.fieldOfView     = Mathf.Lerp(camScales.x, camScales.y, lerp);
	}
}

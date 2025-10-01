using UnityEngine;
using UnityEngine.UI;


public class PaletteDebug : ActiveUI
{
	private Image image;
	private Color color;
	
	private GameObject child;

	public bool fullTex;

	
	private void Awake()
	{
		image = GetComponent<Image>();
		color = image.color;
		image.color = Color.Lerp(Color.black, color, fullTex ? 1 : .5f);
		child = transform.GetChild(0).gameObject;
	}
	
	
	public override bool HitUI(int click)
	{
		if (UI_Manager.ImPointedAt(child))
		{
			if (click == 1)
			{
				fullTex = !fullTex;
				image.color = Color.Lerp(Color.black, color, fullTex ? 1 : .5f);
				Palette.Load(fullTex);
			}

			return true;
		}

		return false;
	}
}

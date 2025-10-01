using UnityEngine;


public class BounceLight : Shadow
{
	public Color color;
	public float multi;
	
	
	protected override void UpdateTransform()
	{
		UpdateBounceColor();
		
		_transform.rotation = LightingSet.BounceRot;
	}


	public void SetColor(Color color)
	{
		this.color = color;

		UpdateBounceColor();
	}


	private void UpdateBounceColor()
	{
		float factor = multi * (front ? 1 : .5f);
		spriteRenderer.color = new Color(color.r * LightingSet.SunColor.r * factor,
										 color.g * LightingSet.SunColor.g * factor,
			                             color.b * LightingSet.SunColor.b * factor, 0);
	}
}

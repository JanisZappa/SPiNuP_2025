using System.Collections.Generic;
using UnityEngine;


public class MeshPaletteColor : MonoBehaviour
{
	public int pick;
	
	public  MeshFilter mF;

	[Space(10)]
	public BounceLight bounceLight;
	
	private Mesh mesh;
	private List<Color32> colors;

	[Space(10)]
	public bool setOnEnable = true;
	
	
	private void Awake()
	{
		mesh = mF.mesh;
		
		colors = new List<Color32>();
		Color32[] c = mesh.colors32;
		for (int i = 0; i < c.Length; i++)
			colors.Add(c[i]);

		pick = colors[0].r;
		
		UpdateBounceLightColor();
		
		ShaderPaletteInit.onSwitchActorColor += onSwitchActorColor;
	}		

	
	private void OnEnable()
	{
		if(setOnEnable)
			SetColor();
	}


	private void onSwitchActorColor()
	{
		pick = (pick + 1) % Palette.Colors.Length;
		
		SetColor();
	}

	
	private void SetColor()
	{
		if (!gameObject.activeInHierarchy)
			return;
		
		UpdateBounceLightColor();
		
		if (mesh == null)
			return;

		int  count = colors.Count;
		byte p     = (byte)pick;
		
		for (int i = 0; i < count; i++)
			colors[i] = new Color32(p, colors[i].g, colors[i].b, colors[i].a);
		
		mesh.SetColors(colors);
	}


	private void UpdateBounceLightColor()
	{
		bounceLight.SetColor(Palette.Colors[pick]);
	}
}

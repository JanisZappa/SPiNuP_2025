using System.Collections.Generic;
using UnityEngine;


public class NewColorGen : MonoBehaviour
{
	public int hueSteps, satSteps; 
	private int tintSteps;
	public float hueOffset;

	[Space] 
	public float tintMin;
    public float tintMax, satMin;
    [Space]
    public int[] shiftMap;
	[Space] 
	public Color checkColor;

	[Space] public float[] hueOffsets;
	public bool autoStart;
	

	public bool showUsedColors;
	
	private const int count = 1024;

	private MeshRenderer mR, hueMR, a, b;
	private Texture2D tex, hueTex;
	public static readonly Color[] colors = new Color[count];
	private Color[] hueColors;

	private int huePick;
	private static readonly int Color = Shader.PropertyToID("_Color");

	private Color[] palette;
	private int[] paletteIDs;
	private bool[] used;


	private void Awake()
	{
		Transform t = transform;
		hueMR = t.GetChild(0).GetComponent<MeshRenderer>();
		a = t.GetChild(1).GetComponent<MeshRenderer>();
		b = t.GetChild(2).GetComponent<MeshRenderer>();
		
		hueMR.transform.SetParent(null);
		a.transform.SetParent(null);
		b.transform.SetParent(null);
		
		palette = HoudiniUsed(); //Resources.Load<Palette>("Palette").colors;
		paletteIDs = new int[palette.Length];
		
		mR = GetComponent<MeshRenderer>();
		
		if(!autoStart)
			return;
		
		Init();
	}


	public void Init()
	{
		tintSteps = Mathf.FloorToInt(Mathf.Floor((float) count / hueSteps) / satSteps);
            
		float scale = hueMR.transform.localScale.x;
		hueMR.transform.localScale = new Vector3(scale, scale * ((float)satSteps / tintSteps), scale);
		              
		hueColors = new Color[satSteps * tintSteps];

		if(count == 1024)
			tex = new Texture2D(32, 32) {filterMode = FilterMode.Point};
		else
		if(count == 2048)
		{
			tex = new Texture2D(32, 64) {filterMode = FilterMode.Point};
			scale = transform.localScale.x;
			transform.localScale = new Vector3(scale * .5f, scale, scale); 
		}
		else
			tex = new Texture2D(64, 64) {filterMode = FilterMode.Point};
			
			
		mR.material.mainTexture = tex;
		
		hueTex = new Texture2D(tintSteps, satSteps) {filterMode = FilterMode.Point};
		hueMR.material.mainTexture = hueTex;
		
		used = new bool[tintSteps * satSteps];

		TexUpdate();
	}

	
	private void TexUpdate()
	{
		tex.SetPixels(GeneratCrazy(hueSteps, satSteps, hueOffsets));
		//tex.SetPixels(GenerateClamped(satMin, 1, tintMin, tintMax, hSteps, sSteps, true, shiftMap));
		tex.Apply();

		UpdateHueTex();
	}


	private void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			TexUpdate();
		}

		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			huePick = (int)Mathf.Repeat(huePick + 1, hueSteps);
			UpdateHueTex();
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			huePick = (int)Mathf.Repeat(huePick - 1, hueSteps);
			UpdateHueTex();
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			showUsedColors = !showUsedColors;
			UpdateHueTex();
		}
		
		a.material.SetColor(Color, checkColor);
		
		b.material.SetColor(Color, colors[checkColor.ClosestColorIndex(colors)]);
	}


	private void UpdateHueTex()
	{
		if (!hueMR.gameObject.activeInHierarchy)
			return;

		for (int i = 0; i < palette.Length; i++)
			paletteIDs[i] = palette[i].ClosestColorIndex(colors);

		for (int i = 0; i < used.Length; i++)
			used[i] = false;

		for (int hue = 0; hue < hueSteps; hue++)
		{
			int colorIndex = 0;
			for (int saturation = 0; saturation < satSteps; saturation++)
			for (int tint = 0; tint < tintSteps; tint++)
			{
				int colorID = hue * satSteps * tintSteps + saturation * tintSteps + tint;

				for (int i = 0; i < palette.Length; i++)
					if (paletteIDs[i] == colorID)
					{
						used[colorIndex] = true;
						break;
					}

				colorIndex++;
			}
		}
		
		
		int index = 0;
		for (int saturation = 0; saturation < satSteps; saturation++)
		for (int tint = 0; tint < tintSteps; tint++)
		{
			int colorID = huePick * satSteps * tintSteps + saturation * tintSteps + tint;

			/*bool usedColor = false;
			for (int i = 0; i < palette.Length; i++)
				if (paletteIDs[i] == colorID)
				{
					hueColors[index++] = colors[colorID];
					usedColor = true;
					break;
				}

			if (!usedColor)
				hueColors[index++] = colors[colorID].Multi(.25f);*/
			//hueColors[index++] = colors[colorID];

			if (showUsedColors)
			{
				hueColors[index] = colors[colorID].Multi(used[index]? 1 : .25f);
				index++;
			}
			else
				hueColors[index++] = colors[colorID];
		}
		
		hueTex.SetPixels(hueColors);
		hueTex.Apply();
	}
	
	
	public static Color[] GeneratCrazy(int hSteps , int sSteps, float[] hueOffsets, bool addOldColors = false)
	{
		int hueSteps        = hSteps;
		int saturationSteps = sSteps;
		
		float hueShift = 1f / hueSteps;
		float saturationShift = 1f / saturationSteps;

		int hueColors  = Mathf.FloorToInt((float) count / hueSteps);
		int tintColors = Mathf.FloorToInt((float) hueColors / (saturationSteps));

		float tintShift = 1f / (tintColors + 1);
		float tintOffset = tintShift * 2f;
		tintShift = 1f / (tintColors + 3.5f);

		int totalHueColors = tintColors  * saturationSteps * hueSteps;
		int greyColors = count - totalHueColors;

		#if UNITY_EDITOR
		if (addOldColors)
			greyColors -= PaletteSource.Get.oldColors.Length;
		#endif
			
		int index = 0;
		for (int hue = 0; hue < hueSteps; hue++)
		{
			float hueValue = hue * hueShift + hueOffsets[hue] * hueShift;

			for (int sat = 0; sat < saturationSteps ; sat++)
			{
				float saturationValue = saturationShift + sat * saturationShift;

				for (int tint = 0; tint < tintColors; tint++)
				{
					float tintValue = tintOffset + tint * tintShift;
						
					colors[index++] = new HLS(hueValue, tintValue, saturationValue);
				}
			}
		}
			
		//  GreyScale  //
		float greyShift = 1f /(greyColors - 1);
		for (int grey = 0; grey < greyColors; grey++)
		{
			float greyValue = grey * greyShift;
			colors[index++] = new HLS(1, greyValue, 0);
		}
		
	#if UNITY_EDITOR
		if (addOldColors)
			for (int i = 0; i < PaletteSource.Get.oldColors.Length; i++)
				colors[index++] = PaletteSource.Get.oldColors[i];
	#endif

		return colors;
	}

	
	public static Color[] HoudiniUsed(bool getAll = false)
	{
		string[] colorLines = DesktopTxt.Read("oldhoudinicolors", ".spn");
		List<Color> oldColors = new List<Color>();
		for (int i = 0; i < colorLines.Length; i++)
		{
			string line = colorLines[i].Replace(" ","").Replace("{","").Replace("}","");
			string[] parts = line.Split(',');
            
			oldColors.Add(new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), 1));
		}

		if (getAll)
			return oldColors.ToArray();
    
		string[] idLines = DesktopTxt.Read("houdiniused", ".spn");
		List<int> colorIDs = new List<int>();

		for (int i = 0; i < idLines.Length; i++)
			if (!idLines[i].Contains("/"))
				colorIDs.AddUnique(int.Parse(idLines[i]));
			

		List<Color> usedColors = new List<Color>();
		for (int i = 0; i < colorIDs.Count; i++)
			usedColors.Add(oldColors[colorIDs[i]]);
        
		Debug.Log(usedColors.Log());

		return usedColors.ToArray();
	}
	
	
	#region Other
	
	public static Color[] Generate(int hSteps, int sSteps)
	{
		int hueSteps        = hSteps;
		int saturationSteps = sSteps;
		
		float hueShift = 1f / hueSteps;
		float saturationShift = 1f / (saturationSteps);

		int hueColors  = Mathf.FloorToInt((float) count / hueSteps);
		int tintColors = Mathf.FloorToInt((float) hueColors / (saturationSteps));

		float tintShift = 1f / (tintColors + 1);
		float tintOffset = tintShift * 2f;
		tintShift = 1f / (tintColors + 3.5f);

		int totalHueColors = tintColors  * saturationSteps * hueSteps;
		int greyColors = count - totalHueColors;
		Debug.Log("Tint: " + tintColors + " - Hue: " + totalHueColors + " - Grey: " + greyColors);
			
		int index = 0;
		for (int hue = 0; hue < hueSteps; hue++)
		{
			float hueValue = hue * hueShift;

			for (int sat = 0; sat < saturationSteps ; sat++)
			{
				float saturationValue = saturationShift + sat * saturationShift;

				for (int tint = 0; tint < tintColors; tint++)
				{
					float tintValue = tintOffset + tint * tintShift;
						
					colors[index++] = new HLS(hueValue, tintValue, saturationValue);
				}
			}
		}
			
		//  GreyScale  //
		float greyShift = 1f /(greyColors - 1);
		for (int grey = 0; grey < greyColors; grey++)
		{
			float greyValue = grey * greyShift;
			colors[index++] = new HLS(1, greyValue, 0);
		}

		return colors;
	}
	
	
	public static Color[] GenerateNew(int hSteps , int sSteps, float hueOffset)
	{
		int hueSteps        = hSteps;
		int saturationSteps = sSteps;
		
		float hueShift = 1f / hueSteps;
		float saturationShift = 1f / (saturationSteps);

		int hueColors  = Mathf.FloorToInt((float) count / hueSteps);
		int tintColors = Mathf.FloorToInt((float) hueColors / (saturationSteps));

		float tintShift = 1f / (tintColors + 1);
		float tintOffset = tintShift * 2f;
		tintShift = 1f / (tintColors + 3.5f);

		int totalHueColors = tintColors  * saturationSteps * hueSteps;
		int greyColors = count - totalHueColors;
			
		int index = 0;
		for (int hue = 0; hue < hueSteps; hue++)
		{
			float hueValue = hue * hueShift + hueOffset * hueOffset;

			for (int sat = 0; sat < saturationSteps ; sat++)
			{
				float saturationValue = saturationShift + sat * saturationShift;

				for (int tint = 0; tint < tintColors; tint++)
				{
					float tintValue = tintOffset + tint * tintShift;
						
					colors[index++] = new HLS(hueValue, tintValue, saturationValue);
				}
			}
		}
			
		//  GreyScale  //
		float greyShift = 1f /(greyColors - 1);
		for (int grey = 0; grey < greyColors; grey++)
		{
			float greyValue = grey * greyShift;
			colors[index++] = new HLS(1, greyValue, 0);
		}

		return colors;
	}
	
	
	public static Color[] GeneratCrazy2(int hSteps , int sSteps, float[] hueOffsets, float tintMin, float tintMax)
	{
		int hueSteps        = hSteps;
		int saturationSteps = sSteps;
		
		float hueShift = 1f / hueSteps;
		float saturationShift = 1f / (saturationSteps);

		int hueColors  = Mathf.FloorToInt((float) count / hueSteps);
		int tintColors = Mathf.FloorToInt((float) hueColors / (saturationSteps));

		float tintShift = (tintMax - tintMin) / (tintColors + 1);

		int totalHueColors = tintColors  * saturationSteps * hueSteps;
		int greyColors = count - totalHueColors;
			
		int index = 0;
		for (int hue = 0; hue < hueSteps; hue++)
		{
			float hueValue = hue * hueShift + hueOffsets[hue] * hueShift;

			for (int sat = 0; sat < saturationSteps ; sat++)
			{
				float saturationValue = saturationShift + sat * saturationShift;

				for (int tint = 0; tint < tintColors; tint++)
				{
					float tintValue = tintMin + tint * tintShift;
						
					colors[index++] = new HLS(hueValue, tintValue, saturationValue);
				}
			}
		}
			
		//  GreyScale  //
		float greyShift = 1f /(greyColors - 1);
		for (int grey = 0; grey < greyColors; grey++)
		{
			float greyValue = grey * greyShift;
			colors[index++] = new HLS(1, greyValue, 0);
		}

		return colors;
	}
	
	
	public static Color[] GenerateClamped(float satMin, float satMax, float tintMin, float tintMax, int hSteps, int sSteps, bool debug = false, int[] shiftMap = null)
	{
		int hueSteps        = hSteps;
		int saturationSteps = sSteps;
		
		float hueShift = 1f / hueSteps;
		float saturationShift = (satMax - satMin) / (saturationSteps - 1);

		int hueColors  = Mathf.FloorToInt((float) count / hueSteps);
		int tintColors = Mathf.FloorToInt((float) hueColors / (saturationSteps));

		float tintShift = (tintMax - tintMin) / (tintColors - 1);

		int totalHueColors = tintColors  * saturationSteps * hueSteps;
		int greyColors = Mathf.Min(32, count - totalHueColors);
		
		bool usesShiftMap = shiftMap != null && tintColors == shiftMap.Length;
			
		if(debug)
			Debug.Log("Tint: " + tintColors + " - Hue: " + totalHueColors + " - Grey: " + greyColors);
		
		int index = 0;
		for (int hue = 0; hue < hueSteps; hue++)
		{
			float hueValue = hue * hueShift;

			for (int sat = 0; sat < saturationSteps ; sat++)
			{
				float saturationValue = satMin + sat * saturationShift;

				for (int tint = 0; tint < tintColors; tint++)
				{
					float tintValue;
					if(usesShiftMap)
						tintValue = tintMin + tint * tintShift + tintShift * shiftMap[tint];
					else
						tintValue = tintMin + tint * tintShift;
						
					colors[index++] = new HLS(hueValue, tintValue, saturationValue);
				}
			}
		}
			
		//  GreyScale  //
		float greyShift = 1f /(greyColors - 1);
		for (int grey = 0; grey < greyColors; grey++)
		{
			float greyValue = grey * greyShift;
			colors[index++] = new HLS(1, greyValue, 0);
		}

		while (index < count)
			colors[index++] = new Color(0, 0, 0, 1);

		return colors;
	}
	
	
	public static Color[] GenerateClampedShift(float satMin, float satMax, float tintMin, float tintMax, int hSteps, int sSteps, float[] hueShifts)
	{
		int hueSteps        = hSteps;
		int saturationSteps = sSteps;
		
		float hueShift = 1f / hueSteps;
		float saturationShift = (satMax - satMin) / (saturationSteps - 1);

		int hueColors  = Mathf.FloorToInt((float) count / hueSteps);
		int tintColors = Mathf.FloorToInt((float) hueColors / (saturationSteps));

		float tintShift = (tintMax - tintMin) / (tintColors - 1);

		int totalHueColors = tintColors  * saturationSteps * hueSteps;
		int greyColors = Mathf.Min(32, count - totalHueColors);
		
		int index = 0;
		for (int hue = 0; hue < hueSteps; hue++)
		{
			float hueValue = hue * hueShift;

			for (int sat = 0; sat < saturationSteps ; sat++)
			{
				float saturationValue = satMin + sat * saturationShift;

				for (int tint = 0; tint < tintColors; tint++)
				{
					float tintValue = tintMin + tint * tintShift;
						
					colors[index++] = new HLS(hueValue, tintValue, saturationValue);
				}
			}
		}
			
		//  GreyScale  //
		float greyShift = 1f /(greyColors - 1);
		for (int grey = 0; grey < greyColors; grey++)
		{
			float greyValue = grey * greyShift;
			colors[index++] = new HLS(1, greyValue, 0);
		}

		while (index < count)
			colors[index++] = new Color(0, 0, 0, 1);

		return colors;
	}
	
	#endregion
}


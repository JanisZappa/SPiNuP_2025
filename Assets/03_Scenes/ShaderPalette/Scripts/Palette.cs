using UnityEngine;


[CreateAssetMenu]
public class Palette : ScriptableObject  
{
	public Color[] colors;
	public Texture2D textures, matCaps;

	[Range(0, 2)]
	public float reflectionMulti;
	
	public static bool Initialized;
	public static Color[] Colors;
	public static Palette Inst;
	
	public const int texSize = 128;
	
	public const float texMargin = 2 / 128f;
	
	private static Texture2D gamutText, genTex;
	

	private static readonly int MainTex    = Shader.PropertyToID("MainTex");
	private static readonly int PaletteTex = Shader.PropertyToID("Palette");
	private static readonly int MatCapTex  = Shader.PropertyToID("MatCap");
	
	private static readonly int PaletteFactor   = Shader.PropertyToID("PaletteFactor");
	private static readonly int PaletteOffset   = Shader.PropertyToID("PaletteOffset");
	private static readonly int MatCapXMulti    = Shader.PropertyToID("MatCapXMulti");
	private static readonly int TexMargin       = Shader.PropertyToID("TexMargin");
	private static readonly int TexMarginMulti  = Shader.PropertyToID("TexMarginMulti");
	private static readonly int ReflectionMulti = Shader.PropertyToID("ReflectionMulti");

	
	public static void Load(bool loadNormal = true)
	{
		Inst = Resources.Load<Palette>("Palette");
		
		Colors = Inst.colors;
		for (int i = 0; i < Colors.Length; i++)
			Colors[i] = Colors[i].A(1);

		if (loadNormal)
		{
			genTex = new Texture2D(Inst.colors.Length, 1, TextureFormat.ARGB32, false)
			{
				filterMode = FilterMode.Point
			};
            
			genTex.SetPixels(Colors);
		}
		else
		{
			genTex = new Texture2D(Inst.colors.Length, 1, TextureFormat.ARGB32, false)
			{
				filterMode = FilterMode.Point
			};
            
			genTex.SetPixels(Colors.Copy().Randomize());
		}
		
		genTex.Apply();

		Shader.SetGlobalTexture(PaletteTex,       genTex);
		Shader.SetGlobalFloat(PaletteFactor, 1f / genTex.width);
		Shader.SetGlobalFloat(PaletteOffset, 1f / genTex.width * .5f);
		
		Shader.SetGlobalTexture(MatCapTex, Inst.matCaps);
		Shader.SetGlobalFloat(MatCapXMulti, 1f / ((float)Inst.matCaps.width / Inst.matCaps.height));
		
		Shader.SetGlobalFloat(TexMargin, texMargin);
		Shader.SetGlobalFloat(TexMarginMulti, 1f / (1f + texMargin * 2));
		
		Shader.SetGlobalTexture(MainTex, Inst.textures);
		
		Shader.SetGlobalFloat(ReflectionMulti, Inst.reflectionMulti);

		Initialized = true;
	}


	public static Color GetColor(Color color)
	{
		return genTex.GetPixel(Mathf.RoundToInt(CM_ID(color).x), 0);
	}
	
	
	private static Vector2 CM_ID(Color color)
	{
		float matcapValue  = Mathf.Round(color.g * 256);
		float matCapOffset = Mathf.Floor(matcapValue / 16);
		float matCap       = matcapValue - matCapOffset * 16;

		float texID = Mathf.Round(color.r * 256) + matCapOffset * 256;
    
		return new Vector2(texID, matCap);
	}
	
	
	public static void EditorLoad(){ Load();}
}

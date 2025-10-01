using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PaletteSource : ScriptableObject 
{
	public Color[] oldColors, newColors, hues, houdini;
	
	[Space(10)] 
	public Texture2D[] textures, matCaps;

	[Space(10)] 
	public Texture2D testMatCap;

	[HideInInspector]
	public int[] mapping;
	
	
#if UNITY_EDITOR
	private static PaletteSource _source;

	public static PaletteSource Get
	{
		get
		{
			if(_source == null)
				_source = AssetDatabase.LoadAssetAtPath<PaletteSource>("Assets/03_Scenes/ShaderPalette/SourceFiles/PaletteSource.asset");
			return _source;
		}
	}
#endif
}

using System.IO;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PaletteSource))]
public class PaletteSourceEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);
        
        GUILayout.BeginHorizontal();
        
        GUI.color = COLOR.green.lime;
        if (GUILayout.Button("Update Textures"))
            UpdateTextures(target as PaletteSource, false);
        
        GUI.color = COLOR.orange.coral;
        if (GUILayout.Button("Update Textures with Test MatCap"))
            UpdateTextures(target as PaletteSource, true);
        
        GUI.color = Color.white;
        GUILayout.EndHorizontal();
    }


    private static void UpdateTextures(PaletteSource source, bool useTestMaptCap)
    {
        if(!Palette.Initialized)
            Palette.Load();

        Palette palette = Palette.Inst;
        
    //  Creating Wall Textures  //
        {
            const int texRes = Palette.texSize;
            Texture2D wallTexture = new Texture2D((source.textures.Length * texRes).NextPowerOfTwo(), texRes, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp, requestedMipmapLevel = 0
            };

            const float stepMulti = (1 + Palette.texMargin * 2) / 1f;
    
            for (int i = 0; i < source.textures.Length; i++)
            {
                Texture2D readTex = source.textures[i];

                float size     = readTex.width;
                float readStep = size / texRes ;
                float offset   = size * -Palette.texMargin;

                int xOffset = i * texRes;

                for (int x = 0; x < texRes; x++)
                for (int y = 0; y < texRes; y++)
                {
                    int readX = Mathf.FloorToInt(Mathf.Repeat(offset + x * readStep * stepMulti, size));
                    int readY = Mathf.FloorToInt(Mathf.Repeat(offset + y * readStep * stepMulti, size));
                    
                    wallTexture.SetPixel(x + xOffset, y, readTex.GetPixel(readX, readY));
                }  
            }
        
            wallTexture.Apply();
    
            wallTexture = SaveTexture(wallTexture, "WallTextures");
            palette.textures = wallTexture;
        }
        
    //  Creating MatCapTexture  //
        {
            int matCapRes = source.matCaps[0].height;
            Texture2D matCapTex = new Texture2D((source.matCaps.Length * matCapRes).NextPowerOfTwo(), matCapRes, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Trilinear, 
                wrapMode = TextureWrapMode.Clamp,
            };
        
            for (int i = 0; i < source.matCaps.Length; i++)
            {
                Texture2D readTex = useTestMaptCap? source.testMatCap : source.matCaps[i];
    
                for (int x = 0; x < matCapRes; x++)
                for (int y = 0; y < matCapRes; y++)
                    matCapTex.SetPixel(x + i * matCapRes, y, readTex.GetPixel(x, y));
            }
            
            matCapTex.Apply();
        
            matCapTex = SaveTexture(matCapTex, "MatCaps");
            palette.matCaps = matCapTex;
        }
        
        EditorUtility.SetDirty(palette);
        
        Palette.Load();
    }


    private static Texture2D SaveTexture(Texture2D texture, string name)
    {
        string fullPath = Application.dataPath + "/03_Scenes/ShaderPalette/SourceFiles/" + name + ".png";
        
        byte[] bytes = texture.EncodeToPNG();
        FileStream   stream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
        BinaryWriter writer = new BinaryWriter(stream);
        for (int i = 0; i < bytes.Length; i++)
            writer.Write(bytes[i]);
        
        writer.Close();
        stream.Close();
        
        AssetDatabase.Refresh();
        
        return AssetDatabase.LoadAssetAtPath<Texture2D>(folder + name + ".png");
    }
    
    
    private const string folder = "Assets/03_Scenes/ShaderPalette/SourceFiles/";
}

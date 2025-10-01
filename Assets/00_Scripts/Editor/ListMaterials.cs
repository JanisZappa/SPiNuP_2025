using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public static class ListMaterials 
{
    [MenuItem("Tools/Project Info/List Mats")]
    private static void List()
    {
        Material[] mats = GetAllMats();
        List<string> matShaders = new List<string>();

        List<string> lines = new List<string>();
        for (int i = 0; i < mats.Length; i++)
        {
            Material mat = mats[i];
            string assetPath = AssetDatabase.GetAssetPath(mat);
            matShaders.Add(assetPath);
            lines.Add(mat.name + " <- " + mat.shader.name + " | " + assetPath);
        }

        {
            int longest = 0;
            for (int i = 0; i < lines.Count; i++)
                longest = Mathf.Max(longest, lines[i].Split('<')[0].Length);

            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split('<');
                lines[i] = parts[0].PadRight(longest) + "<" + parts[1];
            }   
        }
       
        {
            int longest = 0;
            for (int i = 0; i < lines.Count; i++)
                longest = Mathf.Max(longest, lines[i].Split('|')[0].Length);

            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split('|');
                lines[i] = parts[0].PadRight(longest) + "|" + parts[1];
            }   
        }
        
        ProjectTxt.Write("04_TextFiles/Materials", lines.ToArray());


        List<string> shaderPaths = GetAllShadersPaths();
        for (int i = 0; i < shaderPaths.Count; i++)
        {
            string path = shaderPaths[i];
            for (int e = 0; e < matShaders.Count; e++)
                if (path == matShaders[e])
                {
                    shaderPaths.RemoveAt(i);
                    i--;
                    break;
                }
        }

        for (int i = 0; i < shaderPaths.Count; i++)
            Debug.Log(shaderPaths[i]);
    }
    
    
    private static Material[] GetAllMats()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.mat*", SearchOption.AllDirectories);
        
        List<Material> mats = new List<Material>();
        foreach (string file in files)
        {
            string fullPath = file.Replace(@"\","/");
            string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
            Material material = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material)) as Material;

            if(material!= null)
                mats.Add(material);
        }

        return mats.ToArray();
    }
    
    private static List<string> GetAllShadersPaths()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.shader*", SearchOption.AllDirectories);
        
        List<string> shaders = new List<string>();
        foreach (string file in files)
        {
            string fullPath = file.Replace(@"\","/");
            string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
            Shader shader = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Shader)) as Shader;

            if(shader!= null)
                shaders.Add(assetPath);
        }

        return shaders;
    }
}

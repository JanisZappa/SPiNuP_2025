using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;


public static class ColorTools 
{
    [MenuItem("Colors/Palette/Set Old Colors")]
    private static void SetOld()
    {
        Palette palette = Resources.Load<Palette>("Palette");
        
        palette.colors = PaletteSource.Get.oldColors;
        EditorUtility.SetDirty(palette);
        
        Palette.Load();
        
        SetIDsToCostumes();
    }
    
    [MenuItem("Colors/Palette/Set New Colors")]
    private static void SetNew()
    {
        Palette palette = Resources.Load<Palette>("Palette");
        
        palette.colors = PaletteSource.Get.newColors;
        EditorUtility.SetDirty(palette);
        
        Palette.Load();

        SetIDsToCostumes();
    }

    
    private static void SetIDsToCostumes()
    {
        CostumeColors[] costumes = Assets.FindAll<CostumeColors>();

        for (int i = 0; i < costumes.Length; i++)
        {
            CostumeColors costume = costumes[i];

            for (int e = 0; e < costume.colors.Count; e++)
            {
                CostumeColors.CostumeColor color = costume.colors[e];
                color.colorID = (short)color.realColor.ClosestColorIndex(Palette.Colors);
            }
            
            EditorUtility.SetDirty(costume);
        }
    }

    [MenuItem("Colors/Debug Costume")]
    private static void DebugCostume()
    {
        if (!GameManager.Running)
            return;

        Rig rig = Spinner.Get(0).rig;
        Debug.Log(rig.cC.Log());
        Selection.activeObject = rig.cC;

        LogMesh(rig.GetComponent<SkinnedMeshRenderer>().sharedMesh);
    }
    
    
    [MenuItem("Colors/Houdini/Export")]
    private static void Export()
    {
        HoudiniVexExport(PaletteSource.Get.newColors);
        return;
        HoudiniPyExport("gamecolors", PaletteSource.Get.newColors);
        HoudiniPyExport("oldColors", NewColorGen.HoudiniUsed(true));
    }


    [MenuItem("Colors/Houdini/Get Used")]
    private static void ShowHoudiniUsed()
    {
        NewColorGen.HoudiniUsed();
    }
    
    
    private static float Dist(Color colorA, Color colorB)
    {
        return Mathf.Abs(colorA.r - colorB.r) + Mathf.Abs(colorA.g - colorB.g) + Mathf.Abs(colorA.b - colorB.b);
    }
    
    
    private static int GetColorIndex(Color[] colors, Color color)
    {
        float bestDist = 1000000;
        int pick = 0;

        int count = colors.Length;

        for(int i = 0; i < count; i++)
        {
            float dist = Dist(color, colors[i]);

            if(dist < bestDist)
            {
                bestDist = dist;
                pick = i;
            }
        }

        return pick;
    }
    
    
    private static void HoudiniVexExport(Color[] colors)
    {
        DateTime start = DateTime.Now;
        
        const string tab = "    ", tabtab = tab + tab;
		
        List<string> houdiniLines = new List<string>();
        houdiniLines.Add("int count()");
        houdiniLines.Add("{");
        houdiniLines.Add(tab + "return " + colors.Length + ";");
        houdiniLines.Add("}\n\n");
		
	//  Color Array
        houdiniLines.Add("vector color(int pick)");
        houdiniLines.Add("{");
        houdiniLines.Add(tab + "vector palette[] =");
        houdiniLines.Add(tab + "{");

        {
            int perLine = 0;
            string line = tabtab;
            int count = colors.Length;
            for (int i = 0; i < count; i++)
            {
                line += "{ " + ColorString(colors[i]) + "}" + (i < count - 1? "," : "");
                perLine++;

                if (perLine == 7 || i == count - 1)
                {
                    houdiniLines.Add(line);
                    line = tabtab;
                    perLine = 0;
                }
            }
        }
        
			
        houdiniLines.Add(tab + "};");
        houdiniLines.Add("");
        houdiniLines.Add(tab + "return palette[pick % len(palette)];");
        houdiniLines.Add("}");
        
    //  Color Index Map  
        houdiniLines.Add("");
        houdiniLines.Add("");
        houdiniLines.Add("int colorID(int pick)");
        houdiniLines.Add("{");
        houdiniLines.Add(tab + "int ids[] =");
        houdiniLines.Add(tab + "{");
    
        const int res = 64;
        const int max = res * res * res;
        const int multi = 256 / res;
        
        int[] map = new int[max];
        for(int r = 0; r < res; r++)
        for(int g = 0; g < res; g++)
        for(int b = 0; b < res; b++)
        {
            int index = r + g * res + b * res * res;
            map[index] = GetColorIndex(colors, new Color32((byte)(r * multi),(byte)(g * multi), (byte)(b * multi), 0));
        }

        {
            int perLine = 0;
            string line = tabtab;
            for (int i = 0; i < max; i++)
            {
                line += map[i].ToString().PadLeft(4) + (i < max - 1 ? "," : "");
                perLine++;

                if (perLine == 200 || i == max - 1)
                {
                    houdiniLines.Add(line);
                    line = tabtab;
                    perLine = 0;
                }
            }
        }


        houdiniLines.Add(tab + "};");
        houdiniLines.Add("");
        houdiniLines.Add(tab + "return ids[pick % len(ids)];");
        houdiniLines.Add("}");
        

        string[] colorToolLines = File.ReadAllLines(Application.dataPath + "/04_TextFiles/HoudiniTemplates/ColorTools.h");
        for (int i = 0; i < colorToolLines.Length; i++)
            houdiniLines.Add(colorToolLines[i]);	
        
        File.WriteAllLines(@"D:\HoudiniProjects\SpinUp\vex\include/gamecolors.h", houdiniLines.ToArray());
        
        TimeSpan span = DateTime.Now - start;
        Debug.LogFormat("Wrote Vex in {0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
    }


    private static void HoudiniPyExport(string libraryName, Color[] colors)
    {
        const string tab = "	", colorTab = tab + tab + tab + tab;
			
        List<string> houdiniLines = new List<string>();
		
        houdiniLines.Add("import numpy as np\n\n");
		
        for (int i = 0; i < colors.Length; i++)
        {
            string first = i == 0 ? "colors = np.array([" : colorTab;
            string end = i == colors.Length - 1 ? "])" : ",";
			
            houdiniLines.Add(first + "[ " + ColorString(colors[i]) + "]" + end);
        }
		
        string[] colorToolLines = File.ReadAllLines(Application.dataPath + "/04_TextFiles/HoudiniTemplates/ColorTools.py");
        for (int i = 0; i < colorToolLines.Length; i++)
            houdiniLines.Add(colorToolLines[i]);	
		
        File.WriteAllLines(@"D:\HoudiniProjects\SpinUp\vex\include/" + libraryName + ".py", houdiniLines.ToArray());
    }
    
    
    private static string FloatString(float value)
    {
        string returnString = value.ToString(CultureInfo.InvariantCulture);
        if (returnString.Length > 1 && returnString[0] == '0')
            returnString = returnString.Substring(1, returnString.Length - 1);

        return returnString;
    }


    private static string ColorString(Color color)
    {
        return FloatString(color.r).PadLeft(10) + ", " + FloatString(color.g).PadLeft(10) + ", " + FloatString(color.b).PadLeft(10);
    }
    
    
    [MenuItem("Assets/ColorIDs and MatCaps")]
     private static void MeshCheck()
     {
         if (Selection.activeObject == null)
             return;
 
         Mesh mesh = Selection.activeObject as Mesh;
 
         if (mesh != null)
            LogMesh(mesh);
     }


     private static void LogMesh(Mesh mesh)
     {
         Color[] colors = mesh.colors;
         List<Color> cols = new List<Color>();
         List<int> colorIDs = new List<int>();
         List<int> matcaps = new List<int>();
         for (int i = 0; i < colors.Length; i++)
         {
             int colorValue = Mathf.RoundToInt(colors[i].r * 256f);
             int matCapValue = Mathf.RoundToInt(colors[i].g * 256f);
             
             int matCapOffset = Mathf.FloorToInt(matCapValue / 16.0f);
 
             int resultID     = matCapOffset * 256 + colorValue;

             if (colorIDs.AddUnique(resultID))
             {
                 cols.Add(Palette.Colors[resultID]);
                 matcaps.Add(matCapValue % 16);
             }
                 
         }
         
         Debug.Log("Colors: " + colorIDs.Log());
         Debug.Log("Colors: " + cols.Log());
         Debug.Log("MatCaps: " + matcaps.Log());
     }
    
    [MenuItem("Assets/ColorIDs and MatCaps", true)]
    private static bool NewMenuOptionValidation()
    {
        // This returns true when the selected object is a Variable (the menu item will be disabled otherwise).
        return Selection.activeObject is Mesh;
    }

    
    #region Misc
    
    /*[MenuItem("Colors/Fix Costumes")]
    private static void FixCostumes()
    {
        CostumeColors[] costumes = Assets.FindAll<CostumeColors>();

        for (int i = 0; i < costumes.Length; i++)
        {
            for (int e = 0; e < costumes[i].colors.Count; e++)
            {
                CostumeColors.CostumeColor cc = costumes[i].colors[e];
                cc.realColor = PaletteSource.Get.oldColors[cc.color];
            }

            EditorUtility.SetDirty(costumes[i]);
        }
    }*/

    /*[MenuItem("Colors/Save old Colors")]
    private static void SaveOld()
    {
        if(PaletteSource.Get.oldColors.Length > 0)
            return;
        
        Palette palette = Resources.Load<Palette>("Palette");
        
        PaletteSource.Get.oldColors = palette.colors;
        EditorUtility.SetDirty(PaletteSource.Get);
    }*/

    #endregion
}

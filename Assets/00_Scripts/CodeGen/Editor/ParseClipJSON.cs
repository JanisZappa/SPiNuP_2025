using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEditor;


public static class ParseClipJSON
{
    public static JSONNode GetJSON()
    {
        string fileText = File.ReadAllText("Assets/00_Scripts/CodeGen/clips.json");
		
        return JSON.Parse(fileText);
    }
	
    public static JSONNode[] JSON_Nodes()
    {
        JSONNode node = GetJSON();

        List<JSONNode> list = new List<JSONNode>();

        for (int i = 0; i < node.Count; i++)
            foreach (JSONNode item in node[i].Children)
                list.Add(item);


        return list.ToArray();
    }
    
    
	
	//    Filter
	
	private const string csPath = "Assets/00_Scripts/CodeGen/clipType.cs";
	private const string eName = "ClipType", vName = "clipType";
	
	
	[MenuItem("CodeGen/clipType")]
    public static void ReloadClipDefinition()
    {
        JSONNode[] nodes = JSON_Nodes();

    //  Writing CS  //
        using (StreamWriter outfile =
            new StreamWriter(csPath))
        {
            outfile.WriteLine("namespace Clips");
            outfile.WriteLine("{");

            outfile.CreateEnum(1, nodes);
            outfile.Space(2);

            outfile.WriteLine(T(1) + "public static class ClipTypeFilter");
            outfile.WriteLine(T(1) + "{");

            {
            //  IsNotPlaying  //
                outfile.CreateBoolMethod("IsNotPlaying", "NotPlaying", 2, nodes, true);
                outfile.Space(2);
                
            //  IsSerializable  //
                outfile.CreateBoolMethod("IsSerializable", "Serializable", 2, nodes);
                outfile.Space(2);
                
            //  IsAnyJump  //
                outfile.CreateBoolMethod("IsAnyJump", "IsJump", 2, nodes);
                outfile.Space(2);
                
            //  IsAnySwing  //
                outfile.CreateBoolMethod("IsAnySwing", "IsSwing", 2, nodes);
            }


            outfile.WriteLine(T(1) + "}");

            outfile.WriteLine("}");
        }

        AssetDatabase.Refresh();
    }
    
    
    #region Helper
   

    
    
    private static string GetBuff(this string compareString, int max)
    {    
        int length = max - compareString.Length;
        
        string buffer = "";
        for (int i = 0; i < length; i++)
            buffer += " ";

        return buffer;
    }
    
    
    private static string T(int tabs)
    {
        const string gap  = "    ";
        string tabSpace = "";
        for (int i = 0; i < tabs; i++)
            tabSpace += gap;

        return tabSpace;
    }
        
    #endregion

    
    private static void CreateEnum(this StreamWriter outfile, int tabs, JSONNode[] nodes)
    {
        outfile.WriteLine(T(tabs) + "public enum " + eName);
        outfile.WriteLine(T(tabs) + "{");

        List<string> items = new List<string>();
        for (int i = 0; i < nodes.Length; i++)
            items.Add(nodes[i]["name"]);
		
        int longest = items.MaxStringLength();
		
        for (int i = 0; i < nodes.Length; i++)
        {
            string comma = i < nodes.Length - 1 ? "," : "";
            outfile.WriteLine(T(tabs + 1) + items[i].PadRight(longest) + " = " + (i + 1) + comma);
        }
		

        outfile.WriteLine(T(tabs) + "}");
    }


    private static void CreateBoolMethod(this StreamWriter outfile, string name, string key, int tabs, JSONNode[] nodes, bool includeZero = false)
    {
        outfile.WriteLine(T(tabs) + "public static bool " + name + "(this " + eName + " " + vName + ")");
        outfile.WriteLine(T(tabs) + "{");

        List<string> items = new List<string>();
        for (int i = 0; i < nodes.Length; i++)
        {
            JSONNode infos = nodes[i]["info"];
            foreach (JSONNode info in infos.Children)
                if (info.Value.Contains(key))
                {
                    items.Add(nodes[i]["name"]);
                    break;
                }
        }


        if (items.Count > 0 || includeZero)
        {
            int longest = items.MaxStringLength();

            outfile.WriteLine(T(tabs + 1) + "switch(" + vName + ")");
            outfile.WriteLine(T(tabs + 1) + "{");

            if (includeZero)
            {
                string result = items.Count > 0 ? ":" : ": " + " ".GetBuff(longest) + "return true;";
                outfile.WriteLine(T(tabs + 2) + "case 0" + result);
            } 
            
            for (int i = 0; i < items.Count; i++)
            {
                string result = i < items.Count - 1 ? ":" : ": " + items[i].GetBuff(longest) + "return true;";
                outfile.WriteLine(T(tabs + 2) + "case " + eName + "." + items[i] + result);
            }

            outfile.WriteLine(T(tabs + 1) + "}");
            outfile.Space(1);
        }

        outfile.WriteLine(T(tabs + 1) + "return false;");
        outfile.WriteLine(T(tabs) + "}");
    }
}

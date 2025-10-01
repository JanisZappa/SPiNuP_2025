using System.Collections.Generic;
using System.IO;
using System.Linq;
using LevelElements;
using SimpleJSON;
using UnityEditor;
using UnityEngine;


public static class ParseElementJSON 
{
	[MenuItem("CodeGen/elementType")]
	public static void Parse()
	{
		ElementAttributes();
		Masks();
		ParseGroupJSON.ParseGroups();
	}
	
	
	private static JSONNode GetJSON()
	{
		string fileText = File.ReadAllText("Assets/00_Scripts/CodeGen/elements.json");
		
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


	private static void ElementAttributes()
    {
        JSONNode[] nodes = JSON_Nodes();

    //  Writing CS  //
        const string csPath = "Assets/00_Scripts/CodeGen/elementType.cs";
    
        using (StreamWriter outfile =
            new StreamWriter(csPath))
        {
            outfile.WriteLine("using UnityEngine;");
            outfile.Space(1);

            outfile.WriteLine("namespace LevelElements");
            outfile.WriteLine("{");

            outfile.CreateEnum(1, nodes);
            outfile.Space(2);

            outfile.WriteLine(T(1) + "public static class ElementTypeExt");
            outfile.WriteLine(T(1) + "{");


           

            {
            //  Items and Fluff  //
                
            //  InstanceCount  //
                outfile.CreateIntMethod("InstanceCount", "inst", 0, 2, nodes);
                outfile.Space(2);
                
            //  Weight  //
                outfile.CreateFloatMethod("Mass", "mass", 1, 2, nodes);
                outfile.Space(2);
            
            //  Damp  //
                outfile.CreateFloatMethod("Damp", "damp", 1, 2, nodes);
                outfile.Space(2);
            
            //  Accel  //
                outfile.CreateFloatMethod("Accel", "accel", 1, 2, nodes);
                outfile.Space(2);
            
            //  Lazyness  //
                outfile.CreateFloatMethod("Lazyness", "lazy", 1, 2, nodes);
                outfile.Space(2);

            //  Radius  //
                outfile.CreateFloatMethod("Radius", "radius", Item.DefaultRadius, 2, nodes);
                outfile.Space(2);
                
            //  Value  //
				outfile.CreateIntMethod("Value", "value", 0, 2, nodes);
				outfile.Space(2);
            
            
            //  Items  //

            //  GetOtherEnd  //
                outfile.CreateWarpPairMethod("Warp_Start", 2, nodes);
                outfile.Space(2);
            
            
            //  Tracks  //

            //  MaxItems  //
                outfile.CreateIntMethod("MaxItems", "max", 1, 2, nodes);
                outfile.Space(2);

            //  DefaultVScale  //
                outfile.CreateFloatMethod("DefaultVScale", "vector", 0, 2, nodes);
                outfile.Space(2);

            
            //  All  //
            
            //  DebugColor  //
                outfile.CreateDebugColorMethod(2, nodes);
                outfile.Space(2);
                
            //  LongestCategoryName  //
                outfile.CreateCategoryNameLengthMethod(2, nodes);
                outfile.Space(2);
                
            //  Name  //
                outfile.CreateNameMethod(2, nodes);
                outfile.Space(2);
            }


            outfile.WriteLine(T(1) + "}");

            outfile.WriteLine("}");
        }

        AssetDatabase.Refresh();
    }
    
	
	private static string T(int tabs)
	{
		const string gap  = "    ";
		string tabSpace = "";
		for (int i = 0; i < tabs; i++)
			tabSpace += gap;

		return tabSpace;
	}
	
	
	private static string GetBuff(this string compareString, int max)
	{    
		int length = max - compareString.Length;
        
		string buffer = "";
		for (int i = 0; i < length; i++)
			buffer += " ";

		return buffer;
	}
	
	
	private const string eName = "elementType", vName = "value";
	
	
	private static void CreateEnum(this StreamWriter outfile, int tabs, JSONNode[] nodes)
	{
		const string elementIDPath = "00_Scripts/CodeGen/Editor/elementIDs";
		List<string> idLines = ProjectTxt.Read(elementIDPath).ToList();
		
		outfile.WriteLine(T(tabs) + "public enum " + eName);
		outfile.WriteLine(T(tabs) + "{");

		List<string> items = new List<string>();
		for (int i = 0; i < nodes.Length; i++)
			items.Add(nodes[i]["name"]);

	//  Remove unused idLines
		for (int i = 0; i < idLines.Count; i++)
		{
			string name = idLines[i].Split('|')[0];
			for (int e = 0; e < items.Count; e++)
				if (name == items[e])
					goto ItsFine;

			idLines.RemoveAt(i);
			i--;
			Debug.Log("Removing idline " + name);
			
			ItsFine: ;
		}
		
		
		Dictionary<string, int> groupOffsets = new Dictionary<string, int>()
		{
			{"Item",              0}, 
			{"Track",       1000000},
			{"Fluff",       2000000},
			{"Collectable", 3000000},
			{"Link",        4000000},
		};
		
		string groupName = "";
		int offset = 0;
		
		int longest = items.MaxStringLength();
		for (int i = 0; i < nodes.Length; i++)
		{
			JSONNode infos = nodes[i]["info"];
			foreach (JSONNode info in infos.Children)
				if (info.Value.Contains("!"))
				{
					if (groupName != "")
						outfile.Space(2);

					groupName = info.Value.Replace("!", "");
					offset = groupOffsets[groupName];
					outfile.WriteLine(T(tabs) + "//  " + groupName + "  //");

					break;
				}

			int bestID = -1;
			string name = items[i];
		//  Reuse ID if old element  //
			for (int e = 0; e < idLines.Count; e++)
			{
				string line = idLines[e];
				if (line.Split('|')[0] == name)
				{
					bestID = int.Parse(line.Split('|')[1]);
					break;
				}	
			}

		
			if (bestID == -1)
			{
			//  Find lowest available ID  //
				bestID = Mathf.Max(1, offset);
				while (true)
				{
					for (int e = 0; e < idLines.Count; e++)
					{
						int id = int.Parse(idLines[e].Split('|')[1]);
						if (id == bestID)
						{
							bestID++;
							goto TryAgain;
						}
					}

					idLines.Add(name + "|" + bestID);
					break;
					
					TryAgain: ;
				}
			}
			
			
			string comma = i < nodes.Length - 1 ? "," : "";
			outfile.WriteLine(T(tabs + 1) + name.PadRight(longest) + " = " + bestID + comma);
		}
		
		ProjectTxt.Write(elementIDPath, idLines.ToArray());

		outfile.WriteLine(T(tabs) + "}");
	}
	
	
	private static void CreateIntMethod(this StreamWriter outfile, string name, string key, int defaultValue, int tabs, JSONNode[] nodes)
    {
        outfile.WriteLine(T(tabs) + "public static int " + name + "(this " + eName + " " + vName + ")");
        outfile.WriteLine(T(tabs) + "{");

        List<string> items = new List<string>();
        List<string> ints  = new List<string>();
        
        for (int i = 0; i < nodes.Length; i++)
        {
	        JSONNode valueNode = nodes[i][key];
	        if (valueNode != null)
	        {
		        items.Add(nodes[i]["name"]);
		        ints.Add(valueNode.Value);
	        }
        }
        

        if (items.Count > 0)
        {
            outfile.WriteLine(T(tabs + 1) + "switch(" + vName + ")");
            outfile.WriteLine(T(tabs + 1) + "{");

            int longest = items.MaxStringLength();
            
            while (items.Count != 0)
            {
                string r = ints[0];
                int count = 0;
                for (int i = 0; i < ints.Count; i++)
                    if (ints[i] == r)
                        count++;

                for (int i = 0; i < count; i++)
                for (int e = 0; e < ints.Count; e++)
                    if (ints[e] == r)
                    {
                        bool showResult = i == count - 1;
                        string result = showResult ? items[e].GetBuff(longest) + " return " + r + ";" : "";
                        outfile.WriteLine(T(tabs + 2) + "case " + eName + "." + items[e] + ":" + result);
                        items.RemoveAt(e);
                        ints.RemoveAt(e);
                        break;
                    }

                if (items.Count != 0)
                    outfile.Space(1);
            }

            outfile.WriteLine(T(tabs + 1) + "}");
            outfile.Space(1);
        }

        outfile.WriteLine(T(tabs + 1) + "return " + defaultValue + ";");
        outfile.WriteLine(T(tabs) + "}");
    }
	
	
	private static void CreateFloatMethod(this StreamWriter outfile, string name, string key, float defaultValue, int tabs, JSONNode[] nodes)
	{
        outfile.WriteLine(T(tabs) + "public static float " + name + "(this " + eName + " " + vName + ")");
        outfile.WriteLine(T(tabs) + "{");

        List<string> items  = new List<string>();
        List<string> floats = new List<string>();
        
        for (int i = 0; i < nodes.Length; i++)
        {
	        JSONNode valueNode = nodes[i][key];
	        if (valueNode != null)
	        {
		        items.Add(nodes[i]["name"]);
		        floats.Add(valueNode.Value);
	        }
        }
        
       
        if (items.Count > 0)
        {
            outfile.WriteLine(T(tabs + 1) + "switch(" + vName + ")");
            outfile.WriteLine(T(tabs + 1) + "{");

            int longest = items.MaxStringLength();
            
            while (items.Count != 0)
            {
                string r = floats[0];
                int count = 0;
                for (int i = 0; i < floats.Count; i++)
                    if (floats[i] == r)
                        count++;

                for (int i = 0; i < count; i++)
					for (int e = 0; e < floats.Count; e++)
						if (floats[e] == r)
						{
							bool showResult = i == count - 1;
							string result = showResult ? items[e].GetBuff(longest) + " return " + FloatString(r) + ";" : "";
							outfile.WriteLine(T(tabs + 2) + "case " + eName + "." + items[e] + ":" + result);
							 items.RemoveAt(e);
							floats.RemoveAt(e);
							break;
						}

                if (items.Count != 0)
                    outfile.Space(1);
            }

            outfile.WriteLine(T(tabs + 1) + "}");
            outfile.Space(1);
        }


        outfile.WriteLine(T(tabs + 1) + "return " + FloatString(defaultValue.ToString()) + ";");
        outfile.WriteLine(T(tabs) + "}");
    }


	private static string FloatString(string value)
	{
		if (value.Length > 1 && value[0] == '0')
			value = value.Remove(0, 1);

		if (value.Contains("."))
			return value + "f";

		return value;
	}
	
	
	private static void CreateNameMethod(this StreamWriter outfile, int tabs, JSONNode[] nodes)
	{
		outfile.WriteLine(T(tabs) + "public static string Name(this " + eName + " " + vName + ")");
		outfile.WriteLine(T(tabs) + "{");

		List<string> items = new List<string>();
		for (int i = 0; i < nodes.Length; i++)
			items.Add(nodes[i]["name"]);
		

		if (items.Count > 0)
		{
			outfile.WriteLine(T(tabs + 1) + "switch(" + vName + ")");
			outfile.WriteLine(T(tabs + 1) + "{");

			int longest = items.MaxStringLength();
			
			for (int i = 0; i < items.Count; i++)
			{
				string result = items[i].GetBuff(longest) + " return \"" + items[i] + "\";";
				outfile.WriteLine(T(tabs + 2) + "case " + eName + "." + items[i] + ":" + result);
			}

			outfile.WriteLine(T(tabs + 1) + "}");
			outfile.Space(1);
		}

		outfile.WriteLine(T(tabs + 1) + "return \"???\";");
		outfile.WriteLine(T(tabs) + "}");
	}
	
	
	private static void CreateWarpPairMethod(this StreamWriter outfile, string key, int tabs, JSONNode[] nodes)
	{
		outfile.WriteLine(T(tabs) + "public static " + eName + " GetOtherEnd(this " + eName + " " + vName + ")");
		outfile.WriteLine(T(tabs) + "{");

		List<string> items = new List<string>();

		for (int i = 0; i < nodes.Length; i++)
		{
			JSONNode infos = nodes[i]["info"];
			foreach (JSONNode info in infos.Children)
				if (info.Value == key)
				{
					items.Add(nodes[i]["name"]);
					items.Add(nodes[i + 1]["name"]);
					i++;
					break;
				}
		}			


		if (items.Count > 0)
		{
			outfile.WriteLine(T(tabs + 1) + "switch(" + vName + ")");
			outfile.WriteLine(T(tabs + 1) + "{");
			
			int longest = items.MaxStringLength();

			for (int i = 0; i < items.Count; i += 2)
			{
				string a = items[i];
				string b = items[i + 1];

				outfile.WriteLine(T(tabs + 2) + "case " + eName + "." + a + ": " + a.GetBuff(longest) + "return " + eName + "." + b + ";");
				outfile.WriteLine(T(tabs + 2) + "case " + eName + "." + b + ": " + b.GetBuff(longest) + "return " + eName + "." + a + ";");
			}

			outfile.WriteLine(T(tabs + 1) + "}");
			outfile.Space(1);
		}

		outfile.WriteLine(T(tabs + 1) + "return " + vName + ";");
		outfile.WriteLine(T(tabs) + "}");
	}
	
	
	private static void CreateDebugColorMethod(this StreamWriter outfile, int tabs, JSONNode[] nodes)
    {
        outfile.WriteLine(T(tabs) + "public static Color DebugColor(this " + eName + " " + vName + ")");
        outfile.WriteLine(T(tabs) + "{");

        List<string> items  = new List<string>();
        List<string> colors = new List<string>();
        for (int i = 0; i < nodes.Length; i++)
        {
	        JSONNode valueNode = nodes[i]["debug"];
	        if (valueNode != null)
	        {
		        items.Add(nodes[i]["name"]);
		        colors.Add(valueNode.Value);
	        }
        }

        if (items.Count > 0)
        {
            outfile.WriteLine(T(tabs + 1) + "switch(" + vName + ")");
            outfile.WriteLine(T(tabs + 1) + "{");

            int longest = items.MaxStringLength();
            
            while (items.Count != 0)
            {
                string r = colors[0];
                int count = 0;
                for (int i = 0; i < colors.Count; i++)
                    if (colors[i] == r)
                        count++;

                for (int i = 0; i < count; i++)
                for (int e = 0; e < colors.Count; e++)
                    if (colors[e] == r)
                    {
                        bool showResult = i == count - 1;
                        string result = showResult ? items[e].GetBuff(longest) + " return COLOR." + r + ";" : "";
                        outfile.WriteLine(T(tabs + 2) + "case " + eName + "." + items[e] + ":" + result);
                        items.RemoveAt(e);
                        colors.RemoveAt(e);
                        break;
                    }

                if (items.Count != 0)
                    outfile.Space(1);


            }

            outfile.WriteLine(T(tabs + 1) + "}");
            outfile.Space(1);

            outfile.WriteLine(T(tabs + 1) + "return COLOR.blue.cornflower;");
            outfile.WriteLine(T(tabs) + "}");
        }
    }
	
	
	private static void CreateCategoryNameLengthMethod(this StreamWriter outfile, int tabs, JSONNode[] nodes)
	{
		int longestG = 0;
		List<string> gNames = new List<string>();
		
		
		for (int i = 0; i < nodes.Length; i++)
		{
			JSONNode infos = nodes[i]["info"];
			foreach (JSONNode info in infos.Children)
				if (info.Value.Contains("!"))
				{
					string gN = info.Value.Replace("!", "");
					gNames.Add(gN);
					longestG = Mathf.Max(longestG, gN.Length);
					break;
				}
		}
		
        
		for (int g = 0; g < gNames.Count; g++)
		{
			int longest = 0;
			bool count = false;
                
			for (int i = 0; i < nodes.Length; i++)
			{
				JSONNode infos = nodes[i]["info"];
				foreach (JSONNode info in infos.Children)
					if (info.Value.Contains("!"))
					{
						count = info.Value.Contains(gNames[g]);
						break;
					}

				if (count)
					longest = Mathf.Max(longest, nodes[i]["name"].ToString().Length - 2);
			}

			string name = ("Longest" + gNames[g] + "Name").PadRight(longestG + 11);
			outfile.WriteLine(T(tabs) + "public const int " + name + " = " + longest + ";");
		}
	}

	
	private static void Masks()
	{
		JSONNode[] nodes = JSON_Nodes();
		
		using (StreamWriter outfile =
			new StreamWriter("Assets/00_Scripts/CodeGen/ElementMask.cs"))
		{
			outfile.WriteLine("using LevelElements;");
			outfile.Space(2);
			outfile.WriteLine("public abstract class ElementMask");
			outfile.WriteLine("{");
			outfile.WriteLine(T(1) + "public abstract bool Fits(elementType elementType);");
			outfile.WriteLine("}");
			
			outfile.Space(2);
			
			List<string> masks = new List<string>();
			
			outfile.WriteLine("public static partial class MaskTypes");
			outfile.WriteLine("{");
			
			masks.Add(CreateMask(outfile, "IsItem",        new[] {"IsItem"},        nodes, 1));
			masks.Add(CreateMask(outfile, "IsCollectable", new[] {"IsCollectable"}, nodes, 1));
			masks.Add(CreateMask(outfile, "IsFluff",       new[] {"IsFluff"},       nodes, 1));
			masks.Add(CreateMask(outfile, "IsTrack",       new[] {"IsTrack"},       nodes, 1));
			
			masks.Add(CreateMask(outfile, "CanBeGrabbed", new[] {"Grab"},                               nodes, 1));
			masks.Add(CreateMask(outfile, "AnyThing",     new[] {"IsItem", "IsCollectable", "IsFluff"}, nodes, 1, false));
			masks.Add(CreateMask(outfile, "MustShake",    new[] {"Shake"},                              nodes, 1));
			masks.Add(CreateMask(outfile, "ShakeItem",    new[] {"IsItem", "Shake"},                    nodes, 1));
			masks.Add(CreateMask(outfile, "ShakeFluff",   new[] {"IsFluff", "Shake"},                   nodes, 1));
			
			masks.Add(CreateMask(outfile, "HidingShakeFluff",   new[] {"IsFluff", "Shake", "Hide"}, nodes, 1));
			
			masks.Add(CreateMask(outfile, "Action",       new[] {"Action"},                             nodes, 1));

			masks.Add(CreateMask(outfile, "CanMove",       new[] {"Move"}, nodes, 1));
			masks.Add(CreateMask(outfile, "Hide",          new[] {"Hide"}, nodes, 1));
			masks.Add(CreateMask(outfile, "CreatorButton", new[] {"Button"}, nodes, 1));
			
			masks.Add(CreateMask(outfile, "TrackCanGrow", new[] {"TrackGrow"}, nodes, 1));
			masks.Add(CreateMask(outfile, "TrackCanTurn", new[] {"TrackTurn"}, nodes, 1));
			
			masks.Add(CreateMask(outfile, "Warp",            new[] {"Warp"},       nodes, 2));
			masks.Add(CreateMask(outfile, "Warp_Start",      new[] {"Warp_Start"}, nodes, 2));
			masks.Add(CreateMask(outfile, "Warp_SideSwitch", new[] {"Warp_Side"},  nodes, 2));
			
			outfile.WriteLine("}");
			
			outfile.Space(2);
			outfile.WriteLine("public static partial class Mask");
			outfile.WriteLine("{");

			int count = masks.Count;
			int longestMaskName = 0;
			for (int i = 0; i < count; i++)
				longestMaskName = Mathf.Max(longestMaskName, masks[i].Length);
			
			
			for (int i = 0; i < count; i++)
			{
				string name = masks[i];
				outfile.WriteLine(T(1) + "public static readonly MaskTypes." + (name + "_Mask ").PadRight(longestMaskName + 6) + name.PadRight(longestMaskName) + " = new MaskTypes." + name +"_Mask();");
			}
			
			outfile.WriteLine("}");
		}
	}


	private static string CreateMask(this StreamWriter outfile, string maskName, string[] masks, JSONNode[] nodes, int tabs, bool matchAll = true)
	{
		List<string> items = new List<string>();

		bool checkGroups = false;
		for (int i = 0; i < masks.Length; i++)
			if (masks[i].Substring(0, 2) == "Is")
			{
				checkGroups = true;
				break;
			}

		string group = "";
		for (int i = 0; i < nodes.Length; i++)
		{
			JSONNode infos = nodes[i]["info"];

			bool anyMatch = false;
			bool allMatch = true;
			
		//  Group Check  //
			if(checkGroups)
				foreach (JSONNode info in infos.Children)
				{
					string value = info.Value;
	
					if (value.Contains("!"))
						group = value.Replace("!", "Is");
				}
			

			for (int e = 0; e < masks.Length; e++)
			{
				string mask = masks[e];

				bool hasMatch = checkGroups && group.Contains(mask);

				if (!hasMatch)
				{
					if (mask.Contains("#"))
					{
						mask = mask.Replace("#", "");

						hasMatch = true;
						foreach (JSONNode info in infos.Children)
							if(info.Value.Contains(mask))
								hasMatch = false;
					}
					else
						foreach (JSONNode info in infos.Children)
							if(info.Value.Contains(mask))
								hasMatch = true;
				}
					
				
				if(hasMatch)
					anyMatch = true;
				else
					allMatch = false;
			}


			if (matchAll && allMatch || !matchAll && anyMatch)
			{
				string item = nodes[i]["name"];

				items.AddUnique(item);
			}	
		}

		items.Sort();
		
		
		outfile.WriteLine(T(tabs) + "public class " + maskName + "_Mask : ElementMask");
		outfile.WriteLine(T(tabs) + "{");
		outfile.WriteLine(T(tabs + 1) + "public override bool Fits(elementType elementType)");
		outfile.WriteLine(T(tabs + 1) + "{");
		
		outfile.WriteLine(T(tabs + 2) + "switch(elementType)");
		outfile.WriteLine(T(tabs + 2) + "{");

		int longestItemName = items.MaxStringLength();
		
		for (int i = 0; i < items.Count; i++)
			outfile.WriteLine(T(tabs + 3) + "case elementType." + items[i].PadRight(longestItemName) + " :" + (i == items.Count - 1? " return true;" : ""));
		
		outfile.Space(1);
		outfile.WriteLine(T(4) + "default: return false;");
		outfile.WriteLine(T(tabs + 2) + "}");
		
		outfile.WriteLine(T(tabs + 1) + "}");
		outfile.WriteLine(T(tabs) + "}");

		return maskName;
	}
}
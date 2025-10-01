using System.IO;
using UnityEditor;
using UnityEngine;


public class CreateCharPartsIndex : MonoBehaviour 
{
	[MenuItem("Tools/CreateCharPartsIndex")]
	public static void CCPI()
	{
	//  Writing CS  //
		const string csPath = "Assets/00_Scripts/Costumes/CharPartIndex.cs";

		using (StreamWriter outfile =
			new StreamWriter(csPath))
		{
			outfile.WriteLine("public enum CharPartType : byte");
			outfile.WriteLine("{");

			
			for (int i = 0; i < 4; i++)
			{
				int o = i * 64;
				
				Part("Skin",       0 + o, outfile);
				Part("Eye",        1 + o, outfile);
				Part("Pupil",      2 + o, outfile);
				Part("Mouth",      3 + o, outfile);
				Part("Nose",       4 + o, outfile);
				Part("Ear",        5 + o, outfile);
				Part("Brow",       6 + o, outfile);
				Part("Hair",       7 + o, outfile);
				Part("Beard",      8 + o, outfile);
				Part("Shirt",      9 + o, outfile);
			
				Part("Pants",     10 + o, outfile);
				Part("Socks",     11 + o, outfile);
				Part("Underwear", 12 + o, outfile);
				Part("Accessory", 13 + o, outfile);
				Part("Belt",      14 + o, outfile);
				
				outfile.WriteLine("");
			}
			
			
			outfile.WriteLine("}");
		}
		
		AssetDatabase.Refresh();
	}


	private static void Part(string name, int value, StreamWriter outfile)
	{
		string sub;
		int subIndex= Mathf.FloorToInt(value / 64f);
		switch (subIndex)
		{
			default: sub = "";   break;
			case 1:  sub = "_B"; break;
			case 2:  sub = "_C"; break;
			case 3:  sub = "_D"; break;
		}
		
		outfile.WriteLine(T(1) + (name + sub).PadRight(15) + " = " + value + ",");
	}
	
	
	private static string T(int tabs)
	{
		const string gap  = "    ";
		string tabSpace = "";
		for (int i = 0; i < tabs; i++)
			tabSpace += gap;

		return tabSpace;
	}
}

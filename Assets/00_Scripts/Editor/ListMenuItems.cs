using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public static class ListMenuItems 
{
    [MenuItem("Tools/Project Info/List MenuItems")]
    private static void List()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        List<string> lines = new List<string>();
        
        for (int i = 0; i < files.Length; i++)
        {
            if(files[i].Contains("ListMenuItems"))
                continue;
            
            string[] fileLines = File.ReadAllLines(files[i]);
            for (int e = 0; e < fileLines.Length; e++)
            {
                string line = fileLines[e];
                if (line.Contains("MenuItem") &&
                    !line.Contains("ExecuteMenuItem"))
                {
                    string trimmedLine = line.Split('"')[1];
                    string[] lineParts = trimmedLine.Split(' ');
                    string last = lineParts[lineParts.Length - 1];

                    if (last.Contains("#") || last.Contains("&") || last.Contains("_") || last.Contains("%"))
                    {
                        string adjustedLine = "";
                        for (int l = 0; l < lineParts.Length - 1; l++)
                            adjustedLine += lineParts[l] + " ";

                        adjustedLine += "! ";
                        adjustedLine += last;
                        
                        lines.Add(adjustedLine);
                    }
                       
                }
            }
        }
        
    //  Sort Lines  //
        int longest = 0;
        for (int i = 0; i < lines.Count; i++)
            longest = Mathf.Max(longest, lines[i].Split('!')[0].Length);

        for (int i = 0; i < lines.Count; i++)
        {
            string[] parts = lines[i].Split('!');
            lines[i] = parts[0].PadRight(longest) + parts[1];
        }
        
        lines.Sort();
        
        ProjectTxt.Write("04_TextFiles/MenuItems", lines.ToArray());
    }
}
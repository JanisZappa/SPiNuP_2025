using System;
using System.Collections.Generic;
using System.IO;
// Boo.Lang;
using UnityEditor;
using UnityEngine;


public static class CountLines
{
    private static readonly string[] Ignore = { "FbxExporters", "Plugins", "ThirdParty", "CountLines", ".meta", "_OTHER", "ZappaTools" };
    
    
    [MenuItem("Tools/Project Info/Count Lines")]
    public static void Count()
    {
       string[] file = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
       
       List<string> csNames = new List<string>();
       List<int>    csLines = new List<int>();
       
       List<string> goodLines = new List<string>();

       int goodOnes      = 0;
       int lineCount     = 0;
       int goodLineCount = 0;
       int longestName   = 0; 

       int comments = 0;
       int usings   = 0;
       int hashs    = 0;
       int sqiggly  = 0;
       int empty    = 0;
       
       for (int i = 0; i < file.Length; i++)
       {
           bool allGood = true;
           for (int e = 0; e < Ignore.Length; e++)
               if (file[i].Contains(Ignore[e]))
               {
                   allGood = false;
                   break;
               }

           if (!allGood)
               continue;
           
           
           string[] lines = File.ReadAllLines(file[i]);
           lineCount += lines.Length;

           int fileLineCount = 0;

           for (int e = 0; e < lines.Length; e++)
           {
               string line = lines[e].Replace(" ", "").Replace("\t","");
               if (line.Length > 0)
               {
                   if (line[0] == '/' && line[1] == '/')
                   {
                       comments++;
                       continue;
                   }

                   if (line[0] == 'u' && line[1] == 's' && line[2] == 'i' && line[3] == 'n' && line[4] == 'g')
                   {
                       usings++;
                       continue;
                   }

                   if (line[0] == '#')
                   {
                       hashs++;
                       continue;
                   }

                   if (line[0] == '{' || line[0] == '}')
                   {
                       sqiggly++;
                       continue;
                   }

                   goodLines.Add(line);
                   fileLineCount++;
               }
               else
                   empty++;
           }

           if (fileLineCount > 0)
           {
               goodLineCount += fileLineCount;
               goodOnes++;
               
               csLines.Add(fileLineCount);

               string   name  = file[i].Replace("\\", "/").Replace(Application.dataPath + "/", "");
               string[] parts = name.Split('/');
               
               csNames.Add(parts[parts.Length - 1]);

               longestName = Mathf.Max(longestName, csNames[csNames.Count - 1].Length);
           }
       }
       
       
       List<string> csInfo = new List<string>();
       
       csInfo.Add(DateTime.Now.ToShortDateString() + " -- " + Mathf.Floor((float)DateTime.Now.TimeOfDay.TotalHours) + ":" + DateTime.Now.Minute);
       csInfo.Add("");
       csInfo.Add("");
       {
           int max = Mathf.FloorToInt(Mathf.Max(Mathf.Log10(goodOnes), Mathf.Log10(file.Length)));
           csInfo.Add(file.Length.ToString().PadLeft(max) + "    Total C# Files");
           csInfo.Add(   goodOnes.ToString().PadLeft(max) + " Filtered C# Files");
       }
       csInfo.Add("");
       csInfo.Add("-- -- -- -- --");
       {
           int max = Mathf.FloorToInt(Mathf.Max(Mathf.Log10(goodLineCount), Mathf.Log10(lineCount))) + 1;
           csInfo.Add(    lineCount.ToString().PadLeft(max) + "    Total Lines");
           csInfo.Add(goodLineCount.ToString().PadLeft(max) + " Filtered Lines");
           csInfo.Add("");
           csInfo.Add(empty.ToString().PadLeft(max) + " Empty");
           csInfo.Add(comments.ToString().PadLeft(max) + " Comments");
           csInfo.Add(usings.ToString().PadLeft(max)   + " Usings");
           csInfo.Add(hashs.ToString().PadLeft(max)    + " Hashs");
           csInfo.Add(sqiggly.ToString().PadLeft(max)  + " Squigglys");
       }
       csInfo.Add("-- -- -- -- --");
       csInfo.Add("");
       csInfo.Add("");


       int height = Mathf.CeilToInt(csLines.Count / 4f);
       
       List<string> nameList = new List<string>();
       for (int i = 0; i < height; i++)
           nameList.Add("");
      
       {
           int l   = 0;
           int max = 0;
           
           while (true)
           {
               int longest = 0;
               int pick = 0;
               for (int i = 0; i < csLines.Count; i++)
                   if (csLines[i] > longest)
                   {
                       longest = csLines[i];
                       pick = i;
                   }

               if (l == 0)
                   max = Mathf.FloorToInt(Mathf.Log10(csLines[pick])) + 1;
               
                   
               
               string line = csLines[pick].ToString().PadLeft(max) + " - " + csNames[pick];
               nameList[l] += line;

               l = (l + 1) % height;

               if (l == 0)
               {
                   int maxLength = 0;
                   for (int i = 0; i < height; i++)
                       maxLength = Mathf.Max(maxLength, nameList[i].Length);

                   maxLength += 3;

                   for (int i = 0; i < height; i++)
                       nameList[i] = nameList[i].PadRight(maxLength);
               }
               
               csLines.RemoveAt(pick);
               csNames.RemoveAt(pick);

               if (csLines.Count == 0)
                   break;
           }
       }
       
       for (int i = 0; i < height; i++)
           csInfo.Add(nameList[i]);
       
       
       csInfo.Add("");
       csInfo.Add("");
       csInfo.Add("-- -- -- -- --");
       csInfo.Add("");
       csInfo.Add("- Filtered Lines -");
       csInfo.Add("");
       {
           int count = goodLines.Count;
           for (int i = 0; i < count; i++)
               csInfo.Add(goodLines[i]);
       }

       File.WriteAllLines(Application.dataPath.Replace("Assets", "C#Info.txt"), csInfo.ToArray());
    }
}

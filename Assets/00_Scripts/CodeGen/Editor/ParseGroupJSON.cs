using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEditor;
using UnityEngine;


public static class ParseGroupJSON 
{
    public static void ParseGroups()
    {
        List<GroupElements> groups = new List<GroupElements>();
        
        JSONNode[] nodes = ParseElementJSON.JSON_Nodes();
        for (int i = 0; i < nodes.Length; i++)
        {
            JSONNode valueNode = nodes[i]["group"];
            if (valueNode != null)
            {
                string name = nodes[i]["name"];

                foreach (JSONNode info in valueNode.Children)
                {
                    string groupName = info.Value;
                    
                    GroupElements group = null;
                    for (int e = 0; e < groups.Count; e++)
                        if (groups[e].name == groupName)
                        {
                            group = groups[e];
                            break;
                        }
                    
                    if (group == null)
                    {
                        group = new GroupElements(groupName);
                        groups.Add(group);
                    }
                    
                    group.elements.Add(name);
                }
            }    
        }
        
        int longest = 0;
        for (int i = 0; i < groups.Count; i++)
            longest = Mathf.Max(longest, groups[i].name.Length - 2);
        
    //  Writing CS  //
        const string csPath = "Assets/00_Scripts/CodeGen/Groups.cs";
        
        using (StreamWriter outfile =
            new StreamWriter(csPath))
        {
            outfile.WriteLine("using LevelElements;");
            outfile.Space(1);
            outfile.WriteLine("public static class Groups");
            outfile.WriteLine("{");
            outfile.WriteLine(T(1) + "static Groups()");
            outfile.WriteLine(T(1) + "{");

            const string groupString = " = new Group(new []{ ";

            int gapLength = T(2).Length + longest + groupString.Length;
            string gap = "";
            for (int i = 0; i < gapLength; i++)
                gap += " ";

            int groupCount = groups.Count;
            for (int i = 0; i < groupCount; i++)
            {
                GroupElements group = groups[i];
                int nameCount = group.elements.Count;
                for (int e = 0; e < group.elements.Count; e++)
                {
                    string line;
                    if (e == 0)
                        line = T(2) + group.name.Replace("\"","").PadRight(longest) + groupString + "elementType." + group.elements[e];
                    else
                        line = gap + "elementType." + group.elements[e];
                    
                    outfile.WriteLine(line + (e < nameCount - 1? "," : " });"));
                }
                
                if(i < groupCount - 1)
                    outfile.Space(1);
            }
            
            {
                outfile.Space(1);
                string line = T(2) + "All = new[] { ";
                for (int i = 0; i < groupCount; i++)
                {
                    line += groups[i].name.Replace("\"", "");
                    line += i < groupCount - 1 ? ", " : " };";
                }
                
                outfile.WriteLine(line);
            }
            
            
            outfile.WriteLine(T(1) + "}");
            outfile.Space(2);

            {
                string line = T(1) + "private static readonly Group ";
                
                for (int i = 0; i < groupCount; i++)
                {
                    line += groups[i].name.Replace("\"", "");
                    line += i < groupCount - 1 ? ", " : ";";
                }
                
                outfile.WriteLine(line);
            }
            {
                outfile.WriteLine(T(1) + "public static readonly Group[] All;");
            }
            
            outfile.Space(2);
            
        //  Alias  //
            outfile.WriteLine(T(1) + "public static Group Alias(this elementType elementType)");
            outfile.WriteLine(T(1) + "{");
            outfile.WriteLine(T(2) + "switch(elementType)");
            outfile.WriteLine(T(2) + "{");
            
            outfile.WriteLine(T(3) + "default: return null;");
            
            outfile.Space(1);

            for (int i = 0; i < groupCount; i++)
            {
                GroupElements group = groups[i];
                int nameCount = group.elements.Count;

                for (int e = 0; e < nameCount; e++)
                    outfile.WriteLine(T(3) + "case elementType." + group.elements[e] + (e < nameCount - 1 ? ":" : ": return " + group.name + ";"));
                
                if(i < groupCount - 1)
                    outfile.Space(1); 
            }
            
            outfile.WriteLine(T(2) + "}");
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


    private class GroupElements
    {
        public readonly string name;
        public readonly List<string> elements = new List<string>();

        public GroupElements(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            string value = name + " |";
            for (int i = 0; i < elements.Count; i++)
                value += " " + elements[i];

            return value.B_Pink();
        }
    }
}
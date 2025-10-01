using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


public static class UsedKeys 
{
    [MenuItem("Tools/Project Info/Find used Keys")]
    public static void FindKeys()
    {
        List<string> allKeyCodes = new List<string>();
        foreach(KeyCode vKey in Enum.GetValues(typeof(KeyCode)))
            allKeyCodes.Add(vKey.ToString().Replace("KeyCode.", ""));

        for (int i = 0; i < allKeyCodes.Count; i++)
        {
            string c = allKeyCodes[i];
            if (c.Contains("None") || 
                c.Contains("Joystick") && !c.Contains("JoystickB") ||
                c.Contains("Keypad"))
            {
                allKeyCodes.RemoveAt(i);
                i--;
            }     
        }
        
        
        string[] file = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        List<string> keyLines = new List<string>();
        
        for (int i = 0; i < file.Length; i++)
        {
            if(file[i].Contains("UsedKeys"))
                continue;
            
            string[] lines = File.ReadAllLines(file[i]);

            for (int e = 0; e < lines.Length; e++)
            {
                string line = lines[e];
                
                if(!line.Contains("KeyCode"))
                    continue;
                
                string fileName = file[i].Split('\\').Last().Replace(".cs", "");
                
                if(!AddIf(line, "Input.GetKey(", "★", fileName, keyLines, allKeyCodes))
                    if (!AddIf(line, "Input.GetKeyDown(", "🡇", fileName, keyLines, allKeyCodes))
                        if(!AddIf(line, "Input.GetKeyUp(", "🡅", fileName, keyLines, allKeyCodes))
                            AddIf(line, "case KeyCode.", "⚋", fileName, keyLines, allKeyCodes);
            }
        }

        Pad(keyLines);
        
        List<string> merged = new List<string>();
        while (keyLines.Count > 0)
        {
            string line = keyLines[0];
            keyLines.RemoveAt(0);
            string keyPress = line.Split('|')[0];

            for (int i = 0; i < keyLines.Count; i++)
            {
                string[] parts = keyLines[i].Split('|');
                string thisKeyPress = parts[0];
                if (thisKeyPress == keyPress)
                {
                    line += parts[1];
                    keyLines.RemoveAt(i);
                    i--;
                }
            }
            
            merged.Add(line);
        }
        
        List<string> alphabet = new List<string>();
        for (int i = 0; i < merged.Count; i++)
        {
            string keyPress = merged[i].Split('|')[0].Replace(" ", "");
            if (keyPress.Length == 1)
            {
                alphabet.Add(merged[i]);
                merged.RemoveAt(i);
                i--;
            }
        }
        List<string> numbers = new List<string>();
        for (int i = 0; i < merged.Count; i++)
        {
            string keyPress = merged[i].Split('|')[0];
            if (keyPress.Contains("Alpha"))
            {
                numbers.Add(merged[i]);
                merged.RemoveAt(i);
                i--;
            }
        }
        List<string> keypad = new List<string>();
        for (int i = 0; i < merged.Count; i++)
        {
            string keyPress = merged[i].Split('|')[0];
            if (keyPress.Contains("Keypad"))
            {
                keypad.Add(merged[i]);
                merged.RemoveAt(i);
                i--;
            }
        }
        List<string> fs = new List<string>();
        for (int i = 0; i < merged.Count; i++)
        {
            string keyPress = merged[i].Split('|')[0];

            for (int e = 1; e < 20; e++)
                if (keyPress.Contains("F" + e))
                {
                    fs.Add(merged[i]);
                    merged.RemoveAt(i);
                    i--;
                    break;
                }
        }
        List<string> arrows = new List<string>();
        for (int i = 0; i < merged.Count; i++)
        {
            string keyPress = merged[i].Split('|')[0];
            if (keyPress.Contains("Arrow"))
            {
                arrows.Add(merged[i]);
                merged.RemoveAt(i);
                i--;
            }
        }
        List<string> mouse = new List<string>();
        for (int i = 0; i < merged.Count; i++)
        {
            string keyPress = merged[i].Split('|')[0];
            if (keyPress.Contains("Mouse"))
            {
                mouse.Add(merged[i]);
                merged.RemoveAt(i);
                i--;
            }
        }
        List<string> joystick = new List<string>();
        for (int i = 0; i < merged.Count; i++)
        {
            string keyPress = merged[i].Split('|')[0];
            if (keyPress.Contains("Joystick"))
            {
                joystick.Add(merged[i]);
                merged.RemoveAt(i);
                i--;
            }
        }
        List<string> main = new List<string>();
        for (int i = 0; i < merged.Count; i++)
        {
            string keyPress = merged[i].Split('|')[0];
            if (keyPress.Contains("Space") || keyPress.Contains("Alt") || keyPress.Contains("Shift") || keyPress.Contains("Control"))
            {
                main.Add(merged[i]);
                merged.RemoveAt(i);
                i--;
            }
        }
        
        alphabet.Sort();
        numbers.Sort();
        keypad.Sort();
        fs.Sort();
        arrows.Sort();
        mouse.Sort();
        joystick.Sort();
        main.Sort();
        merged.Sort();
        
        List<string> all = new List<string>();
        all.Add("🡇 Down   ★ Hold   🡅 Up   ⚋ Switch Case");
        all.Add("");
        AddIfAny(alphabet, all);
        AddIfAny(numbers, all);
        AddIfAny(keypad, all);
        AddIfAny(fs, all);
        AddIfAny(arrows, all);
        AddIfAny(main, all);
        AddIfAny(mouse, all);
        AddIfAny(joystick, all);
        AddIfAny(merged, all);
        all.Add("-----------------------------------------------");
        all.Add("Available:");
        all.Add("-----------------------------------------------");
        for (int i = 0; i < allKeyCodes.Count; i++)
            all.Add(allKeyCodes[i]);
        
        ProjectTxt.Write("04_TextFiles/UsedKeys", all.ToArray(), open: true);
    }


    private static bool AddIf(string line, string searchString, string symbol, string fileName, List<string> lines, List<string> allKeyCodes)
    {
        if (line.Contains(searchString))
        {
            string keyCode = line.Split(new[]{ searchString }, StringSplitOptions.None)[1].Split(')')[0].
                Replace("KeyCode.", "").Replace(":", "").Replace(" ", "").Replace("\t", "");
            
            allKeyCodes.Remove(keyCode);
            
            lines.Add(keyCode + " | " + symbol + " " + fileName);
            return true;
        }

        return false;
    }


    private static void Pad(List<string> lines)
    {
        int longest = 0;
        for (int i = 0; i < lines.Count; i++)
            if(lines[i].Contains("|"))
                longest = Mathf.Max(longest, lines[i].Split('|')[0].Length);

        for (int i = 0; i < lines.Count; i++)
            if(lines[i].Contains("|"))
            {
                string[] parts = lines[i].Split('|');
                lines[i] = parts[0].PadRight(longest) + "|" + parts[1];
            }
    }


    private static void AddIfAny(List<string> values, List<string> all)
    {
        if (values.Count > 0)
        {
            all.AddRange(values);
            all.Add("");
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class ThinkingPanel : EditorWindow 
{
    private static ThinkingPanel window;
    private static ThinkingList thoughts;
    private static DateTime last;
    private static int pick;
    private static Color[] colors;

    private const float HoldTime = .12f;
    private static float NextSwitchTime;
		
    [MenuItem("Window/ThinkingPanel")]
    private static void GetWindow()
    {
	    window = GetWindow(typeof(ThinkingPanel), false, "Thinking Panel") as ThinkingPanel;
	    window.maxSize = new Vector2(10000, 35);
	    window.minSize = new Vector2(10, 35);
	    last = DateTime.Now;
    }
    
    
	private void OnGUI()
	{
		if (thoughts == null)
			thoughts = Assets.FindAll<ThinkingList>()[0];

		if (colors == null || colors.Length < 6)
			colors = new[]
			{
				COLOR.yellow.fresh,
				COLOR.green.spring,
				COLOR.turquois.bright,
				COLOR.blue.cornflower,
				COLOR.purple.magenta,
				COLOR.red.tomato
			};
		
		GUILayout.BeginHorizontal();
		
		
		for (int i = 0; i < 6; i++)
		{
			ThinkingList.ThoughtProcess thoughtProcess = (ThinkingList.ThoughtProcess) i;
			List<string> buttonList = GetList(thoughtProcess);
			bool active = buttonList.Count > 0;
			GUI.color = active ? colors[(int) thoughtProcess] : Color.grey;
			if (GUILayout.Button(thoughtProcess + " (" + buttonList.Count + ")") && active)
			{
				thoughts.current = thoughtProcess;
				pick = Random.Range(0, buttonList.Count);
				last = DateTime.Now;
				NextSwitchTime = buttonList[0].Replace(" ", "").Length * HoldTime;
			}
		}
		
		List<string> lines = GetList(thoughts.current);
		
		if(lines.Count > 0)
		if ((DateTime.Now - last).Seconds > NextSwitchTime)
		{
			last = DateTime.Now;
			pick = (pick + 1) % lines.Count;
			NextSwitchTime  = lines[pick].Replace(" ", "").Length * HoldTime;
		}
		
		GUI.color = Color.white;
			
		GUILayout.EndHorizontal();
		
		
		GUI.color = colors[(int) thoughts.current];
		
		if(lines.Count > 0)
		if(GUILayout.Button(lines[pick] + " (" + (pick + 1) + ")"))
			Selection.activeObject = thoughts;
		
		GUI.color = Color.white;
	}


	private List<string> GetList(ThinkingList.ThoughtProcess thoughtProcess)
	{
		switch (thoughtProcess)
		{
			default:	                               return thoughts.todo;
			case ThinkingList.ThoughtProcess.Ideas:	   return thoughts.ideas;
			case ThinkingList.ThoughtProcess.Musings:  return thoughts.musings;
			case ThinkingList.ThoughtProcess.Warnings: return thoughts.warnings;
			case ThinkingList.ThoughtProcess.Bugs:	   return thoughts.bugs;
			case ThinkingList.ThoughtProcess.Build:	   return thoughts.build;
		}
	}


	private void OnInspectorUpdate()
	{
		if (window == null)
			GetWindow();
		
		if ((DateTime.Now - last).Seconds > NextSwitchTime)
			window.Repaint();
	}
}

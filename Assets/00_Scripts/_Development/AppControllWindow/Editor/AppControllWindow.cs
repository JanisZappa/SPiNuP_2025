using System;
using GameModeStuff;
using UnityEditor;
using UnityEngine;


public class AppControllWindow : EditorWindow 
{
    private static AppControllWindow window;
    
    [MenuItem("Window/AppControll")]
    private static void OpenWindow()
    {
        window = GetWindow(typeof(AppControllWindow), false, "AppControll") as AppControllWindow;
        window.minSize  = new Vector2(220, 25);
    }
    
    
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        
        string[] names = Enum.GetNames(typeof(Mode));
        int selection = (int) GameManager.SavedMode;
            
        for (int i = 1; i < names.Length; i++)
        {
            GUI.color = i == selection? 
                Color.Lerp(COLOR.yellow.fresh, COLOR.green.lime, (float)(i - 1) / (names.Length - 2)) : 
                Color.white;
            
            if (GUILayout.Button(names[i], GUILayout.Width(80), GUILayout.Height(20)))
                GameManager.SavedMode = (Mode) i;
        }
        
        GUILayout.FlexibleSpace();
        
        GUI.color = 0 == selection ? COLOR.red.tomato : Color.gray;
        if (GUILayout.Button(names[0]))
            GameManager.SavedMode = 0;
        
        GUI.color = Color.white;
        
        EditorGUILayout.EndHorizontal();
    }
}

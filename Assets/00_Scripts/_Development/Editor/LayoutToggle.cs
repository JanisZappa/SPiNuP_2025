using UnityEditor;
using UnityEngine;


public static class LayoutToggle 
{
    [MenuItem("Window/ToggleLayout &F1", false, 999)]
    private static void ToggleLayout()
    {
        int layoutNumber = PlayerPrefs.GetInt("Editor_LayoutNumber") == 0 ? 1 : 0;
        PlayerPrefs.SetInt("Editor_LayoutNumber", layoutNumber);

        EditorApplication.ExecuteMenuItem(layoutNumber == 0 ? "Window/Layouts/Log Switch Full" : "Window/Layouts/Nexus_Layout");
    }
}

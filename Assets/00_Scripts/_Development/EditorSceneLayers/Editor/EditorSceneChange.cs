using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


[InitializeOnLoad]
public static class EditorSceneChange 
{
    public static string currentScene;

    public delegate void SceneChange();

    public static event SceneChange onSceneChange;
    
    
    static EditorSceneChange()
    {
        currentScene = "";
        EditorApplication.hierarchyChanged += HierarchyWindowChanged;

        onSceneChange = null;
        onSceneChange += ZappaToolsSceneLayers.LatestScenes.SetVisibleLayers;
        onSceneChange += SceneLightLerp.SceneChangeSet;
        onSceneChange += Palette.EditorLoad;
    }
        
        
    private static void HierarchyWindowChanged()
    {
        if (currentScene == SceneManager.GetActiveScene().name || Application.isPlaying)
            return;
        
        
        currentScene = SceneManager.GetActiveScene().name;
        if (onSceneChange != null)
            onSceneChange();
    }
}

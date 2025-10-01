using UnityEditor;
using UnityEngine;


namespace ZappaToolsSceneLayers
{
    public static class LatestScenes
    {
        private static string assetPath
        {
            get { return "Assets/ZappaTools/EditorSceneLayers/Layers/" + EditorSceneChange.currentScene + ".asset"; }
        } 
        
        
        public static void SetVisibleLayers()
        {
            SceneLayers sceneLayers = AssetDatabase.LoadAssetAtPath(assetPath , typeof(Object)) as SceneLayers;
            if (sceneLayers != null)
                Tools.visibleLayers = sceneLayers.layers;
            else
                ShowAllLayers();
        }
        
        
        [MenuItem("Tools/SceneLayers/Save Scene Layers")]
        private static void SaveSceneLayers()
        {
            if (Tools.visibleLayers == -1)
            {
                Debug.Log("No special layers selected for this scene!");
                return;
            }
            
            SceneLayers newLayers = new SceneLayers {layers = Tools.visibleLayers};
    
            AssetDatabase.CreateAsset(newLayers, assetPath);
            AssetDatabase.SaveAssets();
        }
        

        [MenuItem("Tools/SceneLayers/Show Scene Layers &l")]
        private static void ShowSceneLayers()
        {
            SetVisibleLayers();
        }
        
        
        [MenuItem("Tools/SceneLayers/Show All #&l")]
        private static void ShowAllLayers()
        {
            Tools.visibleLayers = -1;
        }
        
        
        [MenuItem("Tools/SceneLayers/Show Save")]
        private static void GotoSave()
        {
            Object sceneLayerObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            if (sceneLayerObject != null)
                Selection.activeObject = sceneLayerObject;
            else
            {
                Debug.Log("There is no save for this scene!");
                Object folder = AssetDatabase.LoadAssetAtPath("Assets/ZappaTools/EditorSceneLayers/Layers", typeof(Object));
                if(folder != null)
                    Selection.activeObject = folder;
            }
        }
    }
}
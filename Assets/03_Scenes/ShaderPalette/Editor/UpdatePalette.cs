using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class UpdatePalette
{
    [MenuItem("Tools/Mesh/Log Vertice Colors %#F10", false, 101)]
    public static void CheckColor()
    {
        if (Selection.activeGameObject == null)
            return;

        MeshFilter          filter              = Selection.activeGameObject.GetComponent<MeshFilter>();
        SkinnedMeshRenderer skinnedMeshRenderer = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
        if (filter == null && skinnedMeshRenderer == null)
            return;
        
        Color32[] verticeColors = filter != null? filter.sharedMesh.colors32 : skinnedMeshRenderer.sharedMesh.colors32;
        List<Color32> colors = new List<Color32>();


        for (int i = 0; i < verticeColors.Length; i++)
            colors.AddUnique(verticeColors[i]);

        for (int i = 0; i < colors.Count; i++)
            Debug.Log(colors.Log());
    }


    [MenuItem("Tools/Mesh/Info")]
    private static void MeshInfo()
    {
        if (Selection.activeGameObject != null)
        {
            MeshFilter mF = Selection.activeGameObject.GetComponent<MeshFilter>();
            if (mF != null)
            {
                Mesh mesh = Application.isPlaying ? mF.mesh : mF.sharedMesh;
                
                Color color = mesh.colors[0];
                
                Debug.Log("R: " + Mathf.Floor(color.r * 255) + " float: " + color.r);
                Debug.Log("G: " + Mathf.Floor(color.g * 255) + " float: " + color.g);
                Debug.Log("B: " + Mathf.Floor(color.g * 255) + " float: " + color.b);
                
                if(mesh.uv.Length > 0)
                    Debug.Log("Mesh has UVs");
                
                if(mesh.tangents.Length > 0)
                    Debug.Log("Mesh has Tangents");
            }
        }
    }


    [MenuItem("Assets/Delete SubAsset")]
    private static void SubAsset()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Object.DestroyImmediate(Selection.activeObject, true);
        AssetDatabase.ImportAsset(path);
    }
    
    
    [MenuItem("Assets/Delete SubAsset", true)]
    private static bool NewMenuOptionValidation()
    {
        // This returns true when the selected object is a Variable (the menu item will be disabled otherwise).
        return Selection.activeObject is Mesh;
    }
}

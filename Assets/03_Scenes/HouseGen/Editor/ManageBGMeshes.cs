using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public static class ManageBGMeshes
{
    [MenuItem("BG_Meshes/Collect")]
    public static void Collect()
    {
        PlacerMeshes pM = Assets.Get<PlacerMeshes>("TestMeshes");

        if (!pM)
            return;

        List<Mesh> pieces = new(), shadows = new();
        Mesh[] pieceMeshes = Assets.FindAllMeshes("HoudiniMesh/_Backgrounds/Pieces", true);
        for (int i = 0; i < pieceMeshes.Length; i++)
        {
            Mesh m = pieceMeshes[i];
            if(m.name.Contains("_Shadow"))
                shadows.Add(m);
            else
                pieces.Add(m);
        }
        
        string[] mapLines = File.ReadAllLines("D:/map.txt");
        List<int> map = new List<int>();
        for (int i = 0; i < mapLines.Length; i++)
            map.Add(int.Parse(mapLines[i]));
        
        pM.SetupMeshes(pieces, shadows, map);
        
        EditorUtility.SetDirty(pM);
    }
    
    [MenuItem("BG_Meshes/Delete")]
    public static void DeleteAll()
    {
        {
            string dir = Application.dataPath + "/HoudiniMesh/_Backgrounds/Pieces/";
    
            string[] allFiles = Directory.GetFiles(dir);
            for (int i = 0; i < allFiles.Length; i++)
                File.Delete(allFiles[i]);
        }
        {
            string dir = Application.dataPath + "/HoudiniMesh/_Backgrounds/Shadows";
    
            string[] allFiles = Directory.GetFiles(dir);
            for (int i = 0; i < allFiles.Length; i++)
                File.Delete(allFiles[i]);
        }
        
        AssetDatabase.Refresh();
        
        PlacerMeshes pM = Assets.Get<PlacerMeshes>("TestMeshes");

        if (!pM)
            return;
        
        pM.pieceShadows  = Array.Empty<PlacerMeshes.PieceAndShadow>();
        EditorUtility.SetDirty(pM);
    }

    [MenuItem("BG_Meshes/Select")]
    public static void Select()
    {
        PlacerMeshes pM = Assets.Get<PlacerMeshes>("TestMeshes");
        if (pM)
            Selection.activeObject = pM;
    }
}

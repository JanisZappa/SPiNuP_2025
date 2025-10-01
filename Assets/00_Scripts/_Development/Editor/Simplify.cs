using UnityEditor;
using UnityEngine;


public class Simplify : MonoBehaviour
{
    [MenuItem("Setup/Simplify All MeshRenderers", false, 11)]
    private static void SimplifyMeshRenderers()
    {
        Object[] objects = FindObjectsOfType<MeshRenderer>();
        int length = objects.Length;
        Debug.Log("Simplifying " + length + " MeshRenderer");

        for(int i = 0; i < length; i++)
            (objects[i] as MeshRenderer).Simplify();
    }
    
    [MenuItem("Setup/Simplify Selected MeshRenderer %m", false, 12)]
    private static void SimplifyMeshRenderer()
    {
        MeshRenderer mR = Selection.activeGameObject != null ? Selection.activeGameObject.GetComponent<MeshRenderer>() : null;
        if (mR != null)
            mR.Simplify();
    }
}
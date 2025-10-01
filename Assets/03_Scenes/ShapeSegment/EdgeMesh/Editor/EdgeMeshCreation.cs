using UnityEditor;
using UnityEngine;


public static class EdgeMeshCreation 
{
	[MenuItem("Assets/CreateEdge", false, -1000)]
	private static void CreateEdge()
	{
		for (int i = 0; i < Selection.objects.Length; i++)
		{
			string fbxPath = AssetDatabase.GetAssetPath(Selection.objects[i]);

			Mesh mesh = null;
			Object[] assetObjects = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
			for (int e = 0; e < assetObjects.Length; e++)
				if (assetObjects[e] is Mesh)
				{
					mesh = (Mesh) assetObjects[e];
					break;
				}

			if (mesh == null)
				return;


			string name     = Selection.objects[i].name;
			string edgePath = fbxPath.Replace("Meshes/" + name + ".fbx", "Edges/" + name + ".asset");
			EdgeMesh edgeMesh = AssetDatabase.LoadAssetAtPath(edgePath, typeof(EdgeMesh)) as EdgeMesh;

			if (!edgeMesh)
			{
				edgeMesh = ScriptableObject.CreateInstance<EdgeMesh>();
				AssetDatabase.CreateAsset(edgeMesh, edgePath);
				Debug.LogFormat("Creating: \"{0}\"", edgePath);
			}


			edgeMesh.SetMesh(mesh);
			EditorUtility.SetDirty(edgeMesh);
			AssetDatabase.SaveAssets();
		}
	}
	
	
	[MenuItem("Assets/CreateEdge", true)]
	private static bool ValidateCreateEdge()
	{
		if (Selection.objects == null || Selection.objects.Length == 0)
			return false;

		for (int i = 0; i < Selection.objects.Length; i++)
			if (!AssetDatabase.GetAssetPath(Selection.objects[i]).Contains(".fbx"))
				return false;
		
		return true;
	}
}

using UnityEditor;
using UnityEngine;


public class MergeMesh : MonoBehaviour 
{
	[MenuItem("Tools/Merge Level Mesh %&m", false, 100)]
	private static void MergeLevelMesh()
	{
		if(!Application.isPlaying)
			return;
		
		Transform level = GameObject.Find("Level").transform;

		for (int c = 0; c < level.childCount; c++)
		{
			Transform category = level.GetChild(c);

			for (int g = 0; g < category.childCount; g++)
			{
				GameObject group = category.GetChild(g).gameObject;
				MeshFilter[] meshFilters = group.GetComponentsInChildren<MeshFilter>();

				if (meshFilters.Length == 0)
					continue;

				Mesh mesh = MeshExt.GetMergedMesh(meshFilters, level);

				string path = "Assets/MergeMeshes/" + group.name + ".asset";
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.ImportAsset(path);
			}
		}
		
		//ActorAnimator.HideAllBecauseWeMergedMeshes();
	}
}

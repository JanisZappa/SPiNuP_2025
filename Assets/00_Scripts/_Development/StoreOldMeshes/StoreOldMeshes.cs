public static class StoreOldMeshes 
{
	/*[MenuItem("Tools/Store Old Meshes")]
	public static void FindAndStore()
	{
		if (Selection.activeObject == null || Selection.activeObject as OldMeshes == null)
			return;
		
		List<SplitGMesh> splitMeshes = new List<SplitGMesh>();
		string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(SplitGMesh)));
		for( int i = 0; i < guids.Length; i++ )
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
			SplitGMesh asset = AssetDatabase.LoadAssetAtPath<SplitGMesh>( assetPath );
			if( asset != null )
				splitMeshes.Add(asset);
		}
        
		List<Mesh> meshes = new List<Mesh>();
		for (int i = 0; i < splitMeshes.Count; i++)
		{
			Object[] assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(splitMeshes[i]));
			for (int e = 0; e < assets.Length; e++)
				if (assets[e] is Mesh)
				{
					Mesh copy = Object.Instantiate((Mesh) assets[e]);
					copy.name = splitMeshes[i].name + "_#_" +assets[e].name;
					meshes.Add(copy);
				}   
		}
		
		OldMeshes oldMeshes = (OldMeshes) Selection.activeObject;
	
		for (int i = 0; i < meshes.Count; i++)
			AssetDatabase.AddObjectToAsset(meshes[i], oldMeshes);
        
		EditorUtility.SetDirty(oldMeshes);
		AssetDatabase.Refresh();
	}*/
}
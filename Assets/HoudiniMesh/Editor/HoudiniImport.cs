using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


public class HoudiniImport : AssetPostprocessor
{
	private const string BodyPartImportFolder  = "_Character";
	private const string BodyPartFolder = "05_BodyParts/";
	private const string MetaData = "Assets/HoudiniMesh/_MetaData/";
	
	
    private void OnPreprocessModel()
    {
	    ModelImporter thisModelImporter = assetImporter as ModelImporter;
	    if (!thisModelImporter.assetPath.Contains("HoudiniMesh"))
		    return;

	    
	    thisModelImporter.globalScale          = 1;
	    thisModelImporter.isReadable = true;//thisModelImporter.assetPath.Contains("..") || thisModelImporter.assetPath.Contains(BodyPartImportFolder) || thisModelImporter.assetPath.Contains("WallOcclusion");
	    thisModelImporter.animationType        = ModelImporterAnimationType.None;
	    thisModelImporter.importTangents       = ModelImporterTangents.None;
	    thisModelImporter.optimizeMeshPolygons = true;
	    thisModelImporter.optimizeMeshVertices = true;
	    
	    thisModelImporter.materialImportMode   = ModelImporterMaterialImportMode.None;
	    thisModelImporter.importLights         = false;
	    thisModelImporter.importCameras        = false;
	    thisModelImporter.importBlendShapes    = false;
	    thisModelImporter.importVisibility     = false;
	    thisModelImporter.weldVertices         = false;
    }
    
    
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
    {
	    for (int i = 0; i < importedAssets.Length; i++)
	    {
		    string path = importedAssets[i];
			
		    if(path.Contains(BodyPartImportFolder) && path.Contains(".fbx"))
			    ParseBodyPart(path);
	    }
    }
    
    
    public static void ParseBodyPart(string fbxPath)
    {
	    GameObject fbx = AssetDatabase.LoadAssetAtPath(fbxPath, typeof(GameObject)) as GameObject;
				
	    string       savePath = "Assets/" + BodyPartFolder + fbx.name + ".asset";
	    BodyPartMesh bodyPart = AssetDatabase.LoadAssetAtPath(savePath, typeof(BodyPartMesh)) as BodyPartMesh;
					
	    if (bodyPart != null)
		    Debug.LogFormat("Updating: \"{0}\"", savePath);
	    else
	    {
		    bodyPart = ScriptableObject.CreateInstance<BodyPartMesh>();
		    AssetDatabase.CreateAsset(bodyPart, savePath);
		    Debug.LogFormat("Creating: \"{0}\"", savePath);
	    }

	    string jsonPath = MetaData + fbx.name + ".json";
	    if (!File.Exists(jsonPath))
		    return;

	//  Get MetaData  //
		JSONNode node = JSONNode.Parse(File.ReadAllText(jsonPath))["meta"];
		List<Locator> locators = new List<Locator>();

		foreach (JSONNode item in node.Children)
		{
			JSONObject jO = item as JSONObject;

			foreach (KeyValuePair<string, JSONNode> N in jO)
			{
				JSONNode valueNode = N.Value;
				
				if (valueNode.Count == 3)
				{
					Vector3 p = new Vector3((float)double.Parse(valueNode[0]), 
						                    (float)double.Parse(valueNode[1]), 
						                    (float)double.Parse(valueNode[2]));
					
					locators.Add(new Locator(N.Key, p));
				}
			}
		}
		
		
		bodyPart.partType = (PartType)Enum.Parse(typeof(PartType), fbx.name.Split('_')[0]);
		bodyPart.locators = locators.ToArray();
	
		MeshFilter meshFilter = fbx.transform.GetComponent<MeshFilter>();
					
	//  Create or Overwrite existing Copy of Mesh  //
		Mesh copyThis = meshFilter != null ? meshFilter.sharedMesh : fbx.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
		Mesh meshCopy = Object.Instantiate(copyThis);
		meshCopy.name = copyThis.name;
					
		Object[] objects = AssetDatabase.LoadAllAssetsAtPath(savePath);
		for (int m = 0; m < objects.Length; m++)
			if (objects[m] is Mesh)
				Object.DestroyImmediate(objects[m], true);
					
		AssetDatabase.AddObjectToAsset(meshCopy, bodyPart);
					
					
		EditorUtility.SetDirty(bodyPart);
		AssetDatabase.ImportAsset(savePath);
					
		objects = AssetDatabase.LoadAllAssetsAtPath(savePath);
		for (int m = 0; m < objects.Length; m++)
			if (objects[m] is Mesh)
			{
				bodyPart.mesh = (Mesh) objects[m];
				break;
			}
							
		EditorUtility.SetDirty(bodyPart);
    }


    private void OnPreprocessAsset()
    {
	    if (assetImporter.assetPath.Contains("HoudiniUsed.txt"))
	    {
		    List<Color> newColors = new List<Color>();
		    string[] lines = File.ReadAllLines(assetImporter.assetPath);

		    for (int i = 0; i < lines.Length; i++)
		    {
			    string[] parts = lines[i].Split(',');
			    newColors.Add(new Color32(byte.Parse(parts[0].Replace("[","")), 
				                          byte.Parse(parts[1]), 
				                          byte.Parse(parts[2].Replace("]","")), 
				                          255));
		    }

		    PaletteSource.Get.houdini = newColors.ToArray();
		    EditorUtility.SetDirty(PaletteSource.Get);
	    }
    }
}

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu]
public class LevelList : ScriptableObject 
{
	public List<LevelSaveFile> levels;

	public int FirstFreeID
	{
		get
		{
			int ID = 0;

			while (true)
			{
				bool alreadyGiven = false;
				for (int i = 0; i < levels.Count; i++)
					if (levels[i] != null && levels[i].levelID == ID)
					{
						alreadyGiven = true;
						break;
					}

				if (!alreadyGiven)
					break;

				ID++;
			}

			return ID;
		}
	}
	
	


	public bool IsNewLevelSave(string levelName)
	{
		for (int i = 0; i < levels.Count; i++)
			if (levels[i] != null && levels[i].name == levelName)
				return false;

		return true;
	}


	public void SaveLevel(string levelName, byte[] bytes)
	{
		#if UNITY_EDITOR
		string path = "Assets/Resources/SavedLevels/" + levelName + ".asset";
		
		LevelSaveFile saveFile = AssetDatabase.LoadAssetAtPath<LevelSaveFile>(path);
		if (saveFile != null)
		{
			saveFile.bytes   = bytes;
			saveFile.levelID = FirstFreeID;
			EditorUtility.SetDirty(saveFile);
			AssetDatabase.SaveAssets();
			return;
		}
		
		
		saveFile = CreateInstance<LevelSaveFile>();
		saveFile.bytes   = bytes;
		saveFile.levelID = FirstFreeID;

		
		AssetDatabase.CreateAsset(saveFile, path);

		bool usedEmptySlot = false;
		for (int i = 0; i < levels.Count; i++)
			if (levels[i] == null)
			{
				levels[i] = saveFile;
				usedEmptySlot = true;
			}
		
		if(!usedEmptySlot)
			levels.Add(saveFile);
		
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
		#endif
	}

	public LevelSaveFile GetLevel(string levelName)
	{
		for (int i = 0; i < levels.Count; i++)
			if (levels[i] != null && levels[i].name == levelName)
				return levels[i];
		
		return levels[0];
	}
	
	
	public LevelSaveFile GetOtherLevel(string levelName)
	{
		List<LevelSaveFile> otherList = new List<LevelSaveFile>();
		
		for (int i = 0; i < levels.Count; i++)
			if (levels[i] != null && levels[i].name != levelName)
				otherList.Add(levels[i]);
		
		return otherList.Count > 0? otherList[Random.Range(0, otherList.Count)] 
			                      : GetLevel(levelName);
	}
	
	
	public LevelSaveFile GetFirstLevel()
	{
		for (int i = 0; i < levels.Count; i++)
			if (levels[i] != null)
				return levels[i];
		
		return null;
	}
}

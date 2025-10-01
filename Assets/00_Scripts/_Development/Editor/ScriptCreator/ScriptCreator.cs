using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


public static class ScriptCreator
{
	[MenuItem("Edit/Create C# %&C")]
	public static void Create()
	{
		GameObject attachToThis = Selection.activeGameObject;
		
		string folderPath = ProjectViewPath;
		string savePath = EditorUtility.SaveFilePanel("Create C# Script", folderPath, "", "");
		string[] parts = savePath.Split('/');
		Info.typeName = parts[parts.Length - 1];
		
		if(string.IsNullOrEmpty(Info.typeName))
			return;
		
		Info.path = savePath + ".cs";
		
		ScriptCreatorWindow.Open();
		
		if (Info.IsMonoBehaviour && attachToThis != null)
		{
			GameObjectID = attachToThis.GetInstanceID().ToString();
			TypeName     = Info.typeName;
		}
		else
		{
			GameObjectID = null;
			TypeName     = null;
		}
	}


	public static void WriteScript()
	{
		using (StreamWriter outfile =
			new StreamWriter(Info.path))
		{
			bool usedSomething = false;
			if (Info.usingUnityEngine)
			{
				outfile.WriteLine("using UnityEngine;");
				usedSomething = true;
			}
			if (Info.usingUnityEditor)
			{
				if (Info.IsMonoBehaviour)
				{
					outfile.WriteLine("#if UNITY_EDITOR");
					outfile.WriteLine("using UnityEditor;");
					outfile.WriteLine("#endif");
				}
				else
					outfile.WriteLine("using UnityEditor;");
				usedSomething = true;
			}
			if (Info.usingSystemCollections)
			{
				outfile.WriteLine("using System.Collections;");
				usedSomething = true;
			}
				
			if (Info.usingSystemCollectionsGeneric)
			{
				outfile.WriteLine("using System.Collections.Generic;");
				usedSomething = true;
			}
			if(usedSomething)
				outfile.Space(2);
			
			if (Info.IsMonoBehaviour)
				outfile.WriteLine("public class " + Info.typeName + " : MonoBehaviour");
			else
				outfile.WriteLine("public static class " + Info.typeName);
			
			outfile.WriteLine("{");

			bool first = true;

			if (Info.IsMonoBehaviour)
			{
				if (Info.Awake)
				{
					outfile.WriteLine(T(1) + "private void Awake()");
					outfile.WriteLine(T(1) + "{");
					outfile.WriteLine(T(1));
					outfile.WriteLine(T(1) + "}");
					first = false;
				}
				if (Info.OnEnable)
				{
					if(!first)	outfile.Space(2);
					outfile.WriteLine(T(1) + "private void OnEnable()");
					outfile.WriteLine(T(1) + "{");
					outfile.WriteLine(T(1));
					outfile.WriteLine(T(1) + "}");
					first = false;
				}
				if (Info.Start)
				{
					if(!first)	outfile.Space(2);
					outfile.WriteLine(T(1) + "private void Start()");
					outfile.WriteLine(T(1) + "{");
					outfile.WriteLine(T(1));
					outfile.WriteLine(T(1) + "}");
					first = false;
				}
				if (Info.Update)
				{
					if(!first)	outfile.Space(2);
					outfile.WriteLine(T(1) + "private void Update()");
					outfile.WriteLine(T(1) + "{");
					outfile.WriteLine(T(1));
					outfile.WriteLine(T(1) + "}");
					first = false;
				}
				if (Info.LateUpdate)
				{
					if(!first)	outfile.Space(2);
					outfile.WriteLine(T(1) + "private void LateUpdate()");
					outfile.WriteLine(T(1) + "{");
					outfile.WriteLine(T(1));
					outfile.WriteLine(T(1) + "}");
					first = false;
				}
			}
			
			outfile.WriteLine("}");
		}
		
		AssetDatabase.Refresh();
		System.Diagnostics.Process.Start(Info.path);
	}
	

	private static string GameObjectID
	{
		get { return PlayerPrefs.GetString("ScriptCreator_GOID"); }
		set { PlayerPrefs.SetString("ScriptCreator_GOID", value); }
	}
	private static string TypeName 
	{
		get { return PlayerPrefs.GetString("ScriptCreator_TypeName"); }
		set { PlayerPrefs.SetString("ScriptCreator_TypeName", value); }
	}


	private static Type GetType(string name)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (var i = 0; i < assemblies.Length; i++)
		{
			Type t = assemblies[i].GetType(name);
			if (t != null)
				return t;
		}

		return null;
	}


	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnScriptsReloaded() 
	{
		string instanceID = GameObjectID;
		if (!string.IsNullOrEmpty(instanceID))
		{
			Object obj = EditorUtility.InstanceIDToObject(int.Parse(instanceID));
			GameObject gO = obj as GameObject;
			if (gO != null)
			{
				string typeName = TypeName;
				if (!string.IsNullOrEmpty(typeName))
				{
					if (gO.name.Contains("GameObject"))
						gO.name = typeName;
					
					Component c = gO.AddComponent(GetType(typeName));
					if(c != null)
					{
						GameObjectID = null;
						TypeName     = null;
					}
				}
			}
		}
	}

	
	public static string ProjectViewPath
	{
		get
		{
			string path = "Assets";
	
			foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
			{
				path = AssetDatabase.GetAssetPath(obj);
				if ( !string.IsNullOrEmpty(path) && File.Exists(path) ) 
				{
					path = Path.GetDirectoryName(path);
					break;
				}
			}
			return path;
		}
	}
	
	
	private static string T(int tabs)
	{
		const string gap  = "    ";
		string tabSpace = "";
		for (int i = 0; i < tabs; i++)
			tabSpace += gap;

		return tabSpace;
	}


	public static class Info
	{
		public static string typeName, path;

		public static readonly prefBool 
			IsMonoBehaviour               = new prefBool("ScriptCreator_IsMonoBehaviour"),
			usingUnityEngine              = new prefBool("ScriptCreator_usingUnityEngine"),
			usingUnityEditor              = new prefBool("ScriptCreator_usingUnityEditor"),
			usingSystemCollections        = new prefBool("ScriptCreator_usingSystemCollections"),
			usingSystemCollectionsGeneric = new prefBool("ScriptCreator_usingSystemCollectionsGeneric"),
			Awake                         = new prefBool("ScriptCreator_Awake"),
			OnEnable                      = new prefBool("ScriptCreator_OnEnable"),
			Start                         = new prefBool("ScriptCreator_Start"),
			Update                        = new prefBool("ScriptCreator_Update"),
			LateUpdate                    = new prefBool("ScriptCreator_LateUpdate");
	}
}

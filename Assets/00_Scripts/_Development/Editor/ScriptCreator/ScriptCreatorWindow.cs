using UnityEditor;
using UnityEngine;


public class ScriptCreatorWindow : EditorWindow
{
	private static ScriptCreatorWindow window;
		
		
	public static void Open()
	{
		window = GetWindow(typeof(ScriptCreatorWindow), false, "Create Script") as ScriptCreatorWindow;
		window.minSize  = new Vector2(360, 300);
	}
    
    
	private void OnGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("\"" + ScriptCreator.Info.path + "\"");
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Change \"" + ScriptCreator.Info.typeName + "\""))
		{
			string newPath = EditorUtility.SaveFilePanel("C# Script", ScriptCreator.ProjectViewPath, "", "");
			string[] parts = newPath.Split('/');
			ScriptCreator.Info.typeName = parts[parts.Length - 1];

			if (string.IsNullOrEmpty(ScriptCreator.Info.typeName))
				window.Close();
			
			ScriptCreator.Info.path = newPath + ".cs";
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.Space(10);
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Usings");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		ScriptCreator.Info.usingUnityEngine.Set(GUILayout.Toggle(ScriptCreator.Info.usingUnityEngine,              "UnityEngine"));
		ScriptCreator.Info.usingUnityEditor.Set(GUILayout.Toggle(ScriptCreator.Info.usingUnityEditor,              "UnityEditor"));
		ScriptCreator.Info.usingSystemCollections.Set(GUILayout.Toggle(ScriptCreator.Info.usingSystemCollections,        "System.Collections"));
		ScriptCreator.Info.usingSystemCollectionsGeneric.Set(GUILayout.Toggle(ScriptCreator.Info.usingSystemCollectionsGeneric, "System.Collections.Generic"));
		
		GUILayout.Space(10);
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(ScriptCreator.Info.IsMonoBehaviour ? "MonoBehaviour" : "Static Class"))
			ScriptCreator.Info.IsMonoBehaviour.Toggle();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		if (ScriptCreator.Info.IsMonoBehaviour)
		{
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Methods");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		
			ScriptCreator.Info.Awake.Set(GUILayout.Toggle(ScriptCreator.Info.Awake, "Awake"));
			ScriptCreator.Info.OnEnable.Set(GUILayout.Toggle(ScriptCreator.Info.OnEnable, "OnEnable"));
			ScriptCreator.Info.Start.Set(GUILayout.Toggle(ScriptCreator.Info.Start, "Start"));
			ScriptCreator.Info.Update.Set(GUILayout.Toggle(ScriptCreator.Info.Update, "Update"));
			ScriptCreator.Info.LateUpdate.Set(GUILayout.Toggle(ScriptCreator.Info.LateUpdate, "LateUpdate"));
		}
		
		GUILayout.Space(10);
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Create"))
		{
			ScriptCreator.WriteScript();
			window.Close();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}

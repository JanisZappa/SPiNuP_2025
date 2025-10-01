using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(FovAdjust))]
public class FovAdjustEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		FovAdjust myTarget = (FovAdjust)target;
		
		GUILayout.BeginHorizontal();
		
		EditorGUILayout.HelpBox(
			"Orthographic: " + (myTarget.orthographic? "True" : "False") + "\r\n" +
			"DefaultAspect: " + myTarget.defaultAspect.ToString("F2") + "\r\n" +
			"DefaultFov: " + myTarget.defaultFov.ToString("F2") + "\r\n" +
			"DefaultSize: " + myTarget.defaultSize.ToString("F2"),
			MessageType.Info
		);
		
		
		GUILayout.BeginVertical();
		GUI.color = COLOR.yellow.fresh;
		if (GUILayout.Button("Set Default"))
		{
			myTarget.SetDefaultValues();
			EditorUtility.SetDirty(myTarget);
		}
		GUI.color = Color.white;
		
	
		
		if (GUILayout.Button("Test Adjustment"))
		{
			myTarget.AdjustCam();
		}
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
	}
}
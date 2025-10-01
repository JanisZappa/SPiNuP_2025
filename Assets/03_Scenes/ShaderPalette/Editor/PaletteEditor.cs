using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Palette))]
public class PaletteEditor : Editor {

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
        
		GUILayout.Space(20);
		
		GUI.color = COLOR.green.lime;
		if (GUILayout.Button("Load"))
			Palette.Load();
	}
}

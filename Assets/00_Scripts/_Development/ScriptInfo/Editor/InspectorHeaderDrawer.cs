using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(InspectorHeader))]
public class InspectorHeaderDrawer : PropertyDrawer 
{
	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(rect, label, property);
		GUI.Label(rect, property.FindPropertyRelative("info").stringValue);
		EditorGUI.EndProperty();
	}
}

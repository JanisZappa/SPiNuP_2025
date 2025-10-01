using UnityEditor;
using UnityEditor.UI;


[CanEditMultipleObjects, CustomEditor(typeof(NonDrawingGraphic), false)]
public class NonDrawingGraphicEditor : GraphicEditor
{
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(m_Script);
		RaycastControlsGUI();
		serializedObject.ApplyModifiedProperties();
	}
}
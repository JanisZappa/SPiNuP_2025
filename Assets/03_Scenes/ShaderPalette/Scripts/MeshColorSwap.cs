using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshColorSwap : MonoBehaviour 
{
	public Mesh[] meshes;
	[HideInInspector] public int[] colorIDs;
	
	[Space(10)]
	public MeshFilter mF;

	private int pick;
	

	private void OnEnable()
	{
		SetColor();
	}
	
	
	public void onSwitchActorColor()
	{
		pick = (pick + 1) % meshes.Length;
		
		SetColor();
	}
	
	
	private void SetColor()
	{
		if (!gameObject.activeInHierarchy)
			return;

		mF.mesh = meshes[pick];
	}


	public void CollectBounceColors()
	{
		colorIDs = new int[meshes.Length];
		for (int i = 0; i < meshes.Length; i++)
			colorIDs[i] = Mathf.FloorToInt(meshes[i].colors[0].r * 255);
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(MeshColorSwap))]
public class MeshColorSwapEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		GUILayout.Space(10);

		if (GUILayout.Button("CheckColors"))
			(target as MeshColorSwap).CollectBounceColors();
	}
}
#endif
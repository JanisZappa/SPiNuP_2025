#if UNITY_EDITOR

using System.Collections;
using UnityEditor;
using UnityEngine;

public class EditorCamControll : Singleton<EditorCamControll>
{
	private bool follow
	{
		get { return PlayerPrefs.GetInt("EditorCamControll") == 1; }
		set { PlayerPrefs.SetInt("EditorCamControll", value? 1 : 0); }
	}
	
	private static EditorWindow gameView;

	
	private void Start()
	{
		gameView = EditorWindow.focusedWindow;
	}
	
	
	private void Update () 
	{
		if (Input.GetKeyDown(KeyCode.LeftBracket))
			follow = !follow;
			
		if(!follow || EditorWindow.focusedWindow != gameView)
			return;
		
		UpdateEditorCamPos();
	}


	private void UpdateEditorCamPos()
	{
		if(SceneView.lastActiveSceneView != null && GameCam.Cam != null)
		{
			Transform cam = GameCam.Cam.transform;
			
			Ray ray = new Ray(cam.position, cam.forward);
			
			float dist;
			new Plane(Vector3.forward, Vector3.zero).Raycast(ray, out dist);
			
			Vector3 hitPoint = ray.origin + ray.direction * dist;
			
			SceneView.lastActiveSceneView.orthographic = true;
			SceneView.lastActiveSceneView.LookAtDirect(hitPoint, cam.rotation);
		}
	}


	public static void SetEditorFocus()
	{
		Inst.StartCoroutine(Inst.UpdateAnyway());
	}


	private IEnumerator UpdateAnyway()
	{
		while (EditorWindow.focusedWindow != gameView)
		{
			yield return null;
			UpdateEditorCamPos();
		}	
	}
}

#endif

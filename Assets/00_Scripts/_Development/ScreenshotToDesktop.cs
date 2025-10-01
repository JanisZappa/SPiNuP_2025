using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ScreenshotToDesktop : MonoBehaviour 
{
	private void Update () 
	{
		if (Input.GetKeyDown(KeyCode.F12))
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("\\", "/") + "/" +
			              SceneManager.GetActiveScene().name + "_" + 
			              DateTime.Now.ToShortDateString().Replace("/", ".") + 
			              "." + DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second + 
			              ".png";

			ScreenCapture.CaptureScreenshot(path);
		}
					
	}
	
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Register()
	{
		if (!Application.isMobilePlatform)
			SceneManager.sceneLoaded += PutInScene;
	}
    
	
	private static void PutInScene(Scene scene, LoadSceneMode mode)
	{
		new GameObject().AddComponent<ScreenshotToDesktop>();
	}
}

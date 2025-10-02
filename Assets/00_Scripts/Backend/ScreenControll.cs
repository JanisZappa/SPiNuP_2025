using System;
using System.Collections.Generic;
using UnityEngine;


public class ScreenControll : MonoBehaviour 
{
	private static DeviceOrientation orientation = DeviceOrientation.Portrait;
	private static readonly Dictionary<DeviceOrientation, ScreenOrientation> getScreenOrientation = new Dictionary<DeviceOrientation, ScreenOrientation>
	{
		{ DeviceOrientation.Portrait,       ScreenOrientation.Portrait},
		{ DeviceOrientation.LandscapeLeft,  ScreenOrientation.LandscapeLeft},
		{ DeviceOrientation.LandscapeRight, ScreenOrientation.LandscapeRight},
	};


	public static float Width, Height, Aspect;
	public static bool Landscape;
    
	public delegate void OrientationChange();
	public static event  OrientationChange onOrientationChange;
    
	private float checkWidth, checkHeight;
	
	public static float PixelPerMilimeter => Screen.dpi * .048f;


	private void Start()
	{
		Debug.Log("Screencontroll Start");
		Screen.autorotateToLandscapeLeft      = false;
		Screen.autorotateToLandscapeRight     = false;
		Screen.autorotateToPortrait           = false;
		Screen.autorotateToPortraitUpsideDown = false;
		
	#if UNITY_IOS
		Application.targetFrameRate = 60;
	#endif
		
		CamUpdate();
		SetNewOrientation();
		CheckOrientation();
	}

	
	private void Update()
	{
		CamUpdate();
		
		if (GTime.Paused && Input.deviceOrientation != orientation)
			SetNewOrientation();
		
		CheckOrientation();
	}


	private static void CamUpdate()
	{
		Debug.Log("Screencontroll CamUpdate");
		
		Camera cam = Camera.main;
		
		Width     = cam ? cam.pixelWidth  : Screen.width; 
		Height    = cam ? cam.pixelHeight : Screen.height;
		
#if UNITY_IOS
		Width     = Screen.width; 
		Height    = Screen.height;
#endif
		
		Aspect    = Width / Height;
		Landscape = Width >= Height;
		
		Debug.Log(".. " + Aspect);
	}
	
	
	
	private static void SetNewOrientation()
	{
		switch (Input.deviceOrientation)
		{
			case DeviceOrientation.Portrait:
			case DeviceOrientation.LandscapeLeft:
			case DeviceOrientation.LandscapeRight:
				orientation        = Input.deviceOrientation;
				Screen.orientation = getScreenOrientation[orientation];
				break;
		}
	}
	
	
	private void CheckOrientation()
	{
		if (f.Same(checkWidth, Width) && f.Same(checkHeight, Height))
			return;
        
		checkWidth  = Width;
		checkHeight = Height;
		
		onOrientationChange?.Invoke();
	}

	private void OnDisable()
	{
		Debug.Log("WTF");
	}
}

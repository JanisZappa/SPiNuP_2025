using GameModeStuff;
using TMPro;
using UnityEngine;


public class FrameCounter : MonoBehaviour
{
	public TextMeshProUGUI textA, textB;
	private float delta;

	private const float step = .2f;
	private float checkTime;
	private int frames, oldFps = -1, refreshRate, oldActualFps = -1;
	
	
	private string[] prepStrings;
	
	
	private void Awake()
	{
		checkTime   = Time.realtimeSinceStartup;

		refreshRate = 0;
		for (int i = 0; i < Screen.resolutions.Length; i++)
			refreshRate = Mathf.Max(refreshRate, Screen.resolutions[i].refreshRate);
		
		if(refreshRate == 0)
			refreshRate = 60;

		//Debug.Log("RefreshRate " + refreshRate);

		int formatting = refreshRate > 99 ? 3 : 2;
		
		prepStrings = new string[refreshRate + 1];
		for (int i = 0; i < prepStrings.Length; i++)
			prepStrings[i] = i.ToString().PadLeft(formatting);
	}

	
	private void Update ()
	{
		if(GameManager.IsCreator)
			gameObject.SetActive(false);
		
		
		float time = Time.realtimeSinceStartup;

		if (time - checkTime >= step)
		{
			delta /= frames;

			int fps = GetFPS(delta);
			
			if (oldFps != fps)
			{
				oldFps     = fps;
				textA.text = prepStrings[fps];
			}

			frames = 0;
			checkTime = time;
			delta = 0;
		}
		else
		{
			delta += Time.deltaTime;
			frames++;
		}
		
		int actualFps = GetFPS(Time.deltaTime);
		
		if (actualFps != oldActualFps)
		{
			oldActualFps = actualFps;
			textB.text = prepStrings[actualFps];
		}
	}


	private int GetFPS(float deltaTime)
	{
		int max   = refreshRate / QualitySettings.vSyncCount;
		int value = (int)Mathf.Max(0, Mathf.Clamp(Mathf.Ceil(1f / deltaTime), 0, max));
		
		if (value < 0)
			value = 0;

		return value;
	}
}

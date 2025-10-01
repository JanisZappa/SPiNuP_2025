using System;
using UnityEngine;
using UnityEngine.UI;


public class GarbageRect : MonoBehaviour
{
	private Image image;
	private Color color;
	
	private bool  showRequest, showing;
	private float showTime;
	
	
	private void Awake()
	{
		image = GetComponent<Image>();
		color = image.color;
		
		new SpawnMe(this);
		
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}


	private class SpawnMe
	{
		private readonly GarbageRect gE;

		public SpawnMe(GarbageRect gE)
		{
			this.gE = gE;
		}
		~SpawnMe()
		{
			GC.ReRegisterForFinalize(this);
			gE.showRequest = true;
			gE.showing     = false;
		}
	}


	private void Update()
	{
		if (showRequest)
		{
			if (!showing)
				image.color = color;
			
			showing     = true;
			showTime    = Time.realtimeSinceStartup;
			showRequest = false;
		}

		if (showing && Time.realtimeSinceStartup - showTime > 1f)
		{
			showing = false;
			image.color = new Color(0, 0, 0, 0);
		}
	}
}



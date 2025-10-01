using System;
using UnityEngine;
using UnityEngine.UI;


public class QualitySwitches : ActiveUI
{
	public enum Method { Quality, SkinnedMeshes, Backgrounds }
	
	[Serializable]
	public class Button
	{
		public  GameObject gameObject;
		public  bool       value;
		public  Method     method;
		private Image      image;
		private Color      color;
		private GameObject bttn;

		
		public void Setup()
		{
			bttn  = gameObject.transform.GetChild(0).gameObject;
			image = gameObject.GetComponent<Image>();
			color = image.color;
			image.color = Color.Lerp(Color.black, color, value ? 1 : .5f);
		}


		public bool Press(int click)
		{
			if (UI_Manager.ImPointedAt(bttn))
			{
				if (click == 1)
				{
					value = !value;
					image.color = Color.Lerp(Color.black, color, value ? 1 : .5f);
				
					switch (method)
					{
						case Method.Quality:
							QualitySettings.vSyncCount = value ? 1 : 2;
							break;
						
						case Method.SkinnedMeshes:
							Spinner.ToggleSkins(value);
							break;
						
						case Method.Backgrounds:
							EdgeMaster.SetActive(value);
							break;
					}
				}
				
				return true;
			}
			
			return false;
		}
	}

	
	public Button[] buttons;


	private void Awake()
	{
		for (int i = 0; i < buttons.Length; i++)
			buttons[i].Setup();
	}
	
	
	public override bool HitUI(int click)
	{
		for (int i = 0; i < buttons.Length; i++)
			if (buttons[i].Press(click))
				return true;

		return false;
	}
}

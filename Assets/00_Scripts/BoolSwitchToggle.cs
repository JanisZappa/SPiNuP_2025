using UnityEngine;
using UnityEngine.UI;


public class BoolSwitchToggle : ActiveUI
{
	public string valueName;
	public bool onMobile;
	private Image image;
	private Color color;

	private GameObject child;


	private void Awake()
	{
		      image = GetComponent<Image>();
		      color = image.color;
		image.color = Color.Lerp(Color.black, color, BoolSwitch.GetValue(valueName) ? 1 : .5f);
		child = transform.GetChild(0).gameObject;

		bool v = DebugDrawUpdate.DrawAnything;
	}


	public override void OnEnable()
	{
		if (Application.isMobilePlatform && !onMobile)
		{
			gameObject.SetActive(false);
			return;
		}
		
		base.OnEnable();
	}

	
	public override bool HitUI(int click)
	{
		if (UI_Manager.ImPointedAt(child))
		{
			switch (click)
			{
				case 1:
					BoolSwitch.ToggleValue(valueName);
					image.color = Color.Lerp(Color.black, color, BoolSwitch.GetValue(valueName) ? 1 : .5f);
					break;
				case 2:
					Debug.Log(valueName);
					break;
			}

			return true;
		}

		return false;
	}
}

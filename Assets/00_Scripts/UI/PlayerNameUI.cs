using Clips;
using TMPro;
using UnityEngine;


public class PlayerNameUI : MonoBehaviour
{
	private static readonly BoolSwitch showLines = new("UI/Show Names", true);
	
	private TextMeshProUGUI[] texts;
	private bool showingNames;
	private RectTransform rect;


	private void Awake()
	{
		rect = GetComponent<RectTransform>();
		
		texts = new TextMeshProUGUI[transform.childCount];
		for (int i = 0; i < texts.Length; i++)
		{
			texts[i] = transform.GetChild(i).GetComponent<TextMeshProUGUI>();
			texts[i].enabled = false;
		}
	}

	
	private void OnEnable()
	{
		GTime.onPaused += onPaused;
	}

	
	private void OnDisable()
	{
		GTime.onPaused -= onPaused;
	}

	
	private void onPaused(bool paused)
	{
		showingNames = paused;
		
		if(!showingNames || !showLines)
			for (int i = 0; i < texts.Length; i++)
				texts[i].enabled = false;
	}		


	private void LateUpdate()
	{
		if (!Application.isMobilePlatform && Input.GetKeyDown(KeyCode.N))
		{
			showingNames = !showingNames;
			onPaused(showingNames);
		}
		
		
		
		if (!showingNames)
			return;

		
		if (!showLines)
		{
			for (int i = 0; i < texts.Length; i++)
				texts[i].enabled = false;
			
			return;
		}


		int activeCount = Spinner.active.Count;
		for (int i = 0; i < texts.Length; i++)
		{
			Spinner spinner = i < activeCount ? Spinner.active[i] : null;
			
			if (spinner != null && spinner.visible)
			{
				Clip clip = spinner.currentClip;
				if (!clip.GetSide(GTime.Now).IsCamSide)
				{
					if (texts[i].enabled)
						texts[i].enabled = false;
					continue;
				}
					
				
				if (!texts[i].enabled)
					texts[i].enabled = true;


				if (texts[i].text != spinner.name)
					texts[i].text = spinner.name;
				
				Vector3 pos    = spinner.currentPlacement.pos;
				Vector3 offset = Camera.main.transform.up * spinner.size.y * -.85f;
				Vector3 screen = Camera.main.WorldToScreenPoint(pos + offset);
				
				Vector2 rectPos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screen, null, out rectPos);
				texts[i].rectTransform.anchoredPosition = rectPos;
			}
			else 
				if (texts[i].enabled)
					texts[i].enabled = false;
		}	
	}
}

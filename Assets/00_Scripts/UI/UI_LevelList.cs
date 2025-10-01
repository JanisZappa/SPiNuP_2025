using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_LevelList : ActiveUI
{
	public const string NewLevel = "NEW!";
	
	public static LevelList levelList;
	private static readonly List<GameObject> loadButtons = new List<GameObject>(20);

	public GameObject referenceButton;


	public override void OnEnable()
	{
		levelList = LevelSaveLoad.LevelList;
		if (levelList.levels.Count == 0)
		{
			LevelSaveLoad.CurrentLevel = NewLevel;
			return;
		}
		
		loadButtons.Clear();
		referenceButton.SetActive(true);

		float yOffset = 100f * .5f * 
		                (levelList.levels.Count + (GameManager.IsCreator ? 1 : 0));
		
		for (int i = 0; i < levelList.levels.Count; i++)
		{
			if(levelList.levels[i] == null)
				continue;
			
			GameObject newButton = Instantiate(referenceButton, transform, false);

			newButton.name = newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = levelList.levels[i].name;
			
			newButton.GetComponent<RectTransform>().anchoredPosition = Vector2.up * (yOffset - i * 100);
			loadButtons.Add(newButton);
		}
		
		//	Add "NEW!" Button	//
		if(GameManager.IsCreator)
		{
			GameObject newButton = Instantiate(referenceButton, transform, false);
			newButton.GetComponent<Image>().color = COLOR.red.tomato;

			newButton.name = newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = NewLevel;
			
			newButton.GetComponent<RectTransform>().anchoredPosition = Vector2.up * (yOffset - levelList.levels.Count * 100);
			loadButtons.Add(newButton);
		}
		
		referenceButton.SetActive(false);
		
		base.OnEnable();
	}

	
	public override bool HitUI(int click)
	{
		for (int i = 0; i < loadButtons.Count; i++)
			if (UI_Manager.ImPointedAt(loadButtons[i]))
			{
				if(click == 1)
					LevelSaveLoad.CurrentLevel = loadButtons[i].name;
				
				return true;
			}

		 return false;
	}
}

using LevelElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_ElementInfo : MonoBehaviour 
{
	public  GameObject elementInfo;
	private TextMeshProUGUI       elementInfoText;
	private Image      elementInfoImage;
	private Color      infoA, infoB;
	
	private static Element highlight;
	
	
	private void OnEnable() 
	{
		elementInfoText  = elementInfo.GetComponentInChildren<TextMeshProUGUI>();
		elementInfoImage = elementInfo.GetComponentInChildren<Image>();
		infoA = elementInfoImage.color;
		infoB = COLOR.purple.orchid;
	}
	
	
	private void LateUpdate () 
	{
		if (ElementEdit.element != null)
		{
			elementInfoImage.color = infoB;
			elementInfoText.text = ElementEdit.element.GetInfo();
			elementInfoImage.rectTransform.sizeDelta = new Vector2(elementInfoText.preferredWidth + 20, elementInfoText.preferredHeight + 20);
		
			return;
		}


		if (highlight != LevelCheck.ClosestElement || elementInfoImage.color != infoA)
		{
			highlight = LevelCheck.ClosestElement;

			elementInfo.SetActive(highlight != null);

			if (highlight != null)
			{
				elementInfoImage.color = infoA;
				elementInfoText.text = highlight.GetInfo();
				
				elementInfoImage.rectTransform.sizeDelta = new Vector2(elementInfoText.preferredWidth + 20, elementInfoText.preferredHeight + 20);
			}
		}
	}
}

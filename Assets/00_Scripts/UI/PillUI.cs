using TMPro;
using UnityEngine;


public class PillUI : MonoBehaviour
{
	private static TextMeshProUGUI text;
	
	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	public static void SetPillCount(int count)
	{
		if (text != null)
			text.text = count.PrepString();
	}
}

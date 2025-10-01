using System.Text;
using TMPro;
using UnityEngine;


public class PoolDebug : MonoBehaviour
{
	private TextMeshProUGUI text;
	public RectTransform image;
	private readonly StringBuilder stringBuilder = new StringBuilder(10000, 10000);

	
	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	
	private void Update()
	{
		stringBuilder.Length = 0;

		int length = PoolInfo.infos.Count;
		for (int i = 0; i < length; i++)
			PoolInfo.infos[i].AddDebugInfo(stringBuilder, i == length - 1);

		text.text = stringBuilder.ToString();

		text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
		image.sizeDelta              = new Vector2(text.preferredWidth + 20, text.preferredHeight + 20);
	}
}

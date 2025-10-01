using System.Text;
using TMPro;
using UnityEngine;


public class ActorDebug : MonoBehaviour
{
	private TextMeshProUGUI text;
	public RectTransform image;
	private readonly StringBuilder stringBuilder = new(10000, 10000);

	private int max;
	
	
	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	
	private void Update()
	{
		if (!GameManager.Running)
			return;
		
		stringBuilder.Length = 0;

		max = Mathf.Max(max, Level.itemCount);
		stringBuilder.Append(Level.itemCount.PrepString().PadLeft(3) + " < " + max.PrepString().PadLeft(3));
		stringBuilder.Append("\n");
		
		int length = ActorAnimator.actorLists.Length;
		for (int i = 0; i < length; i++)
			ActorAnimator.actorLists[i].AddInfo(stringBuilder, i == length - 1);

		text.text = stringBuilder.ToString();

		text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
		image.sizeDelta              = new Vector2(text.preferredWidth + 20, text.preferredHeight + 20);
	}
}

using System.Text;
using TMPro;
using UnityEngine;


public class BGPieceCounterUI : MonoBehaviour
{
	private TextMeshProUGUI text;
	private int max;
	private readonly StringBuilder builder = new StringBuilder(100);

	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update ()
	{
		if (GameManager.Running)
		{
			int count = PlacerMeshes.PieceObjects.ActiveCount;
			max = Mathf.Max(max, count);
			
			builder.Length = 0;
			builder.Append(count.PrepString()).Append(" | ").Append(max.PrepString());
			text.text = builder.ToString();
		}	
	}
}

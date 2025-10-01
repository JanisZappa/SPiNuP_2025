using TMPro;
using UnityEngine;


public class BadFrame : MonoBehaviour
{
	public TextMeshProUGUI text;
	public int alertFrameRate;
	private int drops;
	
	

	private void OnEnable()
	{
		drops = 0;
		text.text = drops.PrepString();
	}


	private void Update()
	{
		if (1f / Time.deltaTime <= alertFrameRate)
		{
			drops++;
			text.text = drops.PrepString();
		}
	}
}

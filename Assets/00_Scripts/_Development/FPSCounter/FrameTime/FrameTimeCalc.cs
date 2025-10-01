using TMPro;
using UnityEngine;


public class FrameTimeCalc : MonoBehaviour
{
	public FirstUpdate firstUpdate;
	private float      frameTime;
	
	private const float step = 1f;
	private TextMeshProUGUI  text;
	private float checkTime;
	private int   frames;
	
	
	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
		checkTime = Time.realtimeSinceStartup;
	}
	
	
	private void Update ()
	{
		float time = Time.realtimeSinceStartup;

		if (time - checkTime >= step)
		{
			frameTime /= frames;
			
			int fps = (int)Mathf.Max(0, Mathf.Clamp(Mathf.Ceil(1f / frameTime), 0, 100000));

			text.text = fps.PrepString();

			frames = 0;
			checkTime = time;
			frameTime = 0;
		}
		else
		{
			frameTime += time - firstUpdate.frameStart;
			frames++;
		}
	}
}

using UnityEngine;


public class TimedLightSource : MonoBehaviour
{
	public bool front;
	
	private float randomness;
	private SpriteRenderer sR;

	
	private void Awake()
	{
		const float range = .005f;
		randomness = Random.Range(-range, range);

		sR = GetComponent<SpriteRenderer>();
	}

	
	private void LateUpdate ()
	{
		if(GameManager.Running)
			sR.enabled = front == GameCam.CurrentSide.front && 
			             Mathf.PingPong(SunDir.DayTimeLerp * 2, 1) + randomness < .1f;
	}
}

using UnityEngine;


public class HeightAmbience : MonoBehaviour
{
	public AudioSource source;
	public Vector4 heights;

	private float volume;


	private void Awake()
	{
		volume = source.volume;
	}


	private void LateUpdate()
	{
		float camHeight = GameCam.CurrentPos.y;

		if (camHeight <= heights.w || camHeight >= heights.x)
		{
			source.volume = 0;
			return;
		}

		if (camHeight <= heights.z)
		{
			source.volume = volume * (1- Mathf.InverseLerp(heights.z, heights.y, camHeight));
			return;
		}

		if (camHeight >= heights.y)
		{
			source.volume = volume * (1 - Mathf.InverseLerp(heights.y, heights.x, camHeight));
			return;
		}
		
		source.volume = volume;
	}
}

using UnityEngine;


public class MusicSideSpeed : MonoBehaviour
{
	public AudioSource source;
	public float frontPitch = 1, backPitch = .95f;

	private void OnEnable()
	{
		GameCam.OnNewCamSide += OnNewCamSide;
	}
	
	private void OnDisable()
	{
		GameCam.OnNewCamSide -= OnNewCamSide;
	}

	private void OnNewCamSide(bool front)
	{
		source.pitch = front ? frontPitch : backPitch;
	}
}

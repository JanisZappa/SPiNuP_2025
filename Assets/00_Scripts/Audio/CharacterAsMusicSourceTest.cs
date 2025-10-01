using UnityEngine;


public class CharacterAsMusicSourceTest : MonoBehaviour
{
	private Sound.SoundObject sound;
	
	private void LateUpdate()
	{
		if (!GameManager.Running)
			return;
		
	//  TODO? If this is funny to you  //
		
		/*if (Spinner.Get(1).IsActive && sound == null)
			sound = Sound.Get(Audio.Music.Katamari).SetSpinner(Spinner.Get(1)).Volume(.3f).Loop().Play();

		if (!Spinner.Get(1).IsActive && sound != null)
		{
			sound.Stop();
			sound = null;
		}*/
	}
}

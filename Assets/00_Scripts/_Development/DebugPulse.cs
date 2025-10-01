using UnityEngine;


public class DebugPulse : MonoBehaviour 
{
	private void Update () 
	{
		if(GameManager.Running && Input.GetKeyDown(KeyCode.P) && !GTime.Paused)
			ActorAnimator.DebugPulse();
	}
}

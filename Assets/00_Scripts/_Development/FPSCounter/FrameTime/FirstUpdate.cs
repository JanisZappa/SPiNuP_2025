using UnityEngine;


public class FirstUpdate : MonoBehaviour 
{
	public float frameStart;
	
	private void Update ()
	{
		frameStart = Time.realtimeSinceStartup;
	}
}

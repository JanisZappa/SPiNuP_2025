using UnityEngine;


public class OccilationTest : MonoBehaviour
{
	public float magnitude;
	public float frequency;
	
	private float startTime;

	private bool shake;


	private void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			shake = true;

			startTime = Time.realtimeSinceStartup;
		}
		
		if(!shake)
			return;

		float occi = GPhysics.NewOscillate(Time.realtimeSinceStartup - startTime, frequency, 10, true);
		
		transform.position = new Vector3(magnitude * occi, 0, 0);
	}
}

using UnityEngine;


public class wobble : MonoBehaviour 
{
	private float r;

	private void OnEnable()
	{
		r = Random.Range (0, 1000f);
	}


	private void Update () 
	{
		transform.localScale = new Vector3(S (5f), S (4f), S (6f));
	}


	private float S(float input)
	{
		return Mathf.Lerp(.25f, 1.4f, Mathf.PerlinNoise (input * .36f * Time.realtimeSinceStartup + r, input * .48f * Time.realtimeSinceStartup + r) * .5f + .5f);
	}
}

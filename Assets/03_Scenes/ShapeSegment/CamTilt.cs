using System.Collections;
using UnityEngine;


public class CamTilt : MonoBehaviour
{
	private float lerp;
	private bool straight = true;

	private int call;
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
			StartCoroutine(SwingCam());
		
		transform.rotation = Quaternion.Slerp(Rot.Zero, Quaternion.Euler(new Vector3(-17, -30, .5f)), lerp);
	}


	private IEnumerator SwingCam()
	{
		straight = !straight;
		
		call++;
		int myCall = call;

		float time = 0;
		float start = lerp;
		float end = straight ? 0 : 1;

		while (time < 1 && myCall == call)
		{
			time += Time.deltaTime * 3;
			lerp = Mathf.SmoothStep(start, end, Mathf.Pow(time, 1.5f));
			yield return null;
		}
	}
}

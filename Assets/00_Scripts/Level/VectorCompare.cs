using UnityEngine;


public class VectorCompare : MonoBehaviour 
{
	void Update ()
	{
		Vector3 dir = Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.forward) * Vector3.up * (Time.frameCount % 100 == 0? 0 : Random.Range(0, 200f));
		float dir_M = dir.magnitude;
		Vector3 dir_N = dir.normalized;
		Vector3 dir_Div = dir * (1f / dir_M);
		
		if(dir_N != dir_Div)
			Debug.Log(dir_N.ToString("F4") + " != " + dir_Div.ToString("F4"));

	}
}

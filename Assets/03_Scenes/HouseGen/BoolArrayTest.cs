using UnityEngine;


public class BoolArrayTest : MonoBehaviour
{
	private bool[] v = new bool[1000000];
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			//float t = Time.realtimeSinceStartup;

			v.Clear();
			
			/*for (int i = 0; i < values.Length; i++)
			{
				values[i] = false;
			}*/
			
			//Debug.Log(Time.realtimeSinceStartup - t);
		}
	}
}

using UnityEngine;


public class LerpTest : MonoBehaviour
{
	public float value, a, b, result;


	private void Update ()
	{
		result = Mth.Repeat(a, b, value);
	}
}

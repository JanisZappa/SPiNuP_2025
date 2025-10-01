using UnityEngine;


public class DigitTest : MonoBehaviour
{
	public float value;
	public int division;

	[Space(10)] 
	public float a;
	public float b;

	[Space(10)] 
	public float result;


	private void Update () 
	{
		a = Mathf.Floor(value);
		b = Mathf.Ceil(value % 1f * division);

		result = a + b / division;
	}
}

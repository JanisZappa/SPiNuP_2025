using UnityEngine;


public class MaskCheck : MonoBehaviour
{
	public bool main, contact, occlusion, floor;
	
	
	private void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			byte byteValue = ((byte) 0).Set(main, contact, occlusion, floor);

			float floatValue = byteValue / 255f;
			
			Debug.Log(floatValue);
		}
	}
}

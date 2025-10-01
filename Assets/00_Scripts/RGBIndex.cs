using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBIndex : MonoBehaviour {

	public int r, g, b;
	
	
	[Space]
	
	public int index;

	private void Update()
	{
		index = r + g * 256 + b * 65536;
	}
}

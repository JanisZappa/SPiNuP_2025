using System.Collections.Generic;
using UnityEngine;

public class HouseDataViewer : MonoBehaviour
{
	public HouseGen.CornerIDs[] cornerIDs;
	private readonly List<HouseGen.CornerIDs> usedCornerIDs = new List<HouseGen.CornerIDs>();
	
	private HouseGen gen;
	
	private void Awake()
	{
		gen = GetComponent<HouseGen>();
		if (gen == null)
			enabled = false;
	}
	
	
	/*private void LateUpdate ()
	{
		usedCornerIDs.Clear();
		HouseGen.CornerIDs[] cIDs = gen.frontIDs;

		for (int i = 0; i < cIDs.Length; i++)
			if(cIDs[i].cornerCount > 0)
				usedCornerIDs.Add(cIDs[i]);
		
		cornerIDs = usedCornerIDs.ToArray();
	}*/
}

using System.Collections.Generic;
using UnityEngine;


public class ShadowTestToggle : Singleton<ShadowTestToggle>
{
	private static BoolSwitch ShowShadows;
	
	public GameObject shadowTest, treeTest;

	private int highQuality, lowQuality;

	private GameObject[] renderers;


	private void Awake()
	{
		highQuality = LayerMask.NameToLayer("ShadowSprites");
		lowQuality  = LayerMask.NameToLayer("Lighting");
		
		List<GameObject> rend = new List<GameObject>();
		for (int i = 0; i < shadowTest.transform.childCount; i++)
			rend.Add(shadowTest.transform.GetChild(i).gameObject);

		renderers = rend.ToArray();
		
		ShowShadows = new ("Visuals/ShadowTest", false, b =>
			{
				Inst.shadowTest.SetActive(b);
				Inst.treeTest.SetActive(b);
			}
		);
	}


	private void Update()
	{
		if (ShowShadows)
		{
			for (int i = 0; i < renderers.Length; i++)
				renderers[i].layer = GameCam.CurrentSide.front ? highQuality : lowQuality;

			treeTest.layer = GameCam.CurrentSide.front ? lowQuality : highQuality;
		}
	}
}
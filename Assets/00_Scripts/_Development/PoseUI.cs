using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;


public class PoseUI : MonoBehaviour
{
	public TextMeshProUGUI text;
	public RectTransform bg;
	
	private readonly StringBuilder writer = new StringBuilder(1000);

	private bool visible;

	private string[] names;

	
	private void Awake()
	{
		if (Application.isMobilePlatform)
		{
			gameObject.SetActive(false);
			return;
		}
		
		List<string> nameList = new List<string>();
		
		Type type = typeof(Anim.Pose);
		FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static );

		int longest = 0;
		for (int i = 0; i < fieldInfos.Length; i++)
		{
			string fieldName = fieldInfos[i].Name;
			nameList.Add(fieldName);
			longest = Mathf.Max(longest, fieldName.Length + 1);
		}

		for (int i = 0; i < fieldInfos.Length; i++)
			nameList[i] = nameList[i].PadRight(longest);

		names = nameList.ToArray();
	}
	
	
	private void LateUpdate ()
	{
		if (!GameManager.Running)
			return;

		if (Input.GetKeyDown(KeyCode.I))
		{
			visible = !visible;
			text.gameObject.SetActive(visible);
			bg.gameObject.SetActive(visible);
		}

		if (visible)
		{
			Spinner spinner = Spinner.Get(0);
			spinner.rig.poser.currentPose.GetInfo(writer, names);

			/*if (spinner.currentClip != null && spinner.currentClip.Type.IsAnySwing())
				writer.Append("\nSpin: ").Append(((Swing) spinner.currentClip).GetActualSpeed(GTime.Now).ToString("F4"));*/
			
			text.text = writer.ToString();

			const float margin = 5;
			bg.sizeDelta = new Vector2(text.preferredWidth + margin * 2, text.preferredHeight + margin * 2);
			bg.anchoredPosition =new Vector2(-margin, -margin);
		}
	}
}

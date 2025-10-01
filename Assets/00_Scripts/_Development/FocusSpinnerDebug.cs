using UnityEngine;
using UnityEngine.UI;


public class FocusSpinnerDebug : MonoBehaviour
{
	public Text text;
	
	private void LateUpdate()
	{
		if(!GameManager.Running)
			return;

		Spinner spinner = Spinner.CurrentFocusClip != null ? Spinner.CurrentFocusClip.spinner : null;
		text.text = spinner != null ? spinner.ID + " " + spinner.name : "NONE";
	}
}

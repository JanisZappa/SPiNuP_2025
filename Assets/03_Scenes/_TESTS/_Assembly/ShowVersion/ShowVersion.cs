using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


#if !UNITY_STANDALONE

#endif

public class ShowVersion : MonoBehaviour
{
	private static readonly BoolSwitch showLogin = new ("Dev/Show Login", false);
	
	private Text text;

	private void Start ()
	{
		text = GetComponent<Text>();

		StartCoroutine(ShowTheVersion());
	}

	private IEnumerator ShowTheVersion()
	{
		text.text = Application.version;
		
		
		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime * 3;
			text.color = new Color(1, 1, 1, Mathf.SmoothStep(0, 1, t));
			yield return null;
		}

		
		t = 0;
		while (t < 1)
		{
			t += Time.deltaTime;
			yield return null;
		}
		
		
		t = 0;
		while (t < 1)
		{
			t += Time.deltaTime * 3;
			text.color = new Color(1, 1, 1, Mathf.SmoothStep(1, 0, t));
			yield return null;
		}

		SceneManager.LoadScene(showLogin? 2 : 1);
	}
}

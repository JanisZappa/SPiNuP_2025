using TMPro;
using UnityEngine;


public class TimeSliderUI : MonoBehaviour
{
	public GameObject sliderObject;
	public TextMeshProUGUI minTimeText, maxTimeText;

	[Space(10)] 
	public RectTransform sliderRange;
	public RectTransform actualSlider;
	public RectTransform handle;

	[Space(10)] public GameObject playButton;
	
	
	private float minTime, maxTime;

	private readonly Vector3[] corners = new Vector3[4];

	private float sliderLerp, smoothlerp;
	private bool  grabbing;

	private Vector2 mousePos;

	public static bool playingSlowMo;

	private bool playNormalSpeed;

	
	private void OnEnable()
	{
		GTime.onPaused += onPaused;
		sliderObject.SetActive(false);
		playButton.SetActive(false);
	}

	
	private void OnDisable()
	{
		GTime.onPaused -= onPaused;
	}

	
	private void onPaused(bool paused)
	{
		sliderObject.SetActive(paused);
		playButton.SetActive(paused);

		
		if (!paused)
			return;

		playingSlowMo = false;
		
		maxTime = GTime.Now;
		minTime = Mathf.Clamp(maxTime - GTime.RewindTime, GTime.StartTime, float.MaxValue);

		minTimeText.text = Mathf.FloorToInt(minTime).PrepString();
		maxTimeText.text = Mathf.FloorToInt(maxTime).PrepString();

		sliderLerp = smoothlerp = 1;
	}


	private void Update()
	{
		if (!sliderObject.activeInHierarchy)
			return;

	//	Slider Range  //
		sliderRange.GetLocalCorners(corners);
		actualSlider.offsetMin = V2.right * Mathf.Lerp(0, corners[3].x - corners[0].x, Mathf.InverseLerp(maxTime - GTime.RewindTime, maxTime, minTime));
		
		
	//	Grab  //
		if (!Input.GetMouseButton(0) && grabbing)
			grabbing = false;
		
		
		if (grabbing)
		{
			actualSlider.GetWorldCorners(corners);
			sliderLerp = Mathf.InverseLerp(corners[0].x, corners[3].x, Controll.TouchPos.x);
		}
		

		if (Input.GetKeyDown(KeyCode.LeftShift))
			playNormalSpeed = !playNormalSpeed;
		
		
		if (playingSlowMo)
		{
			float speed = 1f / (maxTime - minTime) * (playNormalSpeed? 1 : .25f);
			smoothlerp = sliderLerp = Mathf.Repeat(sliderLerp + Time.deltaTime * speed, 1);
		}
		else
			smoothlerp = Mathf.Lerp(smoothlerp, sliderLerp, Time.deltaTime * 4);
		

		actualSlider.GetLocalCorners(corners);
		handle.anchoredPosition = new Vector2(Mathf.Lerp(0, corners[3].x - corners[0].x, sliderLerp), 0);
		
		
		GTime.Now = Mathf.Lerp(minTime, maxTime, smoothlerp);
	}


	public void GrabbingHandle()
	{
		playingSlowMo = false;
		grabbing = true;
	}


	public static void PlayButtonPress()
	{
		playingSlowMo = !playingSlowMo;
	}
}

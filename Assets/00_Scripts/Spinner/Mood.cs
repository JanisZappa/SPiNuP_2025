using UnityEngine;


public static class Mood
{
	private static readonly BoolSwitch showMood = new ("Char/Mood", false);
	
	private static float updateMood, updateTime;
	
	
	public static void Reset()
	{
		updateMood = .5f;
		updateTime = GTime.Now;
	}


	public static void OnSwing(float time)
	{
		if (showMood && time > updateTime)
		{
			updateMood = Mathf.Clamp01(GetMood(time) + .03f);
			updateTime = time;
		}
	}
	
	
	public static void OnJump(float time)
	{
		/*if (time > updateTime)
		{
			updateTime = time;

			mood += .1f;
		}*/
	}


	public static void Update()
	{
		if (!showMood)
			return;
		
		float mood = GetMood(GTime.Now);
		
		float width        = Screen.width - 40;
		float moodWidth    = width * mood;
		float nonMoodWidth = width - moodWidth;

		const float barY = 54, barHeight = 12;
		DRAW.Rectangle(new Vector2(20 + width - nonMoodWidth *.5f, barY), new Vector2(nonMoodWidth, barHeight)).ToScreen().SetColor(Color.black).Fill(.2f, true);

		Color color = Color.Lerp(mood < .5f ? Color.Lerp(Color.red, Color.yellow, mood * 2) : Color.Lerp(Color.yellow, Color.cyan, mood * 2 - 1), Color.white, .1f);
		DRAW.Rectangle(new Vector2(20 + moodWidth *.5f, barY), new Vector2(moodWidth, barHeight)).ToScreen().SetColor(color).Fill(.65f, true);
	}


	private static float GetMood(float time)
	{
		return Mathf.Min(updateMood, Mathf.Clamp01(updateMood - (time - updateTime) * .01f));
	}
}

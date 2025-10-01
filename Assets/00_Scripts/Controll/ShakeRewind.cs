using System.Collections;
using Clips;
using UnityEngine;


public class ShakeRewind : Singleton<ShakeRewind>
{
	public static bool Rewinding;

	public static float CheckTime
	{
		get
		{
			if (!GameManager.Running)
				return 0;
			
			Clip clip = Spinner.CurrentPlayerClip;
			if (clip == null)
				return 0;
			
			if (!clip.Type.IsAnyJump())
				return GTime.Now;
			
			return clip.after != null ? GTime.Now - (clip.after.startTime - clip.startTime) : GTime.Now - GTime.RewindTime;
		}
	}
	
	
	private void OnEnable()
	{
		Rewinding = false;

		Controll.onShake += onShake;
	}


	private void OnDisable()
	{
		Controll.onShake -= onShake;
	}


	private void onShake()
	{
		if (!Rewinding && GameManager.Running && !GTime.Paused && 
		    Spinner.CurrentPlayerClip != null)
		{
			StartCoroutine(Rewind(Spinner.CurrentPlayerClip));
		}
	}

	
	private static IEnumerator Rewind(Clip clip)
	{
		float startTime;
		switch (clip.Type)
		{
			default: yield break;
			
			case ClipType.Jump:
			case ClipType.AirLaunch:
			case ClipType.SlipOff:
				startTime = clip.startTime;
				break;
			
			case ClipType.Bump:
				startTime = clip.before.startTime;
				break;
		}

		Rewinding = true;
		while (GTime.Now > startTime)
		{
			GTime.Speed = Mathf.Max(GTime.Speed - Time.deltaTime * 30, -2);
			yield return null;
		}

		
		 ActorAnimator.ClearAllAfter(clip.spinner, startTime);
		    ScoreStick.ClearAllAfter(clip.spinner, startTime);
		          Poof.ClearAfter(clip.spinner, startTime);
		     Collector.ClearAfter(clip.spinner, startTime);
		
		clip.spinner.Trimm(startTime);
		
		      MultiCam.Shake(Mathf.Abs(GTime.Speed));
		
		GTime.Speed = 1;
		Rewinding   = false;
	}
}


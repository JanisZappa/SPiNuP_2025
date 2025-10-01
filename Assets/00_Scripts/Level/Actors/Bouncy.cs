using UnityEngine;


public class Bouncy : Actor, IElementAction
{
	[Space(10)] 
	public MeshColorSwap colorSwitch;
	
	private float _lerp;
	
	public override void SetTransform(bool forcedUpdate)
	{
		float bigLerp   = Mathf.PingPong((GTime.Now + (item.ID + 1) * .2254f) * GTime.LoopMulti * 4, 1) * 8;
		float smallLerp = Mathf.Clamp01(bigLerp - 6);
		float animLerp  = Mathf.SmoothStep(0, 1, 1 - smallLerp);
		
		if(!HasToUpdate() && !forcedUpdate && Mathf.Abs(animLerp - _lerp) < .01f)
		{
			ShadowUpdate(false);
			return;
		}

		
		_lerp = animLerp;
		Vector2 newLean = anim?.GetLeanNow() ?? V2.zero;
		
		SetDepthPos(item.rootPos);

		Quaternion sideRot = Rot.Y(item.side.front ? 0 : 180);
		Quaternion leanRot = (newLean * animLerp).LeanRotLocal(item);
		         TipOffset = sideRot * leanRot * new Vector3(0, 0, 6f + 3f * animLerp);
		
		
		pivot.localPosition = leanRot * new Vector3(0, 0, 3f * (1 - animLerp));
		pivot.localRotation = leanRot;
		
		ShadowUpdate(true);
	}

	public void Action()
	{
		colorSwitch.onSwitchActorColor();
	}
}


public interface IElementAction
{
	void Action();
}
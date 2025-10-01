using UnityEngine;


public class AngleCam : MonoBehaviour
{
	private bool swiping;
	private Vector2 lastFingerPos;
	
	private Quaternion spin = Rot.Zero, smoothSpin = Rot.Zero;

	public Quaternion pivotRot = Quaternion.identity;
	private Side side;
	
	
	private void OnEnable()
	{
		Controll.onSwipe += onSwipe;
	}

	
	private void OnDisable()
	{
		Controll.onSwipe -= onSwipe;
	}
	
	
	private void onSwipe(bool start)
	{
		if (start)
		{
			if (!swiping && (GTime.Paused || GameManager.IsCreator && Input.GetKey(KeyCode.LeftAlt)))
			{
				swiping = true;
				lastFingerPos = Controll.TouchPos;
				spin = smoothSpin = Rot.Zero;
			}
		}
		else
			swiping = false;
	}


	public void Reset()
	{
		CamUpdate();
	}
	
	
	public void CamUpdate()
	{
		if (!GTime.Paused || !side.IsCamSide)
		{
			pivotRot = Quaternion.SlerpUnclamped(GameCam.PivotRot, pivotRot, GameCam.PauseLerp);
			spin     = smoothSpin = Quaternion.identity;
		
			if (!GTime.Paused)
				side = GameCam.CurrentSide;
		
			return;
		}

		
		if (!GameManager.IsCreator || Input.GetKey(KeyCode.LeftAlt))
			if (swiping)
			{
				Vector2 moveDelta = (Controll.TouchPos - lastFingerPos) / ScreenControll.PixelPerMilimeter;
				        moveDelta = moveDelta.normalized * Mathf.Clamp(moveDelta.magnitude, 0, 5);

				if (!GameCam.CurrentSide.front)
					moveDelta = new Vector2(moveDelta.x, -moveDelta.y);

				spin          = Quaternion.AngleAxis(moveDelta.magnitude * 4.25f, moveDelta.normalized.Rot(90));
				lastFingerPos = Controll.TouchPos;
			}
			else
				spin = Rot.Zero;

		
		smoothSpin = Quaternion.Slerp(smoothSpin, spin, Time.deltaTime * (swiping? 7 : 3));
		
		Quaternion combined = ClampAngler(smoothSpin * pivotRot, GameCam.CurrentSide.front);

		Vector3 aim = combined * V3.forward;
		if(float.IsNaN(aim.x))
			aim = Vector3.forward;
		pivotRot = Quaternion.LookRotation(aim, V3.up);
	}
	
	
	private static Quaternion ClampAngler(Quaternion rot, bool front)
	{
		float dot         = Vector3.Dot(rot * V3.forward, V3.forward);
		float dotDistance = 1 - Mathf.Abs(dot);
		float newDot      = Mth.DampedRange(Mathf.Clamp(dotDistance, 0, GameCam.MaxDot), GameCam.MaxDot * .4f, GameCam.MaxDot);
        
		Quaternion sideForward = front ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
		return Quaternion.Slerp(sideForward, rot, newDot / dotDistance);
	}
}

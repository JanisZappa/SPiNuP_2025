using UnityEngine;


public class PauseCam : MonoBehaviour
{
	public bool pauseSideFront;
	
	private bool swiping, zooming;
	
	private float dollyGoal;
	public  static bool Flip;

	public Vector2 movePos;
	
	
	
	
	private void OnEnable()
	{
		GTime.onPaused  += onPaused;
		Controll.onZoom += onZoom;
	}

	
	private void OnDisable()
	{
		GTime.onPaused  -= onPaused;
		Controll.onZoom -= onZoom;
	}

	
	private void onZoom(bool start)
	{
		if (start)
		{
			if (!zooming && GTime.Paused)
			{
				zooming    = true;
			}
		}
		else
			zooming = false;
	}
	
	
	private void onPaused(bool paused)
	{
		if (paused)
		{
			pauseSideFront = GameCam.CurrentSide.front;
		}
	}
	
	
	public void CamUpdate()
	{
		Flip = GTime.Paused && Spinner.CurrentFocusClip.GetSide(GTime.Now).front != pauseSideFront;

		if (!GTime.Paused)
			return;

		movePos = Spinner.CurrentFocusClip.BasicPlacement(GTime.Now, true).pos;
	}
}

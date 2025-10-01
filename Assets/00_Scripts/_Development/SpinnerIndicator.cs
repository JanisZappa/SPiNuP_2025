using Clips;
using GeoMath;
using UnityEngine;


public class SpinnerIndicator : MonoBehaviour
{
	public static readonly BoolSwitch Indication = new("Char/Indicator", false);
	
	private void LateUpdate () 
	{
		if(!GameManager.Running || !Indication)
			return;
		
		
		int activeCount = Spinner.active.Count;
		for (int i = 0; i < activeCount; i++)
		{
			Spinner spinner = Spinner.active[i];
			Clip clip = spinner.currentClip;
			
			if(clip == null || clip.Type.IsNotPlaying())
				continue;
			
			bool    sameSide  = clip.GetSide(GTime.Now).IsCamSide;
			Color   color     = sameSide ? Color.white : Color.gray;
			Vector2 screenPos = GameCam.Cam.WorldToScreenPoint(spinner.currentPlacement.pos);
			bool    onScreen  = OnScreen(screenPos);
			
			if(sameSide && onScreen)
				continue;
			
			if (!onScreen)
			{
				Vector2 center = new Vector2(ScreenControll.Width * .5f, ScreenControll.Height * .5f);
				Line dirLine = new Line(center, screenPos);

				Vector2 point;
				if(new Line(A, B).Contact(dirLine, out point) ||
				   new Line(B, C).Contact(dirLine, out point) ||
				   new Line(C, D).Contact(dirLine, out point) ||
				   new Line(D, A).Contact(dirLine, out point))
					screenPos = point;
			}
			
			DRAW.Circle(screenPos, IndicatorRadius * .95f, 20).SetColor(Color.black).ToScreen().Fill(.5f, true);
			DRAW.MultiCircle(screenPos, IndicatorRadius, 3, IndicatorRadius * .025f, 20).SetColor(Color.magenta).ToScreen();
			
			DRAW.Text(i.PrepString(), screenPos, color, IndicatorRadius * .2f, screenSpace: true);
		}
	}
	
	
	private static float IndicatorRadius => (ScreenControll.Landscape ? ScreenControll.Width : ScreenControll.Height) * .0125f;


	private static bool OnScreen(Vector2 screenPos)
	{
		return screenPos.x >= IndicatorRadius && screenPos.x < ScreenControll.Width - IndicatorRadius && screenPos.y >= IndicatorRadius && screenPos.y < ScreenControll.Height - IndicatorRadius;
	}


	private static Vector2 A => new(IndicatorRadius, IndicatorRadius);
	private static Vector2 B => new(IndicatorRadius, ScreenControll.Height - IndicatorRadius);
	private static Vector2 C => new(ScreenControll.Width - IndicatorRadius, ScreenControll.Height - IndicatorRadius);
	private static Vector2 D => new(ScreenControll.Width - IndicatorRadius, IndicatorRadius);
}

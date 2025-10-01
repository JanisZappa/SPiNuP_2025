using UnityEngine;


public class CamDebug : MonoBehaviour 
{
	private static readonly BoolSwitch showFrustum    = new("Camera/Frustum", false);
	private static readonly BoolSwitch showAttractors = new("Camera/Attractors", false);
	private static readonly BoolSwitch mapOutline     = new("Camera/Map Outline", false);


	private void LateUpdate()
	{
		if (!GameManager.Running)
			return;
		
		if(showFrustum)
			GameCam.frustum.DebugDraw();

		if (showAttractors)
			ShowAttractors();
		
		if(mapOutline)
			MapCam.MapOutlineDRAW();
	}

	
	private static void ShowAttractors()
	{
		Color c = Color.Lerp(COLOR.yellow.fresh, COLOR.red.tomato, .5f);	
		
		DRAW.Circle(GameCam.frustum.focusPoint, .75f, 12).SetColor(COLOR.red.tomato).SetDepth(Z.W10);
		DRAW.Circle(GameCam.frustum.focusPoint, .55f, 12).SetColor(c).SetDepth(Z.W10);
		
		DRAW.Circle(GameCam.frustum.focusPoint, .75f, 12).SetColor(COLOR.red.tomato).SetDepth(Z.M);
		DRAW.Circle(GameCam.frustum.focusPoint, .55f, 12).SetColor(c).SetDepth(Z.M);
		
		DRAW.GapVector(GameCam.frustum.focusPoint.SetZ(Z.W10), GameCam.frustum.focusPoint.SetZ(Z.M) - GameCam.frustum.focusPoint.SetZ(Z.W10), 10).SetColor(COLOR.turquois.aquamarine);
		
		
		MoveCam moveCam = MoveCam.Inst;
		
		DrawLine(moveCam.charPos, COLOR.red.tomato, GameCam.frustum.focusPoint, COLOR.turquois.aquamarine);
		DrawLine(moveCam.charPos + moveCam.smoothLookAhead, COLOR.turquois.aquamarine, moveCam.charPos, COLOR.turquois.aquamarine);
	}


	private static void DrawLine(Vector3 pos, Color posColor, Vector3 root, Color rootColor)
	{
		DRAW.DotVector(pos, root - pos, .1f, .2f).SetColor(rootColor).SetDepth(Z.W10);
		DRAW.DotVector(pos, root - pos, .1f, .2f).SetColor(rootColor).SetDepth(Z.M);
		
		DRAW.GapVector(pos.SetZ(Z.W10), pos.SetZ(Z.M) - pos.SetZ(Z.W10), 10).SetColor(posColor);
	}
}

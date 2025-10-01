using UnityEngine;


public class EdgeMaster : Singleton<EdgeMaster>
{
	public GameObject edgeA, edgeB, backA, backB, fill;

	[Space(10)] 
	public float frontWidth;
	public float backWidth;

	private MeshRenderer fillRenderer, edgeRendererA, edgeRendererB, backRendererA, backRendererB;

	private bool front, edgeVisible, backVisible;
	private int xEdgeStep, xBackStep;

	private static bool isActive = true;
	
	
	private void Start () 
	{
	//  Edge  //
		edgeRendererA = edgeA.GetComponent<MeshRenderer>();
		edgeRendererB = edgeB.GetComponent<MeshRenderer>();
		
		edgeA.transform.rotation = Quaternion.Euler(0, 180, 0);
			
	//  Back  //
		backRendererA = backA.GetComponent<MeshRenderer>();
		backRendererB = backB.GetComponent<MeshRenderer>();
		
		backA.transform.rotation = Quaternion.Euler(0, 180, 0);
			
	//  Fill  //
		fillRenderer = fill.GetComponent<MeshRenderer>();
	}
	

	private void LateUpdate () 
	{
		if (!GameManager.Running || !isActive)
			return;

		bool camFront      = GameCam.CurrentSide.front;
		bool differentSide = camFront != front;
		front = camFront;
		
	
	//  Camera Height dependent Visibility Check  //
		bool edgeShouldBeVisible = GameCam.CurrentPos.y < 110;
		if (edgeShouldBeVisible != edgeVisible)
		{
			edgeVisible = edgeShouldBeVisible;
			
			edgeRendererA.enabled = edgeVisible;
			edgeRendererB.enabled = edgeVisible;
			fillRenderer.enabled  = edgeVisible;
		}
		
		bool backShouldBeVisible = GameCam.CurrentPos.y < 250;
		if (backShouldBeVisible != backVisible)
		{
			backVisible = backShouldBeVisible;
			backRendererA.enabled = backVisible;
			backRendererB.enabled = backVisible;
		}

		
	//  Check X Steps  //
		
	//  Front  //
		float cPos   = GameCam.CurrentPos.x;
		float focus  = GameCam.frustum.focusPoint.x;
		float frontX = cPos + (focus - cPos) * (ScreenControll.Landscape? 8 : 3);

		int camXEdgeStep = Mathf.RoundToInt(frontX / frontWidth);
		if (edgeVisible && (differentSide || xEdgeStep != camXEdgeStep))
			EdgeUpdate(camXEdgeStep);
		
	//  Back  //
		float backX =  cPos + (focus - cPos) * (ScreenControll.Landscape? 8 : 3) * 2.5f;

		int camXBackStep  = Mathf.RoundToInt(backX / backWidth);
		if (backVisible && (differentSide || xBackStep != camXBackStep))
			BackUpdate(camXBackStep);
	}

	
	private void EdgeUpdate(int xStepPos)
	{
		xEdgeStep = xStepPos;
		
		if (front)
			edgeA.transform.position = new Vector3(xEdgeStep * frontWidth, 0, -Level.WallDepth);
		else
			edgeB.transform.position = new Vector3(xEdgeStep * frontWidth, 0, Level.WallDepth);

		fill.transform.position = new Vector3(xEdgeStep * frontWidth, 0);
	}
	
	
	private void BackUpdate(int xStepPos)
	{
		xBackStep = xStepPos;

		if (front)
			backA.transform.position = new Vector3(xBackStep * backWidth, 0,  Level.WallDepth);
		else
			backB.transform.position = new Vector3(xBackStep * backWidth, 0, -Level.WallDepth);
	}


	public static void SetActive(bool active)
	{
		if (isActive != active)
		{
			isActive = active;
			
			Inst.fillRenderer.enabled  = isActive;
			Inst.edgeRendererA.enabled = isActive;
			Inst.edgeRendererB.enabled = isActive; 
			Inst.backRendererA.enabled = isActive; 
			Inst.backRendererB.enabled = isActive;

			if (isActive)
			{
				Inst.xEdgeStep = int.MinValue; 
				Inst.xBackStep = int.MinValue; 
			}
		}
	}
}

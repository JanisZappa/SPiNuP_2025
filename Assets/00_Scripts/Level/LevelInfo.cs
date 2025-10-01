using UnityEngine;


public partial class Level
{
	public const int cellXmin  = -13, cellXmax = 13, cellYmin = 0, cellYmax  = 36;
	public const int xCells    = cellXmax - cellXmin;
	private const int yCells   = cellYmax - cellYmin;
	public const int cellCount = xCells * yCells;
	
	
	public const int   CellSize        = 15;	//15
	public const float CellHalfSize    = CellSize * .5f;
	
	public const float PlacementRadius = 2f;
	public const int   MaxActiveElements = 400;
	
	public const float WallDepth       = 1.55f; //2.8f; //1.6f;   //2.5f;   //3f;   //.05f;   //5f;   //Lowest: .05f   Old: 3.5f//
	public const float PlaneOffset     = 2.5f;
	public const float BorderDepth     = WallDepth + PlaneOffset * 2;


	public static float GetWallDist(Side side, float offset = 0)
	{
		return side.Sign * (WallDepth + offset);
	}
	
    
	public static float GetPlaneDist(Side side, float offset = 0)
	{
		return side.Sign * (WallDepth + PlaneOffset + offset);
	}
	
        
	public static Vector3 GamePlane(Side side)
	{
		return new Vector3(0, 0, GetPlaneDist(side));
	}


	private static int hitPointFrame;
	private static Vector2 cachedHitpoint;
	public static Vector2 HitPoint
	{
		get
		{
			if (hitPointFrame != Time.frameCount)
			{
				hitPointFrame = Time.frameCount;
				
				Ray fingerRay = GameCam.Cam.ScreenPointToRay(Input.mousePosition);
				float multi = (Mathf.Abs(fingerRay.origin.z) - WallDepth) / Mathf.Abs(fingerRay.direction.z);
				cachedHitpoint = fingerRay.origin + fingerRay.direction * multi;
			}

			return cachedHitpoint;
		}
	}
}

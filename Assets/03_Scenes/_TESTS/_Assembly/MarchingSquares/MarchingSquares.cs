using UnityEngine;


public class MarchingSquares : MonoBehaviour
{
	public int width, height;
	public bool draw;
	public Material mat;
	public int testBlockCount;
	
	public GameObject[] fbx;

	private int[] edgePoints, frontPoints, backPoints;

	private int genWidth, genHeight;
	

	private void Start () 
	{
		Generate();
	}
	
	
	private void Update () 
	{
		if(width != genWidth || height != genHeight || Input.GetKeyDown(KeyCode.Space))
			Generate();
			

		if (!draw)
			return;

		float depth = Camera.main.transform.forward.z > 0 ? -.51f : .51f;
		
		Vector2 min = new Vector2(genWidth * -.5f + .5f, genHeight * -.5f + .5f);
		for (int y = 0; y < genHeight; y++)
		for (int x = 0; x < genWidth; x++)
		{
			Vector2 pos = new Vector2(min.x + x, min.y + y);
			int index = y * genWidth + x;

			int value = edgePoints[index];
			if(value >= 1)
				DRAW.Circle(pos, .15f, 10).SetDepth(depth * value).SetColor(COLOR.turquois.aquamarine).Fill(1, true);
			else
				DRAW.Circle(pos, .1f, 8).SetDepth(depth * value).SetColor(COLOR.red.tomato).Fill(1, true);
		}
	}


	private void Generate()
	{
		foreach (Transform child in transform)
			Destroy(child.gameObject);

		genWidth = width;
		genHeight = height;

		GenerateEdgePoints();
	
		
		int xLess = genWidth  - 1;
		int yLess = genHeight - 1;
		float min = genWidth * -.5f + 1;
		for (int y = 0; y < yLess; y++)
		for (int x = 0; x < xLess; x++)
		{
		//  TL 8 TR 4 BR 2 BL 1  //
			int tile_index = GetIndex(x, y, edgePoints)  * 256 + 
			                 GetIndex(x, y, frontPoints) *  16 + 
			                 GetIndex(x, y, backPoints);
			
			GameObject gO = Instantiate(fbx[tile_index]);
			
			Transform trans = gO.transform;
			          trans.SetParent(transform, false);
			          trans.rotation      = Quaternion.Euler(0, 180, 0);
			          trans.localPosition = new Vector2(min + x, y + .5f);

			MeshRenderer mR = gO.GetComponent<MeshRenderer>();
				
			if(mR != null)
				mR.material = mat;
		}
	}


	private int GetIndex(int x, int y, int[] points)
	{
		int a = points[(y + 1) * genWidth + x];
		int b = points[(y + 1) * genWidth + x + 1];
		int c = points[y       * genWidth + x + 1];
		int d = points[y       * genWidth + x];

		return a * 8 + b * 4 + c * 2 + d;
	}


	private void GenerateEdgePoints()
	{
		edgePoints  = new int[genWidth * genHeight];
		frontPoints = new int[genWidth * genHeight];
		backPoints  = new int[genWidth * genHeight];
		
	//  StackTangling  //
		int maxHeight = genHeight - 6;
		int buildHeight = 0;

		while (buildHeight < maxHeight)
		{
			int blockWidth  = Random.Range(6, 11);
			int xMin        = Random.Range(1, genWidth - 1 - blockWidth);
			int blockHeight = Random.Range(4, 7);

			int hitheight = 0;

			for (int x = 0; x < blockWidth; x++)
			{
				int checkX = xMin + x;

				for (int y = maxHeight; y > -1; y--)
				{
					int index = y * genWidth + checkX;
					if (edgePoints[index] == 1)
					{
						hitheight = Mathf.Max(hitheight, y + 1);
						break;
					}
				}
			}
			
			for (int y = 0; y < blockHeight; y++)
			for (int x = 0; x < blockWidth;  x++)
			{
				int index = (hitheight + y) * genWidth + xMin + x;
				edgePoints[index] = 1;
			}

			buildHeight = Mathf.Max(buildHeight, hitheight + blockHeight);
		}

		int counter = 0;
		while (counter < testBlockCount)
		{
			int castX = Random.Range(1, genWidth  - 1);
			int castY = Random.Range(1, genHeight - 1);
			
			if (edgePoints[castY * genWidth + castX] == 1)
			{
				int blockWidth  = Random.Range(3, 7);
				int xMin        = castX - blockWidth / 2;
				int blockHeight = Random.Range(6, 11);
				int yMin        = castY - blockHeight / 2;
				
				for (int y = 0; y < blockHeight; y++)
				for (int x = 0; x < blockWidth;  x++)
				{
					int xValue = xMin + x;
					int yValue = yMin + y;
					
					if(xValue >= 1 && xValue <= genWidth - 2 && yValue >= 1 && yValue <= genHeight - 2)
					{
						int index2 = yValue * genWidth + xValue;
						frontPoints[index2] = 1;
					}
				}

				counter++;
			}
		}
		counter = 0;
		while (counter < testBlockCount)
		{
			int castX = Random.Range(1, genWidth  - 1);
			int castY = Random.Range(1, genHeight - 1);
			
			if (edgePoints[castY * genWidth + castX] == 1)
			{
				int blockWidth  = Random.Range(3, 7);
				int xMin = castX - blockWidth / 2;
				int blockHeight = Random.Range(6, 11);
				int yMin = castY - blockHeight / 2;
				
				for (int y = 0; y < blockHeight; y++)
				for (int x = 0; x < blockWidth; x++)
				{
					int xValue = xMin + x;
					int yValue = yMin + y;
					
					if(xValue >= 1 && xValue <= genWidth - 2 && yValue >= 1 && yValue <= genHeight - 2)
					{
						int index2 = yValue * genWidth + xValue;
						backPoints[index2] = 1;
					}
				}

				counter++;
			}
		}
	}
}

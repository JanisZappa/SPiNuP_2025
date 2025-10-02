using System.Collections.Generic;
using House;
using Generation;
using GeoMath;
using UnityEngine;
using Piece = HouseGen.Piece;


[CreateAssetMenu]
public class PlacerMeshes : ScriptableObject
{
	public PieceAndShadow[] pieceShadows;
	
	[Space]
	public Material mat;
	public Material shadowMat;
	public int poolSize;
	
	public static readonly MeshPlacement[] placements = new MeshPlacement[10000];
	
	private static readonly PieceObjects[] activePieces  = new PieceObjects[10000];
	
	private static int placedMeshCount;

	private static Stack<PieceObjects> poolPieces;

	private static float scale;

	private static readonly BoolSwitch ForceFlat = new("Level/FlatWall", false);

	[System.Serializable]
	public class PieceAndShadow
	{
		public int id;
		public Mesh piece, shadow;

		public PieceAndShadow(int id, Mesh piece, Mesh shadow)
		{
			this.id     = id;
			this.piece  = piece;
			this.shadow = shadow;
		}
	}


	public void SetupMeshes(List<Mesh> pieces, List<Mesh> shadows, List<int> map)
	{
		Debug.LogFormat("Found {0} Pieces and {1} Shadows", pieces.Count, shadows.Count);
		
		pieceShadows = new PieceAndShadow[pieces.Count];

		for (int i = 0; i < pieces.Count; i++)
		{
			Mesh piece   = pieces[i];
			int id       = MeshNameToID(piece.name);
			int simpleID = id % 1000000000;

			int shadowIndex = 0;
			for (int e = 0; e < shadows.Count; e++)
				if (MeshNameToID(shadows[e].name) == simpleID)
				{
					shadowIndex = e;
					break;
				}
			
			int shadowMapIndex = map[shadowIndex];
			Mesh shadow = shadows[shadowMapIndex];
			
			pieceShadows[i] = new PieceAndShadow(id, piece, shadow);
		}
		
		List<string> uniqueNames = new List<string>();
		for (int i = 0; i < pieces.Count; i++)
		{
			string n = pieceShadows[i].shadow.name;
			
			if(!uniqueNames.Contains(n))
				uniqueNames.Add(n);
		}
		
		Debug.LogFormat("Collected {0} Shadow Meshes", uniqueNames.Count);
	}

	
	public static void Clear(float scale)
	{
		PlacerMeshes.scale = scale;
		
		for (int i = 0; i < placedMeshCount; i++)
		{
			MeshPlacement placement = placements[i];
			Level.RemoveBGMeshFromCells(i, GetBounds(placement), placement.side);
		}

		placedMeshCount = 0;
	}


	public void Setup(Transform parent)
	{		 
		PieceObjects.Pieces = new PieceDictionary(pieceShadows);

		if (!mat)
			mat = Resources.Load<Material>("Mats/MainVertexMat");
		
		poolPieces = new Stack<PieceObjects>(poolSize);
		for (int i = 0; i < poolSize; i++)
			poolPieces.Push(new PieceObjects(mat, shadowMat, parent));
	}


	public static void Place(Piece piece, HouseGen gen, int side, ref bool foundPiece)
	{
		int sideValue = Mathf.Max(0, side);
		
		Vector2 pos = gen.cellMin + (Vector2)piece.pos * HouseGen.cellSize;
		
		int pieceInfo = GetPieceInfo(sideValue, piece.cornerIDs.PieceID(), piece.size.x, piece.size.y, (int)piece.GetPieceType(ForceFlat));

		
		if (!PieceObjects.Pieces.HasMeshFor(pieceInfo))
		{
			pieceInfo = GetPieceInfo(sideValue, piece.cornerIDs.PieceID(true), piece.size.x, piece.size.y, (int)piece.GetPieceType(ForceFlat));
			foundPiece = false;
		}
		
		if (!PieceObjects.Pieces.HasMeshFor(pieceInfo))
		{
			Debug.Log("Nope: " + pieceInfo.ToString("D10") + " " + piece.cornerIDs.Log());
			return;
		}
			
		
		MeshPlacement placement = new MeshPlacement(pos, pieceInfo, piece.cornerIDs, side);
		Level.AddBGMeshToCells(placedMeshCount, GetBounds(placement), side);

		placements[placedMeshCount++] = placement;
	}


	public static void UpdateVisiblePieces(int[] addPieces, int addCount, int[] removePieces, int removeCount)
	{
	//  Remove  //
		for (int i = 0; i < removeCount; i++)
		{
			int id = removePieces[i];

			PieceObjects piece = activePieces[id];
			
			if(piece != null)
				poolPieces.Push(piece.SetInactive());
		}


	//  Add  //
		for (int i = 0; i < addCount; i++)
		{
			int id = addPieces[i];

			if (poolPieces.Count > 0)
				activePieces[id] = poolPieces.Pop().SetActive(placements[id]);
			else
				break;
		}
	}


	private static Bounds2D GetBounds(MeshPlacement placement)
	{
	//  Get Dimensions  //
		int x = Mathf.FloorToInt(placement.pieceInfo % 1000 / 100.0f);
		int y = Mathf.FloorToInt(placement.pieceInfo % 100 / 10.0f);
		
		//Debug.LogFormat("PieceBounds for {0} = {1}|{2}", placement.pieceInfo, x, y);

		Vector2 min = placement.pos - new Vector2(scale * .5f, scale * .5f);
		return new Bounds2D(min).Add(min + new Vector2(x * scale, y * scale));
	}


	public static void DrawPieceBounds(int pieceIndex)
	{
		MeshPlacement placement = placements[pieceIndex];
		
		GetBounds(placement).Pad(-.1f).Draw().SetColor(Color.red).SetDepth(Z.W20);
	}

	
	public static void Report()
	{
		Debug.LogFormat("Got {0} MeshPlacements", placedMeshCount);
	}


	public static int GetPieceInfo(int side, int pieceID, int x, int y, int surface)
	{
		return side    * 1000000000 +
		       pieceID *       1000 +
		       x       *        100 +
		       y       *         10 +
		       surface;
	}
	
	
	public static int MeshNameToID(string name)
	{
		string[] parts = name.Split('_');
				
		string[] corners = parts[1].Split('.');
				
		int pieceID = 
			HouseHelp.GetPieceID(
				int.Parse(corners[0]), 
				int.Parse(corners[1]), 
				int.Parse(corners[2]), 
				int.Parse(corners[3]));
				
		string[] size = parts[2].Split('.');
		int x = int.Parse(size[0]);
		int y = int.Parse(size[1]);
				
		string surface = parts[3];
		int surf;
		switch (surface)
		{
			default:       surf = 0; break;
			case "Bump":   surf = 1; break;
			case "Hole":   surf = 2; break;
			case "Window": surf = 3; break;
		}
				
		int side = parts[4] == "F"? 0 : 1;
				
		return GetPieceInfo(side, pieceID, x, y, surf);
	}
		


	public class PieceObjects
	{
		private readonly Transform    pieceTransform, shadowTransform;
		private readonly MeshFilter   pieceFilter, shadowFilter;
		private readonly MeshRenderer pieceRenderer, shadowRenderer;

		public static PieceDictionary Pieces;

		public static int ActiveCount;

		public PieceObjects(Material pieceMat, Material shadowMat, Transform parent)
		{
			{
				GameObject gO = new GameObject("Piece");
                gO.transform.parent = parent;
			
				pieceTransform  = gO.transform;
				pieceFilter     = gO.AddComponent<MeshFilter>();
				pieceRenderer   = gO.AddComponent<MeshRenderer>();

				pieceRenderer.material = pieceMat;
				pieceRenderer.enabled  = false;
			}
			{
				GameObject gO = new GameObject("Shadow");
                gO.transform.parent = parent;
                
				shadowTransform  = gO.transform;
				shadowFilter     = gO.AddComponent<MeshFilter>();
				shadowRenderer   = gO.AddComponent<MeshRenderer>();

				shadowRenderer.material = shadowMat;
				shadowRenderer.enabled  = false;
				
				gO.layer = LayerMask.NameToLayer("Lighting");
			}
		}

		public PieceObjects SetInactive()
		{
			pieceRenderer.enabled = false;
			shadowRenderer.enabled = false;
			ActiveCount--;
			
			return this;
		}

		public PieceObjects SetActive(MeshPlacement placement)
		{
			PieceAndShadow mesh = Pieces.GetMesh(placement.pieceInfo);
		    
			if (mesh == null)
			{
				Debug.Log("Can't find Piece " + placement.pieceInfo.ToString("D10").B_Purple() + " " + placement.corners.Log());
				return this;
			}
			
			
			pieceTransform.position = placement.pos;
			pieceRenderer.enabled   = true;
			pieceFilter.sharedMesh  = mesh.piece;
			
			shadowTransform.position = placement.pos;
			shadowRenderer.enabled   = true;
			shadowFilter.sharedMesh  = mesh.shadow;
			
			ActiveCount++;
			
			return this;
		}
	}
	
	
	public class PieceDictionary
	{
		private readonly PieceVariation[] pieces;
		private readonly Dictionary<int, int> pieceInfoMap;

		
		public PieceDictionary(PieceAndShadow[] meshes)
		{
			List<PieceVariation> pieceList = new List<PieceVariation>();
			pieceInfoMap = new Dictionary<int, int>();

			int index = 0;
			for (int i = 0; i < meshes.Length; i++)
			{
				PieceAndShadow mesh = meshes[i];
				
				int pieceInfo = mesh.id;
				if (!pieceInfoMap.ContainsKey(pieceInfo))
				{
					pieceInfoMap.Add(pieceInfo, index++);
					pieceList.Add(new PieceVariation(mesh));
				}
				else
				{
					pieceList[pieceInfoMap[pieceInfo]].AddMesh(mesh);
					//Debug.Log("Adding Variation");
				}
			}

			pieces = pieceList.ToArray();
		}

		
		public PieceAndShadow GetMesh(int pieceInfo)
		{
			if (!HasMeshFor(pieceInfo))
			{
				Debug.LogFormat("Can't find Meshes for " + pieceInfo.ToString().B_Purple());
				return null;
			}
			
			return pieces[pieceInfoMap[pieceInfo]].GetMesh();
		}

		
		public bool HasMeshFor(int pieceInfo)
		{
			return pieceInfoMap.ContainsKey(pieceInfo);
		}	
		
		
		private class PieceVariation
		{
			private readonly PieceAndShadow[] variations = new PieceAndShadow[10];
			private int count;

			public PieceVariation(PieceAndShadow mesh)
			{
				AddMesh(mesh);
			}

			public void AddMesh(PieceAndShadow mesh)
			{
				variations[count++] = mesh;
			}

			public PieceAndShadow GetMesh()
			{
				return count == 0 ? variations[0] : variations[Random.Range(0, count)];
			}
		}
	}
}
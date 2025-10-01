using System;
using GameModeStuff;
using GeoMath;
using LevelElements;
using UnityEngine;


namespace Generation
{
	[Serializable]
	public partial class Cell
	{
		private static readonly BoolSwitch HideOccluded = new("Dev/Hide Occluded", true);
		
		public readonly Content front = new(), back = new();
		
		public readonly SidePiece[] bgPieces = new SidePiece[1000];
		public int bgPieceCount;
		
		private readonly Bounds2D[] solids = new Bounds2D[100], edges = new Bounds2D[100];
		private int solidCount, edgeCount;
		
		
		public void Add(Element element)
		{
			(element.side == Side.Front ? front : back).Add(element);
			
			if(GameManager.IsCreator)
				RefreshOcclusion();
		}

		
		public void Remove(Element element)
		{
			(element.side == Side.Front ? front : back).Remove(element);
			
			if(GameManager.IsCreator)
				RefreshOcclusion();
		}
		
		
		public void Add(int bgMeshIndex, int side)
		{
			bgPieceCount = bgPieces.Add(new SidePiece(bgMeshIndex, side), bgPieceCount);
		}
			
		public void Remove(int bgMeshIndex, int side)
		{
			bgPieceCount = bgPieces.ShiftRemove(new SidePiece( bgMeshIndex, side), bgPieceCount);
		}
     

		public void Clear()
		{
			front.Clear();
			back.Clear();
			
			solidCount = 0;
		}


		private bool IsOccluded(Bounds2D bounds)
		{
			bool intersectsSolid = false;
			for (int i = 0; i < solidCount; i++)
				if (solids[i].Intersects(bounds))
				{
					intersectsSolid = true;
					break;
				}

			if (intersectsSolid)
			{
				for (int i = 0; i < edgeCount; i++)
					if (edges[i].Intersects(bounds))
						return false;
					
				return true;
			}

			return false;
		}


		public void AddBound(Bounds2D bounds, bool solid)
		{
			if(solid)
				solids[solidCount++] = bounds;
			else
				edges[edgeCount++] = bounds;
				
			RefreshOcclusion();
		}

		
		public void ClearOccluders()
		{
			solidCount = 0;
			edgeCount  = 0;
			RefreshOcclusion();
		}
		
		private void RefreshOcclusion()
		{
			front.SetOcclusion(this);
			back.SetOcclusion(this);
		}
		
		public void DrawBGBounds()
		{
			int camSide = (int)GameCam.CurrentSide.Sign;

			for (int i = 0; i < bgPieceCount; i++)
			{
				SidePiece piece = bgPieces[i];
				if(piece.side == camSide || piece.side == 0)
					PlacerMeshes.DrawPieceBounds(piece.index);
			}
		}
		
		
		public static Vector2Int ToCellPos(Vector2 position)
		{
			return new Vector2Int(Mathf.FloorToInt(position.x / Level.CellSize), 
				                  Mathf.FloorToInt(position.y / Level.CellSize));
		}
		
		public static Vector2Int ToCellPos(float x, float y)
		{
			return new Vector2Int(Mathf.FloorToInt(x / Level.CellSize), 
				                  Mathf.FloorToInt(y / Level.CellSize));
		}

		public static Bounds2D GetBounds(Vector2Int cellPos)
		{
			return new Bounds2D(cellPos * Level.CellSize).Add(cellPos * Level.CellSize + V2.one * Level.CellSize);
		}
	}


	public partial class Cell
	{
		[Serializable]
		public class Content
		{
			private readonly Item[]  items     = new Item[100];
			public  readonly Item[]  soloItems = new Item[100];
			public  readonly Track[] tracks    = new Track[100];

			public int itemCount, soloItemCount, trackCount;
			
			
			public void Add(Element element)
			{
				if (!Mask.IsTrack.Fits(element.elementType))
				{
					itemCount     = items.Add((Item) element, itemCount);
					soloItemCount = soloItems.Add((Item)element, soloItemCount);
				}
				else
				{
					Track track = (Track) element;
					trackCount = tracks.Add(track, trackCount); 
					
					for (int i = 0; i < track.itemCount; i++)
						itemCount = items.Add(track.items[i], itemCount);
				}
			}
			
			
			public void Remove(Element element)
			{
				if (!Mask.IsTrack.Fits(element.elementType))
				{
					Item item = (Item) element;
				
					if(item.parent == null)
						soloItemCount = soloItems.ShiftRemove(item, soloItemCount);
				
					itemCount = items.ShiftRemove(item, itemCount);
				}
				else
				{
					Track track = (Track) element;
					trackCount = tracks.ShiftRemove(track, trackCount); 
					
					for (int i = 0; i < track.itemCount; i++)
						itemCount = items.ShiftRemove(track.items[i], itemCount);
				}	
			}


			public void Clear()
			{
				itemCount = 0;
				soloItemCount = 0;
				trackCount = 0;
			}
			
			
			public void SetOcclusion(Cell cell)
			{
				for (int i = 0; i < itemCount; i++)
				{
					Item item = items[i];
					item.isOccluded = HideOccluded && (Mask.Hide.Fits(item.elementType) || cell.IsOccluded(item.bounds));
				}

				for (int i = 0; i < trackCount; i++)
				{
					Track track = tracks[i];
					int subCount = track.subBoundCount;

					bool allSubsOccluded = true;
					bool allSubsVisible  = true;
					for (int e = 0; e < subCount; e++)
					{
						bool subIsOccluded =  cell.IsOccluded(track.GetSubBound(e).bounds);
						track.subOcclusion[e] = subIsOccluded;

						if (!subIsOccluded)
							allSubsOccluded = false;
						else
							allSubsVisible = false;
					}

					track.allSubsOccluded = allSubsOccluded;
					track.allSubsVisible  = allSubsVisible;
				}
			}


			public int Hit(Bounds2D bounds2D)
			{
				int soloHit = 0;
				for (int i = 0; i < soloItemCount; i++)
					if (bounds2D.Intersects(soloItems[i].bounds))
						soloHit = Mathf.Max(soloHit, soloItems[i].elementType == elementType.Bouncy? 2 : 1);

				if (soloHit > 0)
					return soloHit;
				
				for (int i = 0; i < trackCount; i++)
				{
					Track track = tracks[i];
					
					if (bounds2D.Intersects(track.bounds))
						{
							bool allGood = false;
							for (int e = 0; e < track.itemCount; e++)
							{
								elementType itemType = track.items[e].elementType;

								switch (itemType)
								{
									case elementType.Stick:
									case elementType.Stick_SmallTip:
									case elementType.Flower:
									case elementType.Flower2:
									case elementType.Flower3:
									case elementType.DebugCube:
									case elementType.DebugCube2:
									case elementType.Branch:
										allGood = true;
										break;
								}

								if (allGood)
									break;
							}
								
							if(allGood)
								for (int e = 0; e < tracks[i].subBoundCount; e++)
									if (bounds2D.Intersects(tracks[i].GetSubBound(e).bounds))
										return 1;
						}
				}		

				return 0;
			}
		}

		[Serializable]
		public struct SidePiece
		{
			public int index;
			public int side;

			public SidePiece(int index, int side)
			{
				this.index = index;
				this.side  = side;
			}
		}
	}
	

	public struct MeshPlacement
	{
		public readonly Vector2 pos;
		public readonly int pieceInfo;
		public readonly HouseGen.CornerIDs corners;
		public readonly int side;

		public MeshPlacement(Vector2 pos, int pieceInfo,  HouseGen.CornerIDs corners, int side)
		{
			this.pos       = pos;
			this.pieceInfo = pieceInfo;
			this.corners   = corners;
			this.side      = side;
		}
	}
}
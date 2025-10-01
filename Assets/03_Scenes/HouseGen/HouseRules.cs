using System.Collections.Generic;
using GeoMath;
using LevelElements;
using UnityEngine;
using RectZone = HouseGen.RectZone;
using Cell = HouseGen.Cell;


public partial class HouseRules : MonoBehaviour
{
	[System.Serializable]
	public struct ZonePoint
	{
		public Vector2 pos;
		public int zoneID;
		public float multi;

		public ZonePoint(Vector2 pos, int zoneID, float radius)
		{
			this.pos = pos;
			this.zoneID = zoneID;

			multi = 1f / Mathf.Pow(radius, 2);
		}

		public ZonePoint(float multi, Vector2 pos, int zoneID)
		{
			this.pos = pos;
			this.zoneID = zoneID;

			this.multi = multi;
		}

		public float RadiusLerp(Vector2 point)
		{
			return (point - pos).sqrMagnitude * multi;
		}
	}


	[SerializeField] private HouseZone[] zones;

	private const int zoneSteps = 10;
	private const int zoneCount = zoneSteps * zoneSteps;
	private readonly ZonePoint[] zonePoints = new ZonePoint[zoneCount];

	private readonly List<Bounds2D> frontElements = new List<Bounds2D>(1000),
		backElements = new List<Bounds2D>(1000);

	private Vector2Int boundMin, boundMax;

	private readonly List<Item> items = new List<Item>(1000);
	private readonly List<Track> tracks = new List<Track>(1000);


	private const float cellSize = HouseGen.cellSize;

	private HouseGen gen;


	public void Init()
	{
		gen = GetComponent<HouseGen>();
	}


	public void SetupZones(Vector2 min, Vector2 max)
	{
		Vector2 step = (max - min) / (zoneSteps - 1);

		int index = 0;
		for (int x = 0; x < zoneSteps; x++)
		for (int y = 0; y < zoneSteps; y++)
		{
			Vector2 pos = min + new Vector2(x * step.x, y * step.y) + 
			              new Vector2(gen.random.Range(-1, 1f) * step.x * .4f, gen.random.Range(-1, 1f) * step.y * .4f);
			
			int zoneID = gen.random.Range(1, zones.Length) * 2 + 1;
			float radius = gen.random.Range(30, 60f);

			zonePoints[index++] = new ZonePoint(pos, zoneID, radius);
		}
		
		/*for (int i = 0; i < zoneCount; i++)
		{
			Vector2 pos = new Vector2(gen.random.Range(min.x, max.x), gen.random.Range(min.y, max.y));
			int zoneID = gen.random.Range(1, zones.Length) * 2 + 1;
			float radius = gen.random.Range(30, 60f);

			zonePoints[i] = new ZonePoint(pos, zoneID, radius);
		}*/
		
		//CreateZoneRects(min, max);
	}


	public void GetElementBounds()
	{
		if (GameManager.State != 0)
		{
			if (Level.Generator.IsNew || HouseGen.ForcedUpdate)
			{
				frontElements.Clear();
				backElements.Clear();

				items.Clear();

				List<Item> activeItems = Item.active;

				int iCount = activeItems.Count;
				for (int i = 0; i < iCount; i++)
				{
					Item item = activeItems[i];

					if (item.parent == null && Mask.Debug.Fits(item.elementType))
					{
						if (item.side.front)
							frontElements.Add(item.bounds);
						else
							backElements.Add(item.bounds);

						items.Add(item);
					}
				}

				tracks.Clear();
				List<Track> activeTracks = Track.active;

				int tCount = activeTracks.Count;
				for (int i = 0; i < tCount; i++)
				{
					Track track = activeTracks[i];

					for (int j = 0; j < track.itemCount; j++)
						if (Mask.Debug.Fits(track.items[j].elementType))
							goto GetSubbounds;

					continue;


					GetSubbounds:

					List<Bounds2D> bounds = track.side.front ? frontElements : backElements;

					for (int s = 0; s < track.subBoundCount; s++)
						bounds.Add(track.GetSubBound(s).bounds);

					tracks.Add(track);
				}
			}
		}
		else
		{
			frontElements.Clear();
			backElements.Clear();

			string[] boundLines = ResourceTxt.Read("AllBounds");

			for (int i = 0; i < boundLines.Length; i++)
			{
				string line = boundLines[i];
				if (line.Length == 0)
					continue;

				string[] parts = boundLines[i].Split('#');
				Bounds2D bounds = Bounds2D.GetViaString(parts[0]);


				if (int.Parse(parts[2]) == 0)
					frontElements.Add(bounds);
				else
					backElements.Add(bounds);
			}
		}
	}


	public void UpdateElementDepths()
	{
		int frontExtrudes = 0, backExtrudes = 0;
		int iCount = items.Count;
		for (int i = 0; i < iCount; i++)
		{
			Item item = items[i];

			int zone = gen.GetBoundZone(item.bounds, item.side.front);
			float depth = GetZoneDepth(zone);
			if (depth > 0)
				if (item.side.front)
					frontExtrudes++;
				else
					backExtrudes++;

			item.depth = depth;
		}

		if (false)// frontExtrudes > 0 || backExtrudes > 0)
			Debug.LogFormat("Extruded ".B_Black() + "{0} ".B_Purple() + "{1}".B_Teal(), frontExtrudes, backExtrudes);

		int tCount = tracks.Count;
		for (int i = 0; i < tCount; i++)
		{
			Track track = tracks[i];

			float depth = float.MaxValue;
			bool front = track.side.front;

			for (int s = 0; s < track.subBoundCount; s++)
				depth = Mathf.Min(depth, GetZoneDepth(gen.GetBoundZone(track.GetSubBound(s).bounds, front)));

			track.SetDepth(depth);
		}
	}


	private static float GetZoneDepth(int zone)
	{
		return zone != 0 && zone % 2 == 0 ? (.62f + .15f + zone * .06f) * 2.5f - Level.WallDepth : 0;
	}


	public RectZone GetRectZone(RectZone rectZone)
	{
		Vector2 pos = gen.GetRectZoneCenter(rectZone);

		int zoneID = 1;
		float radiusLerp = 1;
		for (int i = 0; i < zoneCount; i++)
		{
			ZonePoint zP = zonePoints[i];

			float checkRadiusLerp = zP.RadiusLerp(pos);
			if (checkRadiusLerp < radiusLerp)
			{
				radiusLerp = checkRadiusLerp;
				zoneID = zP.zoneID;
			}
		}

		return rectZone.SetZone(zoneID);
		
		/*float biggestArea = 0;
		int zoneID = 0;
		
		Bounds2D rect = gen.GetRectZoneBounds(rectZone);
		
		for (int i = 0; i < rectCount; i++)
		{
			float area = rect.OverlapArea(zoneRects[i]);

			if (biggestArea < area)
			{
				biggestArea = area;
				zoneID      = zonesIDs[i];
			}
		}
		
		return rectZone.SetZone(zoneID);*/
	}


	public bool InBroadBound(Vector2Int min, Vector2Int max)
	{
		return boundMin.x <= max.x && boundMin.y <= max.y && boundMax.x >= min.x && boundMax.y >= min.y;
	}


	///  Block Cells on back and front if intersecting Element  ///
	public void DetectBlockedCells()
	{
		boundMin = new Vector2Int(100000, 100000);
		boundMax = new Vector2Int(-100000, -100000);

		int fCount = frontElements.Count;
		for (int i = 0; i < fCount; i++)
		{
			Vector2Int min, steps;
			gen.BoundsToCellArea(frontElements[i], out min, out steps);
			gen.SetAreaCells(min, steps, Cell.FrontBlock);

			boundMin = Vector2Int.Min(boundMin, min);
			boundMax = Vector2Int.Max(boundMax, min + steps.Add(-1, -1));
		}

		int bCount = backElements.Count;
		for (int i = 0; i < bCount; i++)
		{
			Vector2Int min, steps;
			gen.BoundsToCellArea(backElements[i], out min, out steps);
			gen.SetAreaCells(min, steps, Cell.BackBlock);

			boundMin = Vector2Int.Min(boundMin, min);
			boundMax = Vector2Int.Max(boundMax, min + steps.Add(-1, -1));
		}
	}


	public HouseZone GetZone(int index)
	{
		return zones[index];
	}


	private readonly Bounds2D[] zoneRects = new Bounds2D[rectCount], newZoneRects = new Bounds2D[rectCount];
	private readonly int[] zonesIDs       = new int[rectCount];
	
	private const int rectCount = 64;

	
	private void CreateZoneRects(Vector2 min, Vector2 max)
	{
		int count = 0;
		zoneRects[count] = new Bounds2D(min).Add(max);
		count++;

		while (count < rectCount)
		{
			int newCount = 0;
			for (int i = 0; i < count; i++)
			{
				Bounds2D bound = zoneRects[i];
				
				Vector2 s = bound.Size;
				
				float a = gen.random.Range(.25f, .75f);

				if (s.x > s.y)
				{
					newZoneRects[newCount++] = bound.Fraction(0, a, 0, 1);
					newZoneRects[newCount++] = bound.Fraction(a, 1, 0, 1);
				}
				else
				{
					newZoneRects[newCount++] = bound.Fraction(0, 1, 0, a);
					newZoneRects[newCount++] = bound.Fraction(0, 1, a, 1);
				}
			}
			
			count = newCount;

			for (int i = 0; i < count; i++)
				zoneRects[i] = newZoneRects[i];
		}


		for (int i = 0; i < rectCount; i++)
			zonesIDs[i] = gen.random.Range(0, 3) * 2 + 1;
	}
}
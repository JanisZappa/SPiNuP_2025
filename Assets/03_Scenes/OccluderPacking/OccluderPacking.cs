using System.Collections.Generic;
using Generation;
using GeoMath;
using UnityEngine;


public class OccluderPacking : MonoBehaviour
{
	private readonly List<Bounds2D> pieceBounds = new List<Bounds2D>();
	
	private readonly List<TriLine> lines = new List<TriLine>();


	public static CellPosBounds[] cellPosBounds;
	
	
	private void Start ()
	{
		ParsePieces();
	}


	private void ParsePieces()
	{
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		
		cellPosBounds = new CellPosBounds[meshFilters.Length];
		
		for (int i = 0; i < meshFilters.Length; i++)
		{
			cellPosBounds[i] = GetCellPosBounds(meshFilters[i].mesh, meshFilters[i].GetComponent<MeshRenderer>().bounds.center);
			Destroy(meshFilters[i].gameObject);
		}
	}

	
	private CellPosBounds GetCellPosBounds(Mesh mesh, Vector3 center)
	{
		lines.Clear();
		pieceBounds.Clear();
		
		Vector3[] vertices  = mesh.vertices;
		float min = float.MaxValue, max = float.MinValue;
		for (int i = 0; i < vertices.Length; i++)
		{
			min = Mathf.Min(min, vertices[i].x);
			max = Mathf.Max(max, vertices[i].x);
		}
		
		int[] triangles = mesh.triangles;
		for (int i = 0; i < triangles.Length; i+=3)
		{
			Vector3 p1 = vertices[triangles[i]];
			Vector3 p2 = vertices[triangles[i + 1]];
			Vector3 p3 = vertices[triangles[i + 2]];

			if (Mathf.Approximately(p1.y, p2.y))						
				lines.Add(p1.x < p2.x? new TriLine(p1, p2, p3.y < p1.y) :  new TriLine(p2, p1, p3.y < p1.y));
			
			if(Mathf.Approximately(p2.y, p3.y))
				lines.Add(p2.x < p3.x? new TriLine(p2, p3, p1.y < p2.y) : new TriLine(p3, p2, p1.y < p2.y));
			
			if(Mathf.Approximately(p3.y, p1.y))
				lines.Add(p3.x < p1.x? new TriLine(p3, p1, p2.y < p3.y) : new TriLine(p1, p3, p2.y < p3.y));
		}


		while (lines.Count > 0)
		{
			TriLine a = lines[0], hitLine = null;
			lines.RemoveAt(0);

			Vector2 a1 = a.line.l1;
			Vector2 a2 = a.line.GetL2();
			
			float dist = float.MaxValue;
			bool hit = false;

			bool top = a.topEdge;
			
			int count = lines.Count;
			for (int e = 0; e < count; e++)
			{
				TriLine b = lines[e];
				Vector2 b1 = b.line.l1;
				Vector2 b2 = b.line.GetL2();
				
				if(a.topEdge == b.topEdge)
					continue;

				if(top && b1.y > a1.y || !top && b1.y < a1.y)
					continue;
				
				if (a1.x < b2.x && a2.x > b1.x)
				{
					float hitDist = top ? a1.y - b1.y : b1.y - a1.y;
					if (hitDist < dist)
					{
						dist = hitDist;
						hit = true;
						hitLine = b;
					}
				}
			}

			if (hit)
			{
				lines.Remove(hitLine);

				Vector2 b1 = hitLine.line.l1;
				Vector2 b2 = hitLine.line.GetL2();
				
				float overlapMin = Mathf.Max(a1.x, b1.x);
				float overlapMax = Mathf.Min(a2.x, b2.x);

				
				if (a1.x < overlapMin)
					lines.Add(new TriLine(a1, new Vector2(overlapMin, a1.y), a.topEdge));

				if (a2.x > overlapMax)
					lines.Add(new TriLine(new Vector2(overlapMax, a2.y), a2, a.topEdge));
			
			
				if (b1.x < overlapMin)
					lines.Add(new TriLine(b1, new Vector2(overlapMin, b1.y), hitLine.topEdge));
					
				if (hitLine.line.GetL2().x > overlapMax)
					lines.Add(new TriLine(new Vector2(overlapMax, b2.y), b2, hitLine.topEdge));
				
				
				pieceBounds.Add(new Bounds2D(new Vector2(overlapMin, a1.y)).Add(new Vector2(overlapMax, b1.y)));
			}
		}

		
	//  Merge Bounds that are next to each other  //
		while (true)
		{
			bool foundNeighbour = false;

			for (int i = 0; i < pieceBounds.Count; i++)
			{
				Bounds2D a = pieceBounds[i];
					
				for (int e = 0; e < pieceBounds.Count; e++)
				{
					if(i == e)
						continue;

					Bounds2D b = pieceBounds[e];

					if (Mathf.Approximately(a.minY, b.minY) && Mathf.Approximately(a.maxY, b.maxY) &&
					    (Mathf.Approximately(a.minX, b.maxX) || Mathf.Approximately(a.maxX, b.minX)))
					{
						foundNeighbour = true;
						
						pieceBounds.Remove(a);
						pieceBounds.Remove(b);
						
						pieceBounds.Add(new Bounds2D(new Vector2(Mathf.Min(a.minX, b.minX), a.minY)).
							                         Add(new Vector2(Mathf.Max(a.maxX, b.maxX), a.maxY)));

						break;
					}
				}

				if (foundNeighbour)
					break;
			}

			if (!foundNeighbour)
				break;
		}
	
		Vector2Int cellPos = Cell.ToCellPos(center);
		
		return new CellPosBounds(cellPos, pieceBounds.ToArray());
	}


	/*public static void AddOccludersToCells()
	{
		if(cellPosBounds != null)
			for (int i = 0; i < cellPosBounds.Length; i++)
				Level.AddOccluderToCell(cellPosBounds[i].pos, cellPosBounds[i].bounds);
	}*/


	private class TriLine
	{
		public Line line;
        public readonly bool topEdge;

		public TriLine(Vector2 l1, Vector2 l2, bool topEdge)
		{
			line = new Line(l1, l2);
			this.topEdge = topEdge;
		}
	}


	public class CellPosBounds
	{
		public Vector2Int pos;
		public readonly Bounds2D[] bounds;

		public CellPosBounds(Vector2Int pos, Bounds2D[] bounds)
		{
			this.pos = pos;
			this.bounds = bounds;
		}

		public void Draw()
		{
			Color c = (pos.x + pos.y) % 2 == 0 ? COLOR.red.tomato : COLOR.purple.orchid;
			
			for (int i = 0; i < bounds.Length; i++)
				bounds[i].Pad(-.075f).Draw().SetColor(c).SetDepth(Z.W10).Fill(.3f, true);
		}
	}
}

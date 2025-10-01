using System.Collections.Generic;
using GeoMath;
using LevelElements;
using UnityEngine;


public static class LevelCheck 
{
	private static readonly int[] Highlight = new int[Item.TotalCount + Track.TotalCount], 
		                          Overlap   = new int[Item.TotalCount + Track.TotalCount];
	private static int Frame;

	public static bool Highlighting { get { return ClosestElement != null; } }
	
	public static Element ClosestElement;
	public static Vector2 ElementPoint;

	public static Link ClosestLink;
	public static Vector2 LinkPoint;
	

	public static ElementMask Filter { get { return Creator.currentFilter; } }
	public static bool ItemCheck { get { return Filter == Mask.IsItem; } }
	private static readonly Vector2[] positions = new Vector2[short.MaxValue];
	
	
	public static void CreatorUpdate()
	{
		Frame = Time.frameCount;

	//  Highlight  //
		if (UI_Manager.HitUI || Input.GetKey(KeyCode.LeftAlt))
		{
			ClosestElement = null;
			ClosestLink    = Link.None;
		}
		else
		{
			ClosestElement = Search.ClosestElement(Level.HitPoint, GTime.Now, GameCam.CurrentSide, Filter, out ElementPoint);
			if (ClosestElement != null)
				Highlight[ClosestElement.ID] = Frame;

			if (LinkEdit.LinkMode)
			{
				Vector2 hitPoint = Level.HitPoint;
				ClosestLink    = Link.None;
				float   distance = Mth.IntPow(Level.CellSize * .5f, 2);

				if (LinkEdit.Creating)
					return;

				List<Link> links = GameCam.CurrentSide.front ? Link.frontLinks : Link.backLinks;
				int count = links.Count;
				for (int i = 0; i < count; i++)
				{
					Link link = links[i];
					Vector2 point = new Line(link.a.GetPos(GTime.Now), link.b.GetPos(GTime.Now)).ClosestPoint(hitPoint);
					float dist = (point - hitPoint).sqrMagnitude;
					if (dist < distance)
					{
						distance      = dist;
						ClosestLink = link;
						LinkPoint     = point;
					}
				}
			}
		}
		
		
		
		
	//  Overlap  //
		for (int i = 0; i < Level.itemCount; i++)
			positions[i] = Level.items[i].GetPos(GTime.Now);

		bool itemCheck = ItemCheck;
		for (int i = 0; i < Level.itemCount; i++)
			if (Filter.Fits(Level.items[i].elementType))
			{
				float aR   = Level.items[i].radius;
				Side  side = Level.items[i].side;
				
				for (int e = 0; e < Level.itemCount; e++)
				{
					if(i == e || side != Level.items[e].side)
						continue;

					bool itemVSItem = itemCheck && Mask.IsItem.Fits(Level.items[e].elementType);
					
					float bR = Level.items[e].radius;
					float rP = aR + bR + (itemVSItem ? Level.PlacementRadius * 2 : 0);
					
					if ((positions[i] - positions[e]).sqrMagnitude < Mth.IntPow(rP, 2))
					{
						Overlap[Level.items[i].ID] = Frame;
						Overlap[Level.items[e].ID] = Frame;
					}
				}
			}
		
	}


	public static bool HighLighted(Element element)
	{
		return Highlight[element.ID] == Frame;
	}
	
	
	public static bool Overlapping(Element element)
	{
		return element != null && Overlap[element.ID] == Frame;
	}


	public static Color GetColor(Element element)
	{
		if(ElementEdit.element == element)
			return COLOR.purple.orchid;

		Color color = element.elementType.DebugColor();
		
		
		if(Overlapping(element))
			color = COLOR.red.hot;

		if (HighLighted(element))
			color = color.Multi(1.7f);

		return color;
	}
}

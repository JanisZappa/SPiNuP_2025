using Clips;
using LevelElements;
using UnityEngine;


public static class ItemCheck 
{
	public static bool FirstHit(float start, float end, float stepLength, Clip clip, Item item, float checkDist, out float checkTime)
	{
		checkTime = start;
		
		while (checkTime < end)
		{
			Vector2 player = clip.BasicPlacement(checkTime).pos;
			Vector2 getPos = item.GetPos(checkTime);
                    
			float sqrDist = (player - getPos).sqrMagnitude;
			if (sqrDist <= checkDist)
			{
				float step = stepLength * .5f;
				checkTime -= step;
				for (int e = 0; e < 5; e++)
				{
					player = clip.BasicPlacement(checkTime).pos;
					getPos = item.GetPos(checkTime);
                            
					sqrDist = (player - getPos).sqrMagnitude;
                            
					step *= .5f;
					checkTime += sqrDist <= checkDist ? -step : step;
				}

				return true;
			}
                    
			checkTime += stepLength;
		}
		
		return false;
	}
}

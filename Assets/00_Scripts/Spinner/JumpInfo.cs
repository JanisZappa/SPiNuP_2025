using Clips;
using Future;
using GeoMath;
using LevelElements;
using UnityEngine;
using UnityEngine.Profiling;


public class JumpInfo 
{
//  Search Consts  //
	private const int   MaxProxItems = 200;
	private const int   SliceSteps   = 4;
	public const float  StepLength   = 1f / SliceSteps;
	public  const float SearchRange  = 4;
	
	
	public int proxCount;
	public readonly ProxItem[] proxItems = CollectionInit.Array<ProxItem>(MaxProxItems);
	
	private readonly PathSlice pathSlice = new PathSlice();
		
	#region Static
	static JumpInfo()
	{
		itemProxList  = new ProxItem[Item.TotalCount];
		checkItems = new Item[MaxProxItems];
		trackCallCheck  = new int[Item.TotalCount + Track.TotalCount];
		proxCheck  = new int[Item.TotalCount];
	}
	
	private static readonly ProxItem[] itemProxList;	
    private static readonly Item[] checkItems;
    private static int    checkCount;
		
    private static int callID;
    private static readonly int[] trackCallCheck;
		
    private static int proxID;
    private static readonly int[] proxCheck;
    #endregion
    
    public void Analyze(Jump jump)
    {
	    Profiler.BeginSample("JumpInfo_Analize");
	    
        float searchTime = jump.WillHit ? jump.duration : 6;

		float radius         = jump.spinner.size.y * SearchRange;
		float radiusSqr      = radius * radius;
		float eventTime      = jump.startTime;
		Vector2 startPos     = jump.startPos;
		Vector2 lastMidPoint = startPos;

		proxID++;
		proxCount = 0;
		
		pathSlice.Setup(SliceSteps, startPos, jump.jumpV, radius, jump.startSide, Mask.AnyThing);

		Item jumpStick = jump.stick.Item;
		Item grabStick = jump.WillConnect ? jump.nextStick.Item : null;

		bool jumpUp = jump.jumpV.y > 0;
		bool grabUp = jump.WillConnect && pathSlice.flyPath.GetMV(searchTime).y > 0;

		float apexTime = pathSlice.flyPath.apexTime;

		int   slice = -1;
		while(true)
		{
			slice++;
			
			pathSlice.CheckSlice(pathSlice.GetBounds(slice, StepLength));
			if (pathSlice.ContainsNoItem)
			{
				float time = StepLength * SliceSteps * (slice + 1);
				if (time >= searchTime)
					break;
				
				lastMidPoint = pathSlice.flyPath.GetPos(time);
				continue;
			}

			
			for (int step = 0; step < SliceSteps; step++)
			{
				int stepNumber   = slice * SliceSteps + step;
				float flightTime = StepLength * stepNumber;
				float time       = eventTime + flightTime;

				if (flightTime >= searchTime || proxCount == MaxProxItems)
				{
					if(proxCount == MaxProxItems)
						Debug.Log("Nooo");
					goto SearchOver;
				}
					

				Vector2 midPoint = pathSlice.flyPath.GetPos(flightTime);
				Vector2 motion   = pathSlice.flyPath.GetMV(flightTime);
				bool motionUp = motion.y > 0;

				Bounds2D b = new Bounds2D(midPoint).Add(lastMidPoint).Pad(radius);
				
				/*if (flightTime - StepLength <= apexTime && flightTime >= apexTime)
					Bounds2D.Helper.Add(pathSlice.flyPath.GetPos(apexTime));*/

				//Bounds2D.Helper.Pad(radius);
				
			//  Collect Solo Items  //
				checkCount = 0;
				for (int i = 0; i < pathSlice.itemCount; i++)
				{
					Item item = pathSlice.items[i];
					
					if ((!item.Equals(jumpStick) || jumpUp != motionUp) && 
					    (!item.Equals(grabStick) || grabUp != motionUp) &&
					    item.bounds.Intersects(b))
						checkItems[checkCount++] = item;
				}
				
			//  Collect Track Items  //	
				callID++;
				for (int i = 0; i < pathSlice.trackCount; i++)
				{
					Track track = pathSlice.trackSubBounds[i].track;
					float minDist = radius + track.maxItemRadius;
					
					float trackDist = (track.GetClosestPoint(midPoint) - midPoint).sqrMagnitude;
					if (trackDist > minDist * minDist)
						continue;
					
					int gotItems = 0;

					for (int subBound = 0; subBound < track.subBoundCount; subBound++)
					{
						if (pathSlice.trackSubBounds[i].validSubBounds[subBound])
						{
							Track.SubBound sB = track.GetSubBound(subBound);
							if (sB.Intersects(b))
							{
								if (trackCallCheck[track.ID] != callID)
								{
									track.FillSubBounds(time, Mask.AnyThing);
									trackCallCheck[track.ID] = callID;
								}

								for (int subI = 0; subI < sB.itemCount; subI++)
								{
									Item item = sB.items[subI];

									if (trackCallCheck[item.ID] != callID &&
									    (!item.Equals(jumpStick) || jumpUp != motionUp) &&
									    (!item.Equals(grabStick) || grabUp != motionUp))
									{
										checkItems[checkCount] = item;
										checkCount++;
										gotItems++;
										trackCallCheck[item.ID] = callID;
									}
								}
							}
						}

						if (gotItems >= track.itemCount)
							break;
					}
				}
					
					
			//  Check Misses  //
					for (int i = 0; i < checkCount; i++)
					{
						Item    item    = checkItems[i];
						Vector2 itemPos = item.GetPos(time);
						Vector2 charDir = new Vector2(midPoint.x - itemPos.x, midPoint.y - itemPos.y);
						
						if(charDir.sqrMagnitude <= radiusSqr)
							if (proxCheck[item.ID] != proxID || !itemProxList[item.ID].IsTheSame(time, motionUp))
							{
								/*if(proxCheck[item.ID] == proxID)
									Debug.Log("New Miss For " + item.ID + " " + itemProxList[item.ID].endTime + " | " + time);*/
								
								   proxCheck[item.ID] = proxID;
								itemProxList[item.ID] = proxItems[proxCount];
								
								proxItems[proxCount++].New(item, time, charDir, motion);
							}
							else
								itemProxList[item.ID].Update(time, charDir, motion);


						if (proxCount == MaxProxItems)
						{
							Debug.Log("Too many misses");
							break;
						}
					}

					lastMidPoint = midPoint;
				}
			}
			
			SearchOver: ;
			
			//Debug.Log(proxCount);
			
			Profiler.EndSample();
    }
}

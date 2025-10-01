using System.Collections.Generic;
using Anim;
using Clips;
using GeoMath;
using LevelElements;
using UnityEngine;
using UnityEngine.Profiling;


namespace Future
{
    public static partial class Prediction
    {
        static Prediction()
        {
            pathSlices = new PathSlice[3];
            for(int i = 0; i < 3; i++)
                pathSlices[i] = new PathSlice();

            MovedItems.Init();
            
            checkItems = new Item[200];
            
            callCheck = new int[Item.TotalCount + Track.TotalCount];
        }
        
        public const float CheckAngle = 6; //16;
        private const int SliceSteps = 10;
        
        private static readonly PathSlice[] pathSlices;
        private static readonly PathSlice mainSlice = new PathSlice();
        
        private static Spinner  spinner;
        private static Vector2  startPos, jumpV;
        private static float    startAngle, eventTime, spin, hitTime, weightLerp, spinAccel;
        private static Side     side;
        private static StickID  oldStick;
        private static ClipType nextClip;
        private static Item     hitItem;
        private static FlyPath  flyPath;
        
        private static readonly Item[] checkItems;
        private static int    checkCount;
        
        private static int itemCallID, trackCallID;
        private static readonly int[] callCheck;
        
        private static bool Done { get { return nextClip != 0; }}
        
        
        public static Clip JumpCheck(float eventTime, Spinner spinner, Vector2 startPos, float weightLerp, Quaternion startRot, Vector2 jumpV, float spin, StickID oldStick)
        {
            Prediction.spinner    = spinner;
            Prediction.weightLerp = weightLerp;
            
                       startAngle = startRot.eulerAngles.z.Wrap(-180, 180);
            Prediction.startPos   = startPos + Swing.GetWeightForce(startRot, spin, weightLerp, oldStick.hands);
            Prediction.eventTime  = eventTime;
                       side       = oldStick.Item.side;
            Prediction.oldStick   = oldStick;

            
            Prediction.jumpV      = jumpV;
            Prediction.spin       = spin;
            hitTime = 0;
            nextClip = 0;
            hitItem = null;
            
            return GetPredictionClip();
        }

        
        private static Clip GetPredictionClip()
        {
            if (DebugPrediction)
                ResetShow();
            
            Profiler.BeginSample("Prediction.GetPredictionClip");
            
        //  Search Variables  //
            float   radius  = spinner.size.y * .5f;
            Vector2 stickMV = oldStick.Item?.GetMV(eventTime) * .4f ?? V2.zero;
            
            for (int dir = 0; dir < 3; dir++)
                pathSlices[dir].Setup(SliceSteps, startPos, (jumpV.Rot(CheckAngle * -.5f + CheckAngle * .5f * dir) + stickMV), radius, side, Mask.IsItem);
            
            jumpV = pathSlices[1].flyPath.startMV;
            mainSlice.Setup(SliceSteps, startPos, jumpV, radius, side, Mask.IsItem);
            
            
        //  Prediction Steps  //
            float searchTime     = Mathf.Min(6, GPhysics.Get_SpinStopTime(spin));
            int   searchSteps    = Mathf.CeilToInt(GPhysics.StepsPerSecond * searchTime);
            int   numberOfSlices = Mathf.CeilToInt((float) searchSteps / SliceSteps);

            Vector2 halfBody = new Vector2(0, (spinner.size.y - spinner.size.x) * .5f);

            
            for (int slice = 0; slice < numberOfSlices; slice++)
            {
                if (Done) break;

                Bounds2D b = pathSlices[0].GetBounds(slice, GPhysics.TimeStep).
                         Add(pathSlices[1].GetBounds(slice, GPhysics.TimeStep)).
                         Add(pathSlices[2].GetBounds(slice, GPhysics.TimeStep));
                
                mainSlice.CheckSlice(b);
                    
                if(mainSlice.ContainsNoItem)
                    continue;

                for (int step = 0; step < SliceSteps; step++)
                {
                    int stepCount = slice * SliceSteps + step;
                    if (Done || stepCount == searchSteps)
                        break;

                    MovedItems.Reset();
                    float flightTime = stepCount * GPhysics.TimeStep;
                    float time       = eventTime + flightTime;

                    trackCallID++;
                    for (int dir = 0; dir < 3; dir++)
                    {
                        if (Done) break;

                        Vector2 lastMid  = pathSlices[dir].lastMidpoint;
                        Vector2 midPoint = pathSlices[dir].flyPath.GetPos(flightTime);
                        Line move = new Line(lastMid, midPoint);
                        
                        if(DebugPrediction)
                            checkLines[dir].Add(move);
                        
                        pathSlices[dir].lastMidpoint = midPoint;

                        Bounds2D midBound = new Bounds2D(midPoint).Add(lastMid).Pad(radius);

                    //  Collect Solo Items  //
                        checkCount = 0;
                        for (int i = 0; i < mainSlice.itemCount; i++)
                            if ((!oldStick.Item.Equals(mainSlice.items[i]) || stepCount > 2) && mainSlice.items[i].bounds.Intersects(midBound))
                                checkItems[checkCount++] = mainSlice.items[i];
                        
                    //  Collect Track Items  //
                        itemCallID++;
                        for (int i = 0; i < mainSlice.trackCount; i++)
                        {
                            Track track = mainSlice.trackSubBounds[i].track;

                            float minDist = radius + track.maxItemRadius;

                            float trackDist = (track.GetClosestPoint(midPoint) - midPoint).sqrMagnitude;

                            if (trackDist > minDist * minDist)
                                continue;
                            
                            int gotItems = 0;

                            for (int subBound = 0; subBound < track.subBoundCount; subBound++)
                            {
                                if (mainSlice.trackSubBounds[i].validSubBounds[subBound])
                                {
                                    Track.SubBound sB = track.GetSubBound(subBound);
                                    if (sB.Intersects(midBound))
                                    {
                                        if (callCheck[track.ID] != trackCallID)
                                        {
                                            track.FillSubBounds(time, Mask.IsItem);
                                            callCheck[track.ID] = trackCallID;
                                        }

                                        for (int subI = 0; subI < sB.itemCount; subI++)
                                        {
                                            Item item = sB.items[subI];
                                            if ((!oldStick.Item.Equals(item) || stepCount > 2) &&
                                                callCheck[item.ID] != itemCallID)
                                            {
                                                checkItems[checkCount++] = item;
                                                gotItems++;
                                                callCheck[item.ID] = itemCallID;
                                            }
                                        }
                                    }
                                }

                                if (gotItems >= track.itemCount)
                                    break;
                            }
                        }

                    //  Move them if necessary  //
                        for (int i = 0; i < checkCount; i++)
                            if(!MovedItems.AlreadyMoved(checkItems[i], dir))
                                MovedItems.Add(checkItems[i], time);


                        for (int i = 0; i < MovedItems.Count; i++)
                        {
                            MovedItems movedItem = MovedItems.Get(i);

                        //  If the Items wasn't moved for this dir -> Skip  //
                            if (MovedItems.moved[movedItem.item.ID][dir] != MovedItems.checkID)
                                continue;

                            Item    item       = movedItem.item;
                            float   minDistSqr = Mth.IntPow(radius + item.radius, 2);

                            if (move.LineIsCloserSqr(movedItem.move, minDistSqr))
                            {
                                if (Mask.CanBeGrabbed.Fits(item.elementType))
                                    nextClip = ClipType.Swing;
                                else
                                {
                                    float checkAngle = startAngle + GPhysics.Get_SpinAngle_Deg(spin, flightTime);

                                    Vector2 bodyLine = halfBody.Rot(checkAngle);
                                    Vector2 feetPos  = midPoint - bodyLine;
                                    Vector2 handPos  = midPoint + bodyLine;

                                    float itemRadiusSqr = Mth.IntPow(spinner.size.x * .5f + item.radius, 2);
                                    if (new Line(feetPos, handPos).SqrDistance(movedItem.move.l1) <= itemRadiusSqr)
                                    {
                                        nextClip = ClipType.Bump;
                                        
                                        if (DebugPrediction)
                                            showBump = true;
                                    }
                                }

                                if (Done)
                                {
                                    hitItem = item;
                                    hitTime = flightTime;
                                    jumpV   = pathSlices[dir].flyPath.startMV;
                                    flyPath = pathSlices[dir].flyPath;

                                    if (DebugPrediction)
                                        bestDir = dir;
                                    
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            
            Profiler.EndSample();
            
            switch(nextClip)
            {
                case ClipType.Swing:    return GetSwingClip();
                case ClipType.Bump:     return GetBumpClip();
                default:                return GetFailClip();
            }
        }
        
        
        private static Clip GetSwingClip()
        {
        //  Increase Precision  //
            float sqrtRadius = Mth.IntPow(spinner.ConnectRadius(hitItem), 2);

            const float steps = 10;
            const float tinyStep = GPhysics.TimeStep / steps;

            for (int i = 0; i < steps; i++)
            {
                float checkTime = hitTime - GPhysics.TimeStep + i * tinyStep;
                Vector2 midPoint   = flyPath.GetPos(checkTime);
                Vector2 stickPos   = hitItem.GetLagPos(eventTime + checkTime);
                Vector2 midToStick = new Vector2(stickPos.x - midPoint.x, stickPos.y - midPoint.y);
                if (midToStick.sqrMagnitude <= sqrtRadius)
                {
                    hitTime = checkTime;
                    break;
                }
            }
            
            return oldStick.Item != null?
                Clip.Get_Jump(spinner, eventTime, startAngle, jumpV, spin, hitTime, oldStick, weightLerp, nextClip, side) :
           Clip.Get_AirLaunch(spinner, eventTime, startAngle, jumpV, spin, hitTime, startPos, nextClip, side);
        }


        private static Clip GetBumpClip()
        {
        //  Increase Precision  //
            Vector2 collisionLine = new Vector2(0, spinner.size.y - spinner.size.x);
            float   tinyStep      = GPhysics.TimeStep * .5f;
            float   flightTime    = hitTime;
            float   itemRadiusSqr = Mth.IntPow(spinner.size.x * .5f + hitItem.radius, 2);
            flightTime -= tinyStep;
            
            for(int u = 0; u < 5; u++ )
            {
                tinyStep *= .5f;
                
                Vector2 midPoint   = flyPath.GetPos(flightTime);
                float   checkAngle = startAngle + GPhysics.Get_SpinAngle_Deg(spin, flightTime);
                Vector2 bodyLine   = collisionLine.Rot(checkAngle);
                Vector2 handPos    = midPoint + bodyLine * .5f;
                Vector2 feetPos    = midPoint - bodyLine * .5f;
                Vector2 itemPos    = hitItem.GetLagPos(eventTime + flightTime);
                
                if (new Line(feetPos, handPos).SqrDistance(itemPos) <= itemRadiusSqr)
                {
                    hitTime = flightTime;
                    flightTime -= tinyStep;
                    
                    if (DebugPrediction)
                    {
                        b1 = feetPos;
                        b2 = handPos;
                    }
                }
                else
                    flightTime += tinyStep;
            }
            
            
            
            return oldStick.Item != null?
                Clip.Get_Jump(spinner, eventTime, startAngle, jumpV, spin, hitTime, oldStick, weightLerp, nextClip, side) :
                Clip.Get_AirLaunch(spinner, eventTime, startAngle, jumpV, spin, hitTime, startPos, nextClip, side);
        }


        private static Clip GetFailClip()
        {
            return oldStick.Item != null ? 
                Clip.Get_Jump(spinner, eventTime, startAngle, jumpV, spin, hitTime, oldStick, weightLerp, nextClip, side) : 
                Clip.Get_AirLaunch(spinner, eventTime, startAngle, jumpV, spin, hitTime, startPos, nextClip, side);
        }
        
        
        public class MovedItems
        {
            static MovedItems()
            {
                items = new MovedItems[100];
            }
            
            public  static int Count;
            private static readonly MovedItems[] items;
            public  static int[][] moved;

            public static int checkID;
            
            
            public static void Init()
            {
                for (int i = 0; i < items.Length; i++)
                    items[i] = new MovedItems();
                
                moved = new int[Item.TotalCount][];
                for (int i = 0; i < Item.TotalCount; i++)
                    moved[i] = new int[3];
            }


            public static void Reset()
            {
                checkID++;
                Count = 0;
            }
            
            
            public static MovedItems Get(int index){ return items[index]; }
            
            
            public static void Add(Item item, float time)
            {
                items[Count++].Set(item, time);
            }


            public static bool AlreadyMoved(Item item, int dir)
            {
                bool movedAlready = moved[item.ID][0] == checkID || moved[item.ID][1] == checkID || moved[item.ID][2] == checkID;
                moved[item.ID][dir] = checkID;
                return movedAlready;
            }

            
            public Item item;
            public Line move;

            private void Set(Item item, float time)
            {
                this.item = item;
                move = new Line(item.GetLagPos(time), item.GetLagPos(time - GPhysics.TimeStep));
            }
        }
    }

    
    public class PathSlice
    {
        private int steps;
        
        public  Vector2 lastMidpoint;
        private float   radius;
        private Side    side;

        public FlyPath flyPath;

        public readonly Item[] items = new Item[400];
        public readonly TrackSubBounds[] trackSubBounds = CollectionInit.Array<TrackSubBounds>(50);
        public int itemCount, trackCount;
    
        public bool ContainsNoItem { get { return itemCount == 0 && trackCount == 0; } }

        private ElementMask filter;
        
        private Bounds2D bounds;
        
        
        public void Setup(int steps, Vector2 startPos, Vector2 jumpV, float radius, Side side, ElementMask filter)
        {
            this.steps = steps;
            
            flyPath = new FlyPath(startPos, jumpV);
            
            this.radius   = radius;
            this.side     = side;

            this.filter   = filter;
        }

        
        public void CheckSlice(Bounds2D checkBounds)
        {
        //  Get Solo Items  //
            Search.ItemsAndTracks(checkBounds, side, filter);

            itemCount = Search.itemCount;
            for (int i = 0; i < itemCount; i++)
                items[i] = Search.boundItems[i];
        
        
        //  Get Tracks & Sub Bounds  // 
            trackCount = 0;
            
            for (int i = 0; i < Search.trackCount; i++)
            {
                Track track = Search.boundTracks[i];
                
                trackSubBounds[trackCount].GetValidSubBounds(track, checkBounds);

            //  If all SubBounds are overlapping -> Just take all valid Items directly  //
                if (trackSubBounds[trackCount].allAreIntersecting)
                {
                    for (int e = 0; e < track.itemCount; e++)
                        if (filter.Fits(track.items[e].elementType))
                            items[itemCount++] = track.items[e];
                }
                else
                    trackCount++;
            }
        }


        public Bounds2D GetBounds(int sliceNumber, float stepLength)
        {
            float timeA = Mathf.Max(0, sliceNumber * steps * stepLength - stepLength);
            float timeB = (sliceNumber + 1) * steps * stepLength;
            
            lastMidpoint = flyPath.GetPos(timeA);
            Vector2 posB = flyPath.GetPos(timeB);
            
            
        //  Fill Jump Slice Bounds  //
            bounds = new Bounds2D(lastMidpoint).Add(posB);

            if (timeA <= flyPath.apexTime && timeB >= flyPath.apexTime)
                bounds = bounds.Add(flyPath.GetPos(flyPath.apexTime));

            bounds = bounds.Pad(radius);

            return bounds;
        }


        public class TrackSubBounds
        {
            public Track track;
            public readonly bool[] validSubBounds = new bool[20];

            public bool allAreIntersecting;

            public void GetValidSubBounds(Track track, Bounds2D checkBounds)
            {
                this.track = track;

                allAreIntersecting = true;
                if (track.subBoundCount == 1)
                    return;

                for (int i = 0; i < track.subBoundCount; i++)
                {
                    validSubBounds[i] = track.GetSubBound(i).Intersects(checkBounds);

                    if (!validSubBounds[i])
                        allAreIntersecting = false;
                }
            }
        }
    }


    public static partial class Prediction
    {
        private static readonly BoolSwitch DebugPrediction = new("Dev/Show Prediction", false);

        private static readonly List<Line>[] checkLines = {new(100), new(100), new(100)};
        private static float checkRadius, checkThickness;

        private static bool showBump;
        private static Vector2 b1, b2;
        private static int bestDir;
        
        
        private static void ResetShow()
        {
            for (int i = 0; i < 3; i++)
                checkLines[i].Clear();
           
            checkRadius = spinner.size.y * .5f;
            checkThickness = spinner.size.x * .5f;
            showBump = false;
            bestDir = 1;
        }


        public static void ShowPrediction()
        {
            if (!DebugPrediction)
                return;

            for (int i = 0; i < 3; i++)
            {
                int length = checkLines[i].Count;
                
                if(length == 0)
                    continue;
                
                Color c;
                switch (i)
                {
                    default: c = Color.red;   break;
                     case 1: c = Color.white; break;
                     case 2: c = Color.green; break;
                }

                for (int e = 0; e < length; e++)
                    checkLines[i][e].DrawShell(checkRadius).SetColor(c.A(.15f)).SetDepth(Z.W05);

                if (i == bestDir)
                    DRAW.Circle(checkLines[i][length - 1].GetL2(), checkRadius * .5f, 24).SetColor(c.A(.3f)).SetDepth(Z.W05).Fill(.15f);
            }

            if (showBump)
                new Line(b1, b2).DrawShell(checkThickness, true).SetColor(Color.yellow).SetDepth(Z.W05);
         }
    }
}
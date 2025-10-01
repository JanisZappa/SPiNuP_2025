using System;
using System.Collections.Generic;
using System.Text;
using ActorAnimation;
using Clips;
using Future;
using GeoMath;
using LevelElements;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;


public static class ActorAnimator 
{
    public class ActorList
    {
        static ActorList()
        {
            allActors  = new Actor[Item.TotalCount];
            actorIndex = new int[Item.TotalCount];
            setups     = new int[Item.TotalCount];
        }
        
        private readonly elementType itemType;

        private readonly Actor[] active;
        private readonly Stack<Actor> inactive, idle;

        public static readonly Actor[] allActors;
        private static readonly int[] actorIndex;

        private readonly int capacity;
        private int activeCount;

        private int  maxActiveActors, requestCount;
        private bool ExeededMax { get { return requestCount > maxActiveActors; }}

        private static readonly int[] setups;
            
        
        public ActorList(GameObject prefab, Transform parent, elementType itemType, int amount)
        {
            this.itemType = itemType;
            capacity = amount;
            
            inactive = new Stack<Actor>(amount);
            active   = new Actor[amount];
            idle     = new Stack<Actor>(amount);
            
            for (int i = 0; i < amount; i++)
            {
                Actor actor = Object.Instantiate(prefab, -V3.up * 10000, Rot.Zero, parent).GetComponent<Actor>();
                actor.Setup();
                inactive.Push(actor);
            }
        }


        private void EnableActor(Item item)
        {
            Actor actor = idle.Count > 0 ? idle.Pop() : inactive.Pop();

            actorIndex[item.ID] = activeCount;
            active[activeCount++] = actor;
            actor.SetItem(item);
            allActors[item.ID] = actor;

            maxActiveActors = Mathf.Max(maxActiveActors, activeCount);

            setups[item.ID] = Time.frameCount;
        }


        public void DisableIdle()
        {
            int count = idle.Count;
            for (int i = 0; i < count; i++)
            {
                Actor actor = idle.Pop();
                inactive.Push(actor);

                int itemID = actor.item.ID;
                allActors[itemID].SetItem(null);
                allActors[itemID] = null;
            }
        }
        
        
        private void SetIdleActor(Item item)
        {
            idle.Push(allActors[item.ID]);

            int index = actorIndex[item.ID];

            active[index] = active[activeCount - 1];
            actorIndex[active[index].item.ID] = index;
              
            activeCount--;
        }

        
        public void SetTransform()
        {
            for (int i = 0; i < activeCount; i++)
                active[i].SetTransform(setups[active[i].item.ID] == Time.frameCount);
        }


        public void Add(Item item)
        {
            if (activeCount < capacity)
                EnableActor(item);
            else
                Level.CantShowItem(item);
                
            requestCount++;
        }


        public void Remove(Item item)
        {
            SetIdleActor(item);
            requestCount--;
        }


        public void ResetAll()
        {
            for (int i = 0; i < activeCount; i++)
            {
                Actor actor = active[i];
                actor.SetItem(null);
                inactive.Push(actor);
            } 
                
            activeCount = 0;
        }


        public void AddInfo(StringBuilder stringBuilder, bool last)
        {
            if (ExeededMax)
                stringBuilder.Append(FancyString.B_Start("yellow"));

            stringBuilder.Append(itemType.Name().PadRight(Mathf.Max(ElementTypeExt.LongestItemName, ElementTypeExt.LongestFluffName) + 1)).
                Append(activeCount.PrepString().PadLeft(3)).
                Append(" /").
                Append(capacity.PrepString().PadLeft(3)).
                Append(ExeededMax? " <" : " >").
                Append(maxActiveActors.PrepString().PadLeft(3));

            if (ExeededMax)
                stringBuilder.Append(FancyString.B_End);

            if (!last)
                stringBuilder.Append("\n");
        }
    }

    
    public static ActorList[] actorLists;

    private static Dictionary<elementType, ActorList> actorListDict;
    private static readonly Item[] otherSideItems = new Item[Level.MaxActiveElements];
    
    
    public static void GameLoad()
    {
        actorListDict = new Dictionary<elementType, ActorList>(new elementTypeComparer());
        
        List<ActorList> actorListList = new List<ActorList>();
        foreach(elementType elementT in Enum.GetValues(typeof(elementType)))
            if (elementT.InstanceCount() > 0)
            {
                GameObject prefab = Resources.Load("Level/Items/" + elementT) as GameObject;
                if (prefab == null)
                    continue;
                
                Transform parent = Application.isEditor? new GameObject(elementT + "s").transform : null;

                if (parent != null)
                {
                    if(Mask.IsItem.Fits(elementT))
                        parent.parent = SceneLocator.Items;
                    
                    if(Mask.IsFluff.Fits(elementT))
                        parent.parent = SceneLocator.Fluff;
                    
                    if(Mask.IsCollectable.Fits(elementT))
                        parent.parent = SceneLocator.Collectables;
                }

                ActorList list = new ActorList(prefab, parent, elementT, elementT.InstanceCount());

                actorListList.Add(list);
                
                actorListDict.Add(elementT, list);
            }


        actorLists = actorListList.ToArray();
    }

    
    public static void GameStart(float restartShift)  
    {
        ActorAnim.Clear();
        //ActorAnim.GameRestart(restartShift);
        
        ScoreStick.ClearStates();
    }
    
    
    public static void LatePoseSet()
    {
        ActorAnim.Trimm();
        
        ScoreStick.Trimm();
        
        
        Profiler.BeginSample("ActorAnimator.LatePoseSet()");
        
        for (int i = 0; i < actorLists.Length; i++)
            actorLists[i].SetTransform();
        
        Profiler.EndSample();
    }


    public static void ClearAll()
    {
        int count = actorLists.Length;
        for (int i = 0; i < count; i++)
            actorLists[i].ResetAll();
    }
    
    
    public static void UpdateVisibleActors(Item[] addItems, int addItemCount, Item[] removeItems, int removeItemCount)
    {  
        Profiler.BeginSample("ActorAnimator.UpdateVisibleActors()");
        
        //  RemoveItems  //
        {
            for (int i = 0; i < removeItemCount; i++)
            {
                Item removeItem = removeItems[i];
                if (removeItem.elementType.InstanceCount() > 0)
                    actorListDict[removeItem.elementType].Remove(removeItem);
            }
        }
        
        //  AddItems  //
        {
            //  Doing Camera Side first - If there aren't enough Instances left then at least they're missing on the other Side
            int otherSideCount = 0;
            for (int i = 0; i < addItemCount; i++)
            {
                Item addItem = addItems[i];
                
                if (addItem.side == GameCam.CurrentSide)
                {
                    if(addItem.elementType.InstanceCount() > 0)
                        actorListDict[addItem.elementType].Add(addItem);
                }
                else
                    otherSideItems[otherSideCount++] = addItem;
            }

            for (int i = 0; i < otherSideCount; i++)
            {
                Item addItem = otherSideItems[i];
                
                if(addItem.elementType.InstanceCount() > 0)
                    actorListDict[addItem.elementType].Add(addItem);
            }
        }
        
        //  Disable Idle  //
        int actorLength = actorLists.Length;
    
        for (int a = 0; a < actorLength; a++)
            actorLists[a].DisableIdle();
        
        Profiler.EndSample();
    }

    
    public static void OnJump(Jump clip)
    {
        if(clip.before == null)
            Debug.Log("JumpClip has no Clip Before " + clip.spinner.name + " " + clip.spinner.ID);
        
        Vector2 currentLean = ((Swing) clip.before).GetWeightDir(clip.startTime) / clip.stick.Item.mass;
        ActorAnim.AddNewShake(Shake.Get(shake.Jump, clip.startTime, clip.stick.Item, currentLean, clip));


        //  Flyby Shakes  //
        float radius    = clip.spinner.size.y * .5f;
        float maxRadius = clip.spinner.size.y * JumpInfo.SearchRange;
    
        JumpInfo info = clip.info;
        for (int i = 0; i < info.proxCount; i++)
        {
            ProxItem miss = info.proxItems[i];
            Item      item = miss.item;
                
            if(!Mask.MustShake.Fits(item.elementType))
                continue;
    
            Vector2 closestDir   = miss.closestDir;
            float   closestDir_M = closestDir.magnitude;
            Vector2 closestDir_N = closestDir * (1f / closestDir_M);
                    
            float lerp = (1 - Mathf.Clamp01(Mathf.InverseLerp(radius, maxRadius, closestDir_M))) * .3f;
            if (lerp < .125f)
                continue;
    
            Vector2 closest   = miss.closestMotion;
            float   closest_M = closest.magnitude;
            Vector2 closest_N = closest * (1f / closest_M);
                
            float   spinSpeed = GPhysics.Get_SpinSpeed_After(clip.startSpin, miss.closestTime - clip.startTime);
            float   magnitude = (closest_M + Mathf.Abs(spinSpeed) * 2) * lerp;
            Vector2 force     = magnitude * Vector2.Lerp(closest_N, -closestDir_N, .4f) * 3;
            float   time      = miss.closestTime + closestDir_M * .01f;
    
            ActorAnim.AddNewShake(Shake.Get(shake.FlyBy, time, item, force, clip));
        }
        
        Spinner.UpdateOffsets(clip.spinner);
    }
    
    
    public static void OnSwing(Swing clip, bool priority)
    {
        if(priority)
            ActorAnim.AddNewShake(Shake.Get(shake.Swing, clip.startTime, clip.startStick.Item, clip.startMotion, clip));
        
        ElementMask filter = priority ? Mask.ShakeItem as ElementMask: Mask.ShakeFluff;
        
        ImpactPulse(clip.startMotion, clip.startTime, clip.startStick.Item, clip, 1, filter);
    }
    
    
    public static void OnBump(Bump clip)
    {
        ActorAnim.AddNewShake(Shake.Get(shake.Swing, clip.startTime, clip.bumpItem, -clip.mV, clip));
        
        ImpactPulse(-clip.mV, clip.startTime, clip.bumpItem, clip, 8, Mask.MustShake);
    }


    private static void ImpactPulse(Vector2 mV, float impactTime, Item item, Clip clip, float forceMulti, ElementMask filter)
    {
        float maxRadius = clip.spinner.size.y * JumpInfo.SearchRange * 1.5f;

        Vector2 rootPos = item.GetPos(impactTime);
        Bounds2D b = new Bounds2D(rootPos).Pad(maxRadius);
        
        Search.Items(b, item.side, impactTime, filter);

        float   mV_M = mV.magnitude;
        Vector2 mV_N = mV * (mV_M > 0? 1f / mV_M : 0);
        
        for (int i = 0; i < Search.itemCount; i++)
        {
            Item bItem = Search.boundItems[i];
            if (bItem == item)
                continue;

            Vector2 itemPos = bItem.GetPos(impactTime);
            Vector2 dir     = itemPos - rootPos;
            float   dir_M   = dir.magnitude;
            Vector2 dir_N   = dir * (1f / dir_M);
            float   dot     = 1 + (Vector2.Dot(mV_N, dir_N) * .5f + .5f);
            float   lerp    = (1 - Mathf.Clamp01(dir_M / maxRadius)) * .55f * dot * .4f;
            if (lerp < .125f)
                continue;

            Vector2 force = dir_N * mV_M * lerp * forceMulti;
            float   time  = impactTime + dir_M * .035f;

            ActorAnim.AddNewShake(Shake.Get(shake.FlyBy, time, bItem, force, clip));
        }
        
        Spinner.UpdateOffsets(clip.spinner);
    }


    public static void DebugPulse()
    {
        const float maxRadius = 6 * JumpInfo.SearchRange * 1.5f;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        Plane plane = new Plane(Vector3.forward * -GameCam.CurrentSide.Sign, new Vector3(0, 0, Level.WallDepth * GameCam.CurrentSide.Sign));
        plane.Raycast(ray, out var dist);
        
        Vector3 hit = ray.origin + ray.direction * dist;
        
        
        Vector2 rootPos = hit;
        Bounds2D b = new Bounds2D(rootPos).Pad(maxRadius);
        
        Search.Items(b, GameCam.CurrentSide, GTime.Now, Mask.MustShake);
        for (int i = 0; i < Search.itemCount; i++)
        {
            Item    item    = Search.boundItems[i];
            Vector2 itemPos = item.GetPos(GTime.Now);
            Vector2 dir     = itemPos - rootPos;
            float   length  = dir.magnitude;
            float   lerp    = (1 - Mathf.Clamp01(length / maxRadius)) * .55f * .4f;
            if (lerp < .125f)
                continue;

            Vector2 force = dir.SetLength(200 * lerp * 2);
            float   time  = GTime.Now + length * .035f;
            
            ActorAnim.AddNewShake(Shake.Get(shake.FlyBy, time, item, force, null));
        }
        
        Spinner.UpdateOffsets(null);
    }


    public static void OnWarp(Swing clip, float time)
    {
        Vector2 currentLean = clip.GetWeightDir(time);
        
        ActorAnim.AddNewShake(Shake.Get(shake.WarpAway, time, clip.GetStick(clip.startTime).Item, currentLean, clip));
        ActorAnim.AddNewShake(Shake.Get(shake.Swing,    time, clip.GetStick(time).Item, V2.zero, clip));
        
        Spinner.UpdateOffsets(clip.spinner);
    }
    

    public static void ClearAllAfter(Spinner spinner, float time)
    {
        int count = ActorAnim.actorAnims.Count;
        for (int i = 0; i < count; i++)
            if(ActorAnim.actorAnims[i].ClearAfter(spinner, time))
            {
                ActorAnim.RemoveAnim(i);
                i--;
                count--;
                
                if (count == 0)
                    break;
            }
    }


    public static void ActorAction(Element element)
    {
        if (Mask.IsItem.Fits(element.elementType))
        {
            Actor actor = ActorList.allActors[element.ID];
            if(actor != null)
                ((IElementAction)actor).Action();
        }
    }
}




namespace ActorAnimation
{
    public partial class ActorAnim : PoolObject
    {
        private float endTime;
        public  Item  item;
        private bool  noWeight;

        private readonly List<Shake> shakes = new List<Shake>(100);

        private bool CanBeTrimmed { get { return endTime <= Mathf.Max(0, GTime.Now - GTime.RewindTime) && GetLeanNow() == V2.zero; }}
        
        
        private void TrimmShakes()
        {
            int count = shakes.Count;
            for (int i = 0; i < count; i++)
                if (shakes[i].shakeEndTime < GTime.Now - GTime.RewindTime)
                {
                    shakes.GetRemoveAt(i).Reset();
                    count--;
                    i--;
                }
        }


        private void Reset()
        {
            if (item != null)
                itemAnims[item.ID] = null;
            
            item = null;

            int count = shakes.Count;
            for (int i = 0; i < count; i++)
                shakes[i].Reset();
            
            shakes.Clear();
            actorAnimPool.Return(this);
        }


        private ActorAnim Setup(Shake shake)
        {
            item = shake.item;

            itemAnims[item.ID] = this;
            
            noWeight = !Mask.CanBeGrabbed.Fits(shake.item.elementType);
            AddShake(shake);
            return this;
        }


        private void AddShake(Shake shake)
        {
            shakes.Add(shake);

            if (shake.shakeEndTime > endTime)
                endTime = shake.shakeEndTime;
        }


        public bool ClearAfter(Spinner spinner, float time)
        {
            int count = shakes.Count, countBefore = count;
            
            for (int i = 0; i < count; i++)
                if (shakes[i].eventTime >= time && shakes[i].Spinner == spinner)
                {
                    shakes.GetRemoveAt(i).Reset();
                    count--;
                    i--;
                }

            if (countBefore != count)
            {
                //  Find new EndTime  //
                endTime = 0;
                for (int i = 0; i < count; i++)
                    if (shakes[i].shakeEndTime > endTime)
                        endTime = shakes[i].shakeEndTime;
                
                Spinner.UpdateOffsets(spinner);
            }
                

            return count == 0;
        }


        public Vector2 GetLean(float time)
        {
            Vector2 lean = Vector2.zero;
        
            //  Get Shakes  //
            for (int i = 0; i < shakes.Count; i++)
                shakes[i].GetShake(time, ref lean);
        
            //  Get Weight  //
            if (!noWeight)
            {
                int activeCount = Spinner.active.Count;
                for (int i = 0; i < activeCount; i++)
                    if (Spinner.active[i].tape.GetClip(time) is Swing swing && swing.GetStick(time).SameAs(item))
                    {
                        const float zFactor = (Level.PlaneOffset /* + spinner.GetZShift(time)*/) / (Level.PlaneOffset * 2) * 2;
                        lean += swing.GetWeightDir(time) * zFactor / item.mass;
                    }
            }
                
            
            //  Damp it  //
            float  dampMax = .9f / item.mass;
            float  lean_M  = lean.magnitude;
            return lean * (lean_M > 0? 1f / lean_M * Mth.DampedRange(lean_M, 0, dampMax) : 0);
        }
        
        
        public Vector2 GetLeanNow()
        {
            Vector2 lean = Vector2.zero;
        
            //  Get Shakes  //
            for (int i = 0; i < shakes.Count; i++)
                shakes[i].GetShake(GTime.Now, ref lean);
        
            //  Get Weight  //
            if (!noWeight)
            {
                int activeCount = Spinner.active.Count;
                for (int i = 0; i < activeCount; i++)
                {
                    Swing swing = Spinner.GetSwingClip(i, item);
                    if (swing != null)
                    {
                        const float zFactor = (Level.PlaneOffset /* + spinner.GetZShift(time)*/) / (Level.PlaneOffset * 2) * 2;
                        lean += swing.GetWeightDir(GTime.Now) * zFactor / item.mass;
                    }
                }
            }
                
            
            //  Damp it  //
            float  dampMax  = .9f / item.mass;
            float  lean_M   = lean.magnitude;
            return lean * (lean_M > 0? 1f / lean_M * Mth.DampedRange(lean_M, 0, dampMax) : 0);
        }


        private void Restart(float restartShift)
        {
            endTime += restartShift;

            int shakeCount = shakes.Count;
            for (int i = 0; i < shakeCount; i++)
                shakes[i].Shift(restartShift);
        }
    }




    public partial class ActorAnim
    {
        static ActorAnim()
        {
            actorAnims    = new List<ActorAnim>(Level.MaxActiveElements);
            actorAnimPool = new Pool<ActorAnim>(() => new ActorAnim(), Level.MaxActiveElements * 2);
            itemAnims     = new ActorAnim[Item.TotalCount];
        }
        
        
        public  static readonly List<ActorAnim> actorAnims;
        private static readonly Pool<ActorAnim> actorAnimPool;
        private static readonly ActorAnim[]     itemAnims;

        
        public static void Trimm()
        {
            int count = actorAnims.Count;
            for (int i = 0; i < count; i++)
                if (actorAnims[i].CanBeTrimmed)
                {
                    RemoveAnim(i);
                    count--;
                    i--;
                }
                else
                    actorAnims[i].TrimmShakes();
        }


        public static void RemoveAnim(int index)
        {
            actorAnims.GetRemoveAt(index).Reset();
        }

        
        public static void Clear()
        {
            int count = actorAnims.Count;
            for (int i = 0; i < count; i++)
                actorAnims[i].Reset();
                
            actorAnims.Clear();
        }


        public static void GameRestart(float restartShift)
        {
            int count = actorAnims.Count;
            for (int i = 0; i < count; i++)
                actorAnims[i].Restart(restartShift);
                
            Trimm();
        }

        
        public static void AddNewShake(Shake shake)
        {
            if (itemAnims[shake.item.ID] != null)
                itemAnims[shake.item.ID].AddShake(shake);
            else
                actorAnims.Add(actorAnimPool.GetFree().Setup(shake));
        }


        public static Color DebugTextColor(Item item)
        {
            if (itemAnims[item.ID] != null && itemAnims[item.ID].GetLeanNow() != V2.zero)
                return itemAnims[item.ID].item == item ? Spinner.IsBeingGrabbed(item)? COLOR.orange.sienna : COLOR.yellow.fresh : COLOR.red.tomato;
            
            return Color.Lerp(Color.white, COLOR.grey.light, .75f);
        }


        public static Vector2 GetLean(Item item, float time)
        {
            return itemAnims[item.ID] == null ? V2.zero : itemAnims[item.ID].GetLean(time);
        }


        public static ActorAnim GetAnim(Item item)
        {
            return itemAnims[item.ID] != null ? itemAnims[item.ID] : null;
        }
    }
}
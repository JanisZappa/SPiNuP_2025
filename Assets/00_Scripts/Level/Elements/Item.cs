using System.Collections.Generic;
using Clips;
using GeoMath;
using UnityEngine;


namespace LevelElements
{
    public partial class Item : Element
    {
        public int   trackIndex;
        public float radius, mass, trackOffset, trackLag;
        public bool  isOccluded;
        public Track parent;
        
        
        public override void Reset()
        {
            base.Reset();
            
            Level.RemoveElementFromCells(this);
            
            parent = null;
            
            trackLag = 0;
            
            itemPool.Return(this);
              active.Remove(this);
        }
        
        
        public override Vector2 GetPos(float time)
        {
            return elementType.Anim(time, ID, parent == null ? rootPos : parent.GetChildPos(this, time));
        }
        
        
        public Vector2 GetLagPos(float time)
        {
            return GetPos(time + trackLag);
        }
        
        
        public Vector2 GetMV(float time)
        {
            if (parent == null)
                return Vector2.zero;
            
            return (parent.GetChildPos(this, time) - 
                    parent.GetChildPos(this, time - GPhysics.TimeStep)) 
                   * GPhysics.StepsPerSecond;
        }
        
        
        public override Element SetType(elementType elementType)
        {
            this.elementType = elementType;
            
            radius   = elementType.Radius();
            mass     = elementType.Mass();
            trackLag = Mask.MustShake.Fits(elementType) ? .0215f / mass * -1 : 0;

            return this;
        }

            
        public override Element SetRootPos(Vector2 rootPos)
        {
            this.rootPos = parent != null ? parent.rootPos : rootPos;

            return this;
        }

        
        public override Element SetSide(Side side)
        {
            this.side = side;

            return this;
        }
        
        
        public override void Refresh()
        {
            if (parent == null)
            {
                Level.RemoveElementFromCells(this);
                bounds = new Bounds2D(rootPos).Pad(radius);
                Level.AddElementToCells(this);
            }
        }


        private Item GetShiftedItem(float timeShift)
        {
            return parent == null? this: parent.GetShiftItem(this, timeShift);
        }


        public override float SqrDistance(Vector2 point, float time)
        {
            Vector2 pos = GetPos(time);
            float xDir = pos.x - point.x;
            float yDir = pos.y - point.y;

            return xDir * xDir + yDir * yDir;
        }


        public bool IsAlike(Item other)
        {
            Group peerGroup = elementType.Alias();
            if (peerGroup != null)
                return peerGroup.Contains(other.elementType);

            return elementType == other.elementType;
        }
        
        
        public void Delete()
        {
            if (parent != null)
                parent.UnparentThis(this).ScanAndRefesh();
            else
            {
                Item buddy = (Item)Link.GetWarpBuddy(this);
                buddy?.Reset();

                Reset();
            }
        }


        /*public void LinkSignal(Swing swing)
        {
            for (int i = 0; i < activeLinkCount; i++)
                activeLinks[i].OnSwing(swing);
        }*/


        public Item QuickSet(elementType type, Vector2 pos, Side side)
        {
            SetType(type);
            SetRootPos(pos);
            SetSide(side);
            Refresh();
                
            return this;
        }
    }
  
    
    
    
    public partial class Item
    {
        public const int   TotalCount    = 10000;
        public const float DefaultRadius = .075f;
        
        public static readonly List<Item> active;
        private static readonly Pool<Item> itemPool; 
        
        public static int Count { get { return itemPool.ActiveElementCount; } }
        
        static Item()
        {
            itemPool = new Pool<Item> (() => new Item(), TotalCount);
              active = new List<Item>(TotalCount);
        }
        
        public static void PoolReset()
        {
              active.Clear();
            itemPool.Reset();
        }
        
        public static Item Get(int itemID)
        {
            return itemPool.all[itemID];
        }
        
        public static Item Get(int itemID, float timeShift)
        {
            return itemPool.all[itemID].GetShiftedItem(timeShift);
        }

        public static Item GetFreeItem { get { return active.GetAdd(itemPool.GetFree()); } }

        public override void Draw()
        {
            Color drawColor = LevelCheck.GetColor(this);

            Vector2 pos = GetPos(GTime.Now);
            
            DRAW.Circle(pos, radius, 16).SetColor(drawColor).SetDepth(Z.W05);


            if (ElementEdit.element == this)
            {
                DRAW.MultiCircle(pos, radius + .6f, 3, .2f, 16).SetColor(drawColor.A(.75f)).SetDepth(Z.W05);  
                if(parent != null)
                    parent.Draw();
                return;
            }
            

            if (LevelCheck.Overlapping(this))
            {
                bool extra = LevelCheck.ItemCheck && LevelCheck.Filter.Fits(elementType);
                DRAW.Circle(pos, radius + (extra? Level.PlacementRadius :.4f), 16).SetColor(drawColor.A(.75f)).SetDepth(Z.W05);  
            }
            else 
                if (LevelCheck.HighLighted(this))
                {
                    DRAW.Circle(pos, radius + .4f, 16).SetColor(drawColor.A(.75f)).SetDepth(Z.W05); 
                    
                    if(parent != null)
                        parent.Draw();
                }   
        }

        public override string GetInfo()
        {
            if (parent == null)
            {
                infoBuilder.Length = 0;
                infoBuilder.Append(ID.ToString().PadLeft(4)).Append(" ").Append("►").Append(" ").
                    Append(elementType.ToString().PadRight(ElementTypeExt.LongestItemName));
                
                //if(Link.)
            }
            else
            {
                string pString = parent.GetInfo();
            
                infoBuilder.Length = 0;
                infoBuilder.Append(ID.ToString().PadLeft(4)).Append(" ").Append("▼").Append(" ").
                    Append(elementType.ToString().PadRight(ElementTypeExt.LongestItemName)).
                    Append(" | TrackPos: ").Append(trackIndex).Append("\n").Append(pString);
            }
            
            return infoBuilder.ToString();
        }
    }
}

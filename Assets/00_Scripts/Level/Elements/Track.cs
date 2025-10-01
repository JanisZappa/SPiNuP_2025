using GeoMath;
using ShapeStuff;
using UnityEngine;


namespace LevelElements
{
    public abstract partial class Track : Element
    {
        protected Track(elementType elementType)
        {
            this.elementType = elementType;
        }
        
        public readonly Item[] items = new Item[20];
        public int itemCount;
        
        public float offset, speed = 2, size = 1, growth = .5f;

        protected Vector2 shapeVector;
        public Vector2 center;

        private int   slowDownSteps;
        public  float maxItemRadius;
        
        public int subBoundCount;
        private const int maxSubBounds = 8;
        private readonly SubBound[] subBounds = CollectionInit.Array<SubBound>(maxSubBounds);
        public readonly bool[] subOcclusion = new bool[maxSubBounds];
        public bool allSubsVisible, allSubsOccluded;

        public abstract float TrackLength { get; }

        
        public override void Reset()
        {
            base.Reset();

            for (int i = 0; i < itemCount; i++)
                items[i].Delete();
            
            itemCount = 0;
            
            Level.RemoveElementFromCells(this);
            
            speed  = 2;
            offset = 0;
            size   = 1;
            growth = 1;

            switch (elementType)
            {
                case elementType.PingPong: pingPongPool.Return(this); break;
                case elementType.Circular: circularPool.Return(this); break;
                case elementType.Arc:           arcPool.Return(this); break;
            }
        }


        public override Element SetRootPos(Vector2 rootPos)
        {
            this.rootPos = rootPos;

            return this;
        }
        

        public Track SetSize(float size)
        {
            this.size = Mathf.Clamp(size, .1f, 3);
            
            return this;
        }

        
        public Track SetAngle(float angle)
        {
            this.angle = angle;

            return this;
        }

        
        public Track SetOffset(float offset)
        {
            this.offset = offset;

            return this;
        }
        
        
        public Track SetCompletion(float completion)
        {
            if (Mask.TrackCanGrow.Fits(elementType))
                growth = Mathf.Clamp(completion, .05f, .95f);
            
            return this;
        }


        public override void Refresh()
        {
            Level.RemoveElementFromCells(this);
            {
                shapeVector = V2.up.Rot(angle, elementType.DefaultVScale() * size);

                CalculateBounds();
            }
            Level.AddElementToCells(this);

            
            for (int i = 0; i < itemCount; i++)
                items[i].bounds = bounds;
        }


        protected abstract void CalculateBounds();
        
        
        public Track ParentThis(Item newItem)
        {
            if(itemCount == elementType.MaxItems())
                return this;

            if (items.Contains(newItem, itemCount))
            {
                Debug.Log("Item already on track ...");
                return this;
            }
            
            newItem.parent = this;
            newItem.SetRootPos(rootPos);
            newItem.SetSide(side);
            
            items[itemCount++] = newItem;

            return this;
        }


        public Track ReplaceAt(Item removeThis, Item newItem)
        {
            removeThis.Reset();
            
            newItem.parent = this;
            newItem.SetRootPos(rootPos).SetSide(side);
            
            items.Replace(removeThis, newItem, itemCount);

            return this;
        }


        public Track UnparentThis(Item removeThis)
        {
            removeThis.Reset();
            
            itemCount = items.ShiftRemove(removeThis, itemCount);

            return this;
        }


        public void ScanAndRefesh()
        {
        //  MaxRadius  //
            maxItemRadius = 0;
            for (int i = 0; i < itemCount; i++)
                maxItemRadius = Mathf.Max(maxItemRadius, items[i].radius);
            
            
        //  Calculate Speed  //
            {
            //  Segment Length  //
                int segmentLength = itemCount;
                for (int i = 0; i < itemCount; i++)
                {
                    int checkLength = itemCount - i;
                    for (int e = 0; e < itemCount; e++)
                    {
                        int toThis = (e + checkLength) % itemCount;

                        if(!items[e].IsAlike(items[toThis]))
                            break;
    
                        if (e == itemCount - 1)
                            segmentLength = checkLength;
                    }
                }
    
                
            //  SlowDownSteps  //
                int   segments  = (int) ((float) itemCount / segmentLength);
                float log       = Mathf.Log(segments, 2);
                bool  canBeSlow = segments > 1 && f.Same(log % 1, 0);
                slowDownSteps   = canBeSlow? (int)log : 0;
    
    
            //  Validate Current Speed  //
                float absSpeed = Mathf.Abs(speed);
                if (absSpeed < 1)
                {
                    bool validSpeed = false;
                    for (int i = 0; i < slowDownSteps + 1; i++)
                        if (f.Same(absSpeed, 1f.Divide(2, i)))
                        {
                            validSpeed = true;
                            break;
                        }
    
                    if (!validSpeed)
                        speed = Mathf.Sign(speed);
                }
            }
        
        
        //  Set TrackIndex and Offsets  //
            {
                float multi = 1f / itemCount;
                for (int i = 0; i < itemCount; i++)
                {
                    items[i].trackIndex = i;
                    items[i].trackOffset = i * multi;
                }
            }
            
            Refresh();
        }


        public bool AnyItemMatchesMask(ElementMask mask)
        {
            for (int i = 0; i < itemCount; i++)
                if (mask.Fits(items[i].elementType))
                    return true;

            return false;
        }


        public void ShiftSpeed(bool forward)
        {
            speed = ChangeSpeed(forward? 1 : -1);
        }

        
        private float ChangeSpeed(float dir)
        {
            float sign     = Mathf.Sign(speed);
            float absSpeed = Mathf.Abs(speed);
            dir           *= sign;
            

            if (dir > 0)
            {
                if (absSpeed >= 1)
                    return (absSpeed + 1) * sign;

                if (slowDownSteps == 0)
                    return sign;
                
                
                int divisions = 0;
                for (int i = 0; i < slowDownSteps + 1; i++)
                    if (f.Same(absSpeed, 1f.Divide(2, i)))
                    {
                        divisions = i - 1;
                        break;
                    }

                return 1f.Divide(2, divisions) * sign;
            }
            
            
            {
                if (absSpeed > 1)
                    return (absSpeed - 1) * sign;
                
                if (f.Same(absSpeed, MinSpeed) || f.Same(absSpeed, 1) && slowDownSteps == 0)
                    return -speed;
                

                int divisions = 0;
                for (int i = 0; i < slowDownSteps + 1; i++)
                    if (f.Same(absSpeed, 1f.Divide(2, i)))
                    {
                        divisions = i + 1;
                        break;
                    }
                
                if (divisions > slowDownSteps)
                    return -speed;
                
                return 1f.Divide(2, divisions) * sign;
            }
        }

        
        public Item GetShiftItem(Item item, float shift)
        {
            if (Mathf.Abs(speed) < 1)
            {
                int shiftSteps = item.trackIndex + Mathf.RoundToInt(itemCount * speed * -shift / GTime.LoopTime);
                
                return items[(int)Mathf.Repeat(shiftSteps, itemCount)];
            }
            
            return item;
        }


        public override Element SetSide(Side side)
        {
            this.side = side;
            
            for (int i = 0; i < itemCount; i++)
                items[i].SetSide(side);

            return this;
        }
        
        
        public Vector2 GetChildPos(Item item, float time)
        {
            return GetTrackPos(GetLerp(time, item != null? item.trackOffset : 0));
        }
        

        protected abstract Vector2 GetTrackPos(float lerp);

        
        protected virtual float GetLerp(float time, float itemOffset)
        {
            return (speed * time * GTime.LoopMulti + itemOffset + offset) % 1;
        }


        public SubBound GetSubBound(int boundIndex)
        {
            return subBounds[boundIndex];
        }


        protected void CalculateSubBounds(Maxima[] maxima = null)
        {
            subBoundCount = (int)Mathf.Max(1, Mathf.Min(maxSubBounds, TrackLength / 6));
            float timeStep = 1f / subBoundCount;
            for (int i = 0; i < subBoundCount; i++)
                subBounds[i].SetLerps(i * timeStep, (i + 1) * timeStep, this, maxima);
        }


        public bool ItemIsInSubBound(Item item, int subBound)
        {
            SubBound sB = subBounds[subBound];
            for (int i = 0; i < sB.itemCount; i++)
                if (sB.items[i].Equals(item))
                    return true;

            return false;
        }


        public void FillSubBounds(float time, ElementMask filter)
        {
            for (int i = 0; i < subBoundCount; i++)
                subBounds[i].itemCount = 0;

            for (int i = 0; i < itemCount; i++)
            {
                Item item = items[i];
                if(!(filter.Fits(item.elementType)))
                    continue;
                
                SubBound subBound = subBounds[SubIndex(SubStep(item, time + item.trackLag))];
                subBound.items[subBound.itemCount++] = item;
            }
        }
        

        public virtual int SubStep(Item item, float time)
        {
            float unclampedLerp = speed * time * GTime.LoopMulti + item.trackOffset + offset;
            return Mathf.FloorToInt(unclampedLerp * subBoundCount);
        }

        
        public virtual int SubIndex(int subStep)
        {
            return subStep >= 0 ? subStep % subBoundCount : (subBoundCount + (subStep % subBoundCount)) % subBoundCount;
        }
        
        
        public abstract Vector2 GetClosestPoint(Vector2 point);


        public override float SqrDistance(Vector2 point, float time)
        {
            Vector2 pos = GetClosestPoint(point);
            float xDir = pos.x - point.x;
            float yDir = pos.y - point.y;

            return xDir * xDir + yDir * yDir;
        }
        
        
        public virtual bool Overlaps(Track other)
        {
            return false;
        }


        public override void SetDepth(float depth)
        {
            base.SetDepth(depth);

            for (int i = 0; i < itemCount; i++)
                items[i].SetDepth(depth);
        }

        public override string GetInfo()
        {
            string speedAdd = speed > 0 ? " " : "";
            
            infoBuilder.Length = 0;
            infoBuilder.Append(ID.ToString().PadLeft(4)).Append(" ► ").Append(elementType.ToString().PadRight(ElementTypeExt.LongestItemName)).
                        Append(" | Size: ").Append(size.ToString("F1")).Append(" • Angle: ").Append(angle.ToString("F1")).
                        Append(" | Speed: ").Append(speedAdd).Append(speed).Append(" • Offset: ").Append(offset.ToString("F1")).
                        Append(" | ").Append(itemCount.ToString().PadLeft(2)).Append(" Item" + (itemCount > 1? "s" : " ") + " • Groups: ").Append(Mathf.RoundToInt(1f / MinSpeed));

            return infoBuilder.ToString();
        }

        
        private float MinSpeed { get { return 1f.Divide(2, slowDownSteps); }}


        public abstract void TellShapeWhatToDo(Shape shape);


        public class SubBound
        {
            public Bounds2D bounds;

            public int itemCount;
            public readonly Item[] items = new Item[20];

            
            public void SetLerps(float lerpA, float lerpB, Track track, Maxima[] maximas)
            {
                bounds = new Bounds2D(track.GetTrackPos(lerpA)).Add(track.GetTrackPos(lerpB));
                
            //  Add Lerp Range Maximas //
                if(maximas != null)
                    for (int i = 0; i < maximas.Length; i++)
                        if (maximas[i].FitsInLerpRange(lerpA, lerpB))
                            bounds = bounds.Add(maximas[i].pos);
                
                bounds = bounds.Pad(track.maxItemRadius);
            }


            public bool Intersects(Bounds2D otherBound)
            {
                return bounds.Intersects(otherBound);
            }
        }

        
        public class Maxima
        {
            public  Vector2 pos;
            private float   lerp;

            public void Set(Vector2 pos, float lerp)
            {
                this.pos  = pos;
                this.lerp = lerp;
            }
            

            public bool FitsInLerpRange(float lerpA, float lerpB)
            {
                return lerpA <= lerp && lerpB > lerp;
            }
        }
        
        
        private int LerpSub(float lerp)
        {
            return Mathf.FloorToInt(Mathf.Repeat(lerp, 1) * subBoundCount);
        }


        public int GetOldSubIndex(Item item, float time)
        {
            return LerpSub(GetLerp(time, item.trackOffset));
        }


        public abstract Track QuickSet(Vector2 from, Vector2 to, Side side);
    }
}

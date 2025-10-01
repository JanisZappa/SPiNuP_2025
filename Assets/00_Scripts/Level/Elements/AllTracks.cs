using System.Collections.Generic;
using GeoMath;
using ShapeStuff;
using UnityEngine;


namespace LevelElements
{
    public class Track_Circular : Track
    {
        public Track_Circular() : base(elementType.Circular){}

        private readonly Maxima[] maxima = CollectionInit.Array<Maxima>(4);

        public override float TrackLength { get { return 2 * Mathf.PI * radius; } }

        private float radius;

        protected override void CalculateBounds() 
        { 
            center = rootPos;
            radius = elementType.DefaultVScale() * size;
            
            bounds = new Bounds2D(rootPos).Pad(elementType.DefaultVScale() * size + maxItemRadius);
            
        //  Maxima  //
            Vector2 dir = shapeVector * (1f / radius);

            const float lerpRange = 1f / 360;
            for (int i = 0; i < 4; i++)
            {
                Vector2 checkDir = V2.up.Rot(i * 90);
                float   lerp     = TurnAngle(dir, checkDir) * lerpRange;
                Vector2 pos      = rootPos + checkDir * radius;
                maxima[i].Set(pos, lerp);
            } 
                
                
            CalculateSubBounds(maxima);
        }
        
        
        protected override Vector2 GetTrackPos(float lerp)
        {
            float rad = lerp * Mth.FullRad;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);

            float tx = shapeVector.x;
            float ty = shapeVector.y;
        
            return new Vector2(cos * tx - sin * ty + rootPos.x, 
                               sin * tx + cos * ty + rootPos.y);
        }

        
        public override Vector2 GetClosestPoint(Vector2 point)
        {
            return rootPos.AimPos(point, radius);
        }


        public override void Draw()
        {
            Color drawColor = Mathf.Abs(speed) < 1? Color.white:  LevelCheck.GetColor(this);
            
            DRAW.Circle(rootPos, radius).SetColor(drawColor).SetDepth(Z.W05);
                    
            Vector2 offsetDir = (GetChildPos(null, GTime.Now) - rootPos).normalized;
            DRAW.Vector(rootPos + offsetDir * shapeVector.magnitude, offsetDir).SetColor(drawColor).SetDepth(Z.W05);
        }


        public override void TellShapeWhatToDo(Shape shape)
        {
            shape.segmentCount = 1;
            
            shape.segments[0].SetPointDir(GetTrackPos(0), Mth.π * .5f).SetLengthBend(TrackLength, 1);
            
            shape.FinishSetup();
        }
        
        
        public override Track QuickSet(Vector2 from, Vector2 to, Side side)
        {
            rootPos = from * .5f + to * .5f;
                    
            Vector2 trackDir = to - from;
            float   trackMag = trackDir.magnitude;
                    
            size   = trackMag / elementType.PingPong.DefaultVScale();
            offset = Random.Range(0, 1f);
            this.side = side;
            
            growth = 1;
            
            radius = elementType.DefaultVScale() * size;
            
            speed  = Mathf.Max(1, 8 - Mathf.Round(TrackLength * .2f)) * (Random.Range(0, 2) == 0? -1 : 1);
            
            return this;
        }
    }
    
    
    public class Track_PingPong : Track
    {
        public    Track_PingPong() : base(elementType.PingPong){}
        protected Track_PingPong(elementType elementType) : base(elementType){}

        public override float TrackLength { get { return elementType.DefaultVScale() * size * 2; } }
        
        protected override void CalculateBounds()
        {
            center = rootPos;
            bounds = new Bounds2D(rootPos - shapeVector * .5f).Add(rootPos + shapeVector * .5f).Pad(maxItemRadius);
            
            CalculateSubBounds();
        }
        
        
        protected override Vector2 GetTrackPos(float lerp)
        {
            lerp -= .5f;
            return new Vector2(shapeVector.x * lerp + rootPos.x, shapeVector.y * lerp + rootPos.y);
        }


        protected override float GetLerp(float time, float itemOffset)
        {
            return Mth.SmoothPP(0, 1, base.GetLerp(time, itemOffset) * 2);
        }
        
        
        public override int SubStep(Item item, float time)
        {
            float unclampedLerp = (speed * time * GTime.LoopMulti + item.trackOffset + offset) * 2;
            
        //  Count All Full Swipe Subs  //
            int fullSwipeSubs = Mathf.FloorToInt(Mathf.Abs(unclampedLerp)) * subBoundCount;
        //  Count and Add Current Swipe Subs  //
            int currentSwipeSubs = Mathf.FloorToInt(Mth.SmoothPP(0, subBoundCount, unclampedLerp % 1));

            return fullSwipeSubs + currentSwipeSubs;
        }
        
        
        public override int SubIndex(int subStep)
        {
            int leftOver = subStep % (subBoundCount * 2);
            return leftOver < subBoundCount ? leftOver : (subBoundCount - 1 - (leftOver - subBoundCount)) % subBoundCount;
        }

        
        public override Vector2 GetClosestPoint(Vector2 point)
        {
            Vector2 half = shapeVector * .5f;

            return new Line(rootPos - half, rootPos + half).ClosestPoint(point);
        }


        public override void Draw()
        {
            Color drawColor = LevelCheck.GetColor(this);
            
            DRAW.Vector(rootPos - shapeVector * .5f, shapeVector).SetColor(drawColor).SetDepth(Z.W05);
                    
            Vector2 offsetPos = GetChildPos(null, GTime.Now);
            Vector2 offsetDir = shapeVector.Rot(90).normalized;
            DRAW.Vector(offsetPos, offsetDir).SetColor(drawColor).SetDepth(Z.W05);
        }
        
        
        public override void TellShapeWhatToDo(Shape shape)
        {
            shape.segmentCount = 1;
            
            shape.segments[0].SetPointDir(rootPos - shapeVector * .5f, shapeVector.ToRadian() - Mth.π * .5f).SetLengthBend(TrackLength * .5f, 0);
            
            shape.FinishSetup();
        }

        
        public override Track QuickSet(Vector2 from, Vector2 to, Side side)
        {
            rootPos = from * .5f + to * .5f;
                    
            Vector2 trackDir = to - from;
            float trackMag = trackDir.magnitude;
                    
            angle  = (trackDir / trackMag).Angle_Sign(Vector2.up);
            size   = trackMag / elementType.PingPong.DefaultVScale();
            offset = Random.Range(0, 1f);
            this.side = side;
            speed  = 2;
            growth = 1;
            
            return this;
        }
    }
    
    
    public class Track_Arc : Track_PingPong
    {
        public Track_Arc() : base(elementType.Arc){}

        private static readonly Maxima[] maxima = CollectionInit.Array<Maxima>(4);

        public override float TrackLength { get { return 2 * Mathf.PI * radius * growth * 2; } }

        private float radius;

        private Arc arc;
        
        
        public override void Reset()
        {
            base.Reset();

            growth = .5f;
        }
        
        
        protected override void CalculateBounds()
        {
            radius = elementType.DefaultVScale() * size;
            arc    = new Arc(rootPos, radius, angle, growth);
            
            center = GetTrackPos(.5f);
            
            bounds = new Bounds2D(GetTrackPos(0)).Add(GetTrackPos(1));
            
                
        //  Maxima + Bounds  //
            Vector2 dir = shapeVector.normalized;

            float lerpRange = 1f / 360 / growth;
            for (int i = 0; i < 4; i++)
            {
                Vector2 checkDir = V2.up.Rot(i * 90);
                float   lerp     = TurnAngle(dir, checkDir) * lerpRange;
                Vector2 pos      = rootPos + checkDir * radius;
                maxima[i].Set(pos, lerp);
                
                if(lerp <= 1)
                    bounds = bounds.Add(pos);
            }

            bounds = bounds.Pad(maxItemRadius);

            CalculateSubBounds(maxima);
        }
        
        
        protected override Vector2 GetTrackPos(float lerp) 
        { 
            return arc.LerpPos(lerp);
        }
        

        public override Vector2 GetClosestPoint(Vector2 point)
        {
            return arc.GetClosestPoint(point);
        }


        public override void Draw()
        {
            Color drawColor = LevelCheck.GetColor(this);
            
            DRAW.Arc(rootPos, radius, arc.bend, angle).SetColor(drawColor).SetDepth(Z.W05);
                    
            Vector2 offsetDir = (GetChildPos(null, GTime.Now) - rootPos).normalized;
            DRAW.Vector(rootPos + offsetDir * shapeVector.magnitude, offsetDir).SetColor(drawColor).SetDepth(Z.W05);

            DRAW.Rectangle(center, V2.one * .2f).SetColor(Color.white).SetDepth(Z.W05).Fill(1);
        }
        
        
        public override void TellShapeWhatToDo(Shape shape)
        {
            shape.segmentCount = 1;
            
            shape.segments[0].SetPointDir(arc.LerpPos(0), arc.LerpDir(0).ToRadian() - Mth.π * .5f).SetLengthBend(TrackLength * .5f, arc.bend);
            
            shape.FinishSetup();
        }
    }
    
    
    
    
    
    
    public abstract partial class Track
    {
        static Track()
        {
            pingPongPool = new Pool<Track>(() => new Track_PingPong(), pCount);
            circularPool = new Pool<Track>(() => new Track_Circular(), cCount);
            arcPool      = new Pool<Track>(() => new Track_Arc(),      aCount);
            
            active = new List<Track>(TotalCount);
        }
        
        private static readonly Pool<Track> pingPongPool, circularPool, arcPool;

        private const int pCount = 100, cCount = 100, aCount = 100;
        public  const int TotalCount = pCount + cCount + aCount;

        public static readonly List<Track> active;
        
        
        public static void PoolReset()
        {
            active.Clear();
            
            pingPongPool.Reset();
            circularPool.Reset();
                 arcPool.Reset();
        }
        
        
        public static Track GetNewTrack(elementType trackType)
        {
            Track track = null;
            switch (trackType)
            {
                case elementType.PingPong:  track = pingPongPool.GetFree(); break;
                case elementType.Circular:  track = circularPool.GetFree(); break;
                case elementType.Arc:       track =      arcPool.GetFree(); break;
            }

            active.Add(track);
            return track;
        }
        
        
        protected static float TurnAngle(Vector2 a, Vector2 b)
        {
            Vector2 aDir   = a.Rot90();
            float   angle  = Vector2.Angle(a, b);
            float   dirDot = Vector2.Dot(aDir, b);

            return dirDot < 0 ? 360 - angle : angle;
        }
        
        
        public static Track Get(int trackID)
        {
            int count = active.Count;
            for (int i = 0; i < count; i++)
                if (active[i].ID == trackID)
                    return active[i];
            
            return null;
        }
    }
}

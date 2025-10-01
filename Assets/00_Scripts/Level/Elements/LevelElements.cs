using System.Text;
using GeoMath;
using UnityEngine;


namespace LevelElements
{
    public abstract partial class Element : PoolObject
    {
        protected Element() { ID = incrementID++; }

        public elementType elementType;
        
        public readonly int ID;
        
        public Bounds2D bounds;
        
        public Side    side = Side.Front;
        public Vector2 rootPos;
        public float depth;
        public float angle;

        public virtual  Element SetType(elementType elementType)
        {
            this.elementType = elementType;

            return this;
        }
        public abstract Element SetRootPos(Vector2 rootPos);
        public abstract Element SetSide(Side side);
        public abstract void Refresh();


        public virtual void Reset()
        {
            //Debug.Log("Resetting " + elementType + " " + ID);
            
            angle = 0;
            depth = 0;
        }
        
        public virtual Vector2 GetPos(float time){ return rootPos; }
        
        public abstract float SqrDistance(Vector2 point, float time);


        public virtual void SetDepth(float depth)
        {
            this.depth = depth;
        }


    //  Debbugging  //
        public abstract void Draw();

        public abstract string GetInfo();

        public string GetIdAndSideInfo()
        {
            return "#" + ID + "#" + (side.front? 0 : 1);
        }
    }
    
    
    public abstract partial class Element
    {
        private static int incrementID;

        public static void ClearAll()
        {
             Item.PoolReset();
            Track.PoolReset();
        }


        public static void GetRidOfEverything()
        {
            int tCount = Track.active.Count;
            for (int i = 0; i < tCount; i++)
                Track.active[i].Reset();
            
            int iCount = Item.active.Count;
            for (int i = 0; i < iCount; i++)
                Item.active[0].Reset();
            
            ClearAll();
        }
                        
        protected static readonly StringBuilder infoBuilder = new StringBuilder(1000);
    }
}


        
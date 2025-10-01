using System;
using UnityEngine;
using GeoMath;


public partial class HouseGen
{
    public struct byte4
    {
        public byte a, b, c, d;

        public byte4(byte a, byte b, byte c, byte d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }
        
        public byte4(int a, int b, int c, int d)
        {
            this.a = (byte) a;
            this.b = (byte) b;
            this.c = (byte) c;
            this.d = (byte) d;
        }
    }
    
    
    [Serializable]
    public partial struct CornerIDs
    {
        public readonly byte  a, b, c, d;
        public readonly short cornerValue;

        
        public CornerIDs(byte a, byte b, byte c, byte d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;

            int aValue = a > 0 ? 1 : 0;
            int bValue = b > 0 ? 1 : 0;
            int cValue = c > 0 ? 1 : 0;
            int dValue = d > 0 ? 1 : 0;

            cornerValue = (short) (aValue * 1000 + bValue * 100 + cValue * 10 + dValue);
        }

        public bool HasZoneChange
        {
            get
            {
                byte compare = a != 0? a : b != 0? b : c != 0? c : d;
                
                if(compare == 0)
                    return false;
                
                return a != 0 && a != compare || 
                       b != 0 && b != compare || 
                       c != 0 && c != compare || 
                       d != 0 && d != compare;
            }
        }

        public bool IsOnEdge { get { return a == 0 || b == 0 || c == 0 || d == 0; }}

        public int ArrayZone { get { return (a - 1) / 2; }}

        public byte Zone     { get { return a != 0 ? a : b != 0 ? b : c != 0 ? c : d; } }

        public bool IsZoneCorner
        {
            get
            {
                return a != b && b == c && c == d ||
                       b != c && c == d && d == a ||
                       c != d && d == a && a == b ||
                       d != a && a == b && b == c;
            }
        }
        
        public bool IsEdgeCorner
        {
            get
            {
                int corners = (a != 0? 1 : 0) + 
                              (b != 0? 1 : 0) + 
                              (c != 0? 1 : 0) + 
                              (d != 0? 1 : 0);
                              
                return corners == 1 || corners == 3;
            }
        }
        
        public bool AllTheSame { get { return a == b && a == c && a == d; }}


        public byte RandomZonePick(System.Random random)
        {
            int offset = random.Range(0, 4);
            
            for (int i = 0; i < 4; i++)
            {
                byte zone;
                switch ((offset + i) % 4)
                {
                    default: zone = a; break;
                    case  1: zone = b; break;
                    case  2: zone = c; break;
                    case  3: zone = d; break;
                }
                
                if(zone != 0)
                    return zone;
            }
            
            return 0;
        }


        public bool BadZoneCorners
        {
            get
            {
                int value = 0;

                if (a != 0)
                    value++;

                if (b != 0 && b != a)
                    value++;
                
                if (c != 0 && c != a && c != b)
                    value++;

                if (d != 0 && d != a && d != b && d != c)
                    value++;
                
                
                return value > 2 || 
                       value == 2 && (a == c && b == d); // ||
                       //solid == 2 && (a == 0 && a == c || b == 0 && b == d);
                       //value == 1 && (a != 0 && c != 0 && b == 0 && d == 0))
                       //a != 0 && a == c && ((b == 0 && (d == 0 || d != a) || (d == 0 && b != a))) ||
                       //b != 0 && b == d && ((a == 0 && (c == 0 || c != b) || (c == 0 && a != b)));
            }
        }


        public bool BadCorners
        {
            get
            {
                return a != 0 && c != 0 && b == 0 && d == 0 || b != 0 && d != 0 && a == 0 && c == 0;
            }
        }


        public short ZoneCornerValue
        {
            get
            {
                byte4 zones = ZoneCorners;

                return (short) (zones.a * 1000 + zones.b * 100 + zones.c * 10 + zones.d);
            }
        }


        public byte4 ZoneCorners
        {
            get
            {
                byte biggest = a > b? a : b;
                biggest = biggest > c? biggest : c;
                biggest = biggest > d? biggest : d;
                 
                return new byte4(a == biggest ? 1 : 0, b == biggest ? 1 : 0, c == biggest ? 1 : 0, d == biggest ? 1 : 0);
            }
        }

        
        public bool FitsDir(CornerIDs id, Vector2Int dir, bool isAir)
        {
            switch (dir.y)
            {
                case -1:
                    switch (dir.x)
                    {
                        case -1: return d != 0? d == id.a && d == id.b && d == id.c && d == id.d : isAir;
                        case  1: return c != 0? c == id.a && c == id.b && c == id.c && c == id.d : isAir;
                        case  0: return c == id.b && d == id.a;
                    }
                    break;
                    
                case 0:
                    switch (dir.x)
                    {
                        case -1: return a == id.b && d == id.c;
                        case  1: return b == id.a && c == id.d;
                    }
                    break;
                    
                case 1:
                    switch (dir.x)
                    {
                        case -1: return a != 0? a == id.a && a == id.b && a == id.c && a == id.d : isAir;
                        case  1: return b != 0? b == id.a && b == id.b && b == id.c && b == id.d : isAir;
                        case  0: return a == id.d && b == id.c;
                    }
                    break;
            }
            
            return false;
        }

        public CornerIDs ProjectDir(int dir = 0)
        {
            switch (dir)
            {
                default: return new CornerIDs(a, b, b, a);    //  Up  //
                case 1 : return new CornerIDs(b, b, c, c);    // Right  //
                case 2 : return new CornerIDs(d, c, c, d);    // Down  //
                case 3 : return new CornerIDs(a, a, d, d);    // Left  //
            }
        }

        public CornerIDs FixSeams()
        {
            if(!HasZoneChange)
                return this;
            
            return new CornerIDs((byte)(a > 0? 1 : 0), (byte)(b > 0? 1 : 0), (byte)(c > 0? 1 : 0), (byte)(d > 0? 1 : 0));
        }

        public string Log()
        {
            return a + "|" + b + "|" + c + "|" + d;
        }
        
        
        public bool Equals(CornerIDs other)
        {
            return a == other.a &&
                   b == other.b &&
                   c == other.c &&
                   d == other.d;
        }
    }
	
    
    [Serializable]
    public struct RectZone
    {
        public Vector2Int min, size;
        private readonly byte frontZone, backZone;

        public bool FrontExtruded { get { return frontZone != 0 && frontZone % 2 == 0; }}
        public bool  BackExtruded { get { return  backZone != 0 &&  backZone % 2 == 0; }}
        public bool  BothExtruded { get { return FrontExtruded && BackExtruded; }}
        
        
        public RectZone(Vector2Int min, Vector2Int size, int zone)
        {
            this.min  = min;
            this.size = size;
            
            frontZone = (byte)zone;
            backZone  = frontZone;
        }
        
        public RectZone(Vector2Int min, Vector2Int size, int frontZone, int backZone)
        {
            this.min  = min;
            this.size = size;
            
            this.frontZone = (byte)frontZone;
            this.backZone  = (byte)backZone;
        }
        
        public byte Zone(bool front = true)
        {
            return front? frontZone : backZone;
        }
        
        public int ArrayZone(bool front = true)
        {
            return (Zone(front) - 1) / 2;
        }

        public RectZone SetZone(int zone)
        {
            return new RectZone(min, size, zone);
        }

        public RectZone Extrude(bool front)
        {
            return new RectZone(min, size, 
                                      front? frontZone % 2 != 0? frontZone + 1 : frontZone : frontZone, 
                                     !front?  backZone % 2 != 0?  backZone + 1 :  backZone : backZone);
        }
        
        public RectZone Level(bool front)
        {
            return new RectZone(min, size, 
                 front? frontZone % 2 == 0? frontZone - 1 : frontZone : frontZone, 
                !front?  backZone % 2 == 0?  backZone - 1 :  backZone : backZone);
        }

        public RectZone SideMargin(int amount, int dir)
        {
            switch (dir)
            {
                default: return new RectZone(min.Add(0, size.y - 1), new Vector2Int(size.x, amount), frontZone, backZone);
                 case 1: return new RectZone(min.Add(size.x - 1, 0), new Vector2Int(amount, size.y), frontZone, backZone);
                 case 2: return new RectZone(min.Add(0, -amount + 1), new Vector2Int(size.x, amount), frontZone, backZone);
                 case 3: return new RectZone(min.Add(-amount + 1, 0), new Vector2Int(amount, size.y), frontZone, backZone);
            }
        }
        
        public int Area { get { return size.x * size.y; }}
        
        public Vector2Int Max { get { return min.Add(size.x - 1, size.y - 1);} }

        public bool Contains(Vector2Int checkPos)
        {
            Vector2Int max = Max;
            return checkPos.x > min.x && checkPos.x < max.x && checkPos.y > min.y && checkPos.y < max.y; 
        }
    }

    
    [Serializable]
    public partial struct Piece
    {
        public Vector2Int  pos;
        public Vector2Int  size;
        public CornerIDs   cornerIDs;
        public PieceType   type;
        public int         sqrArea;
        public int side;

        public Piece(Vector2Int pos, Vector2Int size, CornerIDs cornerIDs, PieceType type, int side)
        {
            this.pos         = pos;
            this.size        = size;
            this.cornerIDs   = cornerIDs;
            this.type        = type;
           
            sqrArea = size.x * size.y;
            
            this.side = side;
        }

        public Piece Grow(Vector2Int dir)
        {
            Vector2Int newPos  =  pos.Add(Mathf.Min(dir.x, 0), Mathf.Min(dir.y, 0));
            Vector2Int newSize = size.Add(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
			
            return new Piece(newPos, newSize, cornerIDs, type, side);
        }

        public Piece(Piece piece, int side)
        {
            pos       = piece.pos;
            size      = piece.size;
            cornerIDs = piece.cornerIDs;
            type      = piece.type;
            
            sqrArea = size.x * size.y;
            
            this.side = side;
        }

        public Bounds2D GetBounds(HouseGen gen)
        {
            return gen.CellAreaToBounds(pos, size);
        }

        public bool Contains(Vector2Int point)
        {
            return point.x >= pos.x && point.x < pos.x + size.x &&
                   point.y >= pos.y && point.y < pos.y + size.y;
        }


        public string Log()
        {
            return cornerIDs.Log() + " - " + size + " - " + type;
        }
        
        
        public static bool operator ==(Piece one, Piece two)
        {
            return one.pos == two.pos &&
                   one.size == two.size &&
                   one.cornerIDs.Equals(two.cornerIDs) &&
                   one.type == two.type &&
                   one.side == two.side;
        }

        public static bool operator !=(Piece one, Piece two)
        {
            return !(one == two);
        }


        public PieceType GetPieceType(bool forceFlat)
        {
            if(forceFlat && !cornerIDs.IsOnEdge)
                return PieceType.Flat;
            
            return type;
        }
    }


    public enum PieceType : byte
    {
        Flat,
        Detail,
        Hole,
        Window
    }

    [Flags]
    public enum Cell
    {
        None            = 0,
        House           = 1 << 1,
        Corner          = 1 << 2,
        Edge            = 1 << 3,
        FrontSeamCorner = 1 << 4,
        BackSeamCorner  = 1 << 5,
        FrontSeam       = 1 << 6,
        BackSeam        = 1 << 7, 
        FrontSeamEdge   = 1 << 8,
        BackSeamEdge    = 1 << 9,
        FrontBlock      = 1 << 10,
        BackBlock       = 1 << 11,
        FrontWindow     = 1 << 12,
        BackWindow      = 1 << 13,
        Hole            = 1 << 14,
        Pillar          = 1 << 15,
        
        Blocked    = FrontBlock | BackBlock,
    }
}


public static class CellHelp
{
    public static void Set(this HouseGen.Cell[] array, HouseGen.Cell info, int index)
    {
        array[index] |= info;
    }

    public static void UnSet(this HouseGen.Cell[] array, HouseGen.Cell info, int index)
    {
        array[index] &= ~info;
    }
    
    public static bool Is(this HouseGen.Cell value, HouseGen.Cell info)
    {
        return (value & info) == info;
    }
    
    public static bool Any(this HouseGen.Cell value, HouseGen.Cell info)
    {
        return (value & info) != 0;
    }
}

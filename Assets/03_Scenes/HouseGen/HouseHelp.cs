using System.Collections.Generic;
using UnityEngine;


namespace House
{
    public static class HouseHelp
    {
        private static readonly Dictionary<int, Vector2Int> cornerDirDict;
        
        static HouseHelp()
        {
            cornerDirDict = new Dictionary<int, Vector2Int>
            {
                { 1000, new Vector2Int( -1, 1) },
                { 0100, new Vector2Int( 1,  1) },
                { 0010, new Vector2Int( 1, -1) },
                { 0001, new Vector2Int(-1, -1) },
                
                { 1101, new Vector2Int( 1, -1) },
                { 1110, new Vector2Int(-1, -1) },
                { 0111, new Vector2Int(-1,  1) },
                { 1011, new Vector2Int( 1,  1) },
                
                { 1100, new Vector2Int( 0, -1) },
                { 0110, new Vector2Int(-1,  0) },
                { 0011, new Vector2Int( 0,  1) },
                { 1001, new Vector2Int( 1,  0) },
                
                { 1111, new Vector2Int( 1,  1) },
                { 0000, new Vector2Int( 0,  0) },
                { 0101, new Vector2Int( 0,  0) },
                { 1010, new Vector2Int( 0,  0) }
            };
        }
       
        
        public static Vector2Int GrowDir(this HouseGen.CornerIDs value)
        {
            return cornerDirDict[value.cornerValue];
        }
        
        
        private static readonly Vector2Int[] Dirs =
        {
            new Vector2Int( 0,  1), 
            new Vector2Int( 1,  1), 
            new Vector2Int( 1,  0), 
            new Vector2Int( 1, -1),
            new Vector2Int( 0, -1), 
            new Vector2Int(-1, -1), 
            new Vector2Int(-1,  0), 
            new Vector2Int(-1,  1)
        };


        public static Vector2Int Dir        (int value) { return Dirs[value % 8]; }
        public static Vector2Int AxisDir    (int value) { return Dirs[value % 4 * 2]; }
        public static Vector2Int DiagonalDir(int value) { return Dirs[value % 4 * 2 + 1]; }


        public static int PieceID(this HouseGen.CornerIDs value, bool fixIt = false)
        {
            if(fixIt)
                value = value.FixSeams();
            
            const int maxZones = 15, max = maxZones * 2 + 1;
            
            //if(false)
                return value.a * max * max * max + 
                       value.b * max * max + 
                       value.c * max + 
                       value.d; 
            
            return (value.a != 0? value.a + 1 : 0) * max * max * max + 
                   (value.b != 0? value.b + 1 : 0) * max * max + 
                   (value.c != 0? value.c + 1 : 0) * max + 
                   (value.d != 0? value.d + 1 : 0); 
        }


        public static int GetPieceID(int a, int b, int c, int d)
        {
            const int maxZones = 15;
            const int max      = maxZones * 2 + 1;
            
            return a * max * max * max + 
                   b * max * max + 
                   c * max + 
                   d; 
        }
    }
}


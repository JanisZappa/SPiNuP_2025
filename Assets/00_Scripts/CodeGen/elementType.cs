using UnityEngine;

namespace LevelElements
{
    public enum elementType
    {
    //  Item  //
        Stick          = 1,
        WarpStickA     = 2,
        WarpStickB     = 3,
        WarpStickC     = 4,
        WarpStickD     = 5,
        StartStick     = 12,
        EndStick       = 13,
        ArrowStick     = 14,
        Bouncy         = 6,
        InOutStick     = 7,
        Branch         = 8,
        Probe          = 9,
        Probe_Orange   = 11,
        Stick_SmallTip = 10,


    //  Collectable  //
        Coin           = 3000000,
        Pill           = 3000001,
        Pill_Green     = 3000002,


    //  Fluff  //
        DebugCube      = 2000000,
        Flower         = 2000001,
        Flower2        = 2000002,
        Flower3        = 2000003,
        Leaf           = 2000006,
        LeafBlue       = 2000007,
        DebugCube2     = 2000004,
        TrackGap       = 2000005,


    //  Track  //
        PingPong       = 1000000,
        Circular       = 1000001,
        Arc            = 1000002
    }


    public static class ElementTypeExt
    {
        public static int InstanceCount(this elementType value)
        {
            switch(value)
            {
                case elementType.Stick:
                case elementType.Bouncy:
                case elementType.Stick_SmallTip: return 50;

                case elementType.WarpStickA:
                case elementType.WarpStickB:
                case elementType.WarpStickC:
                case elementType.WarpStickD:
                case elementType.ArrowStick:
                case elementType.Branch:
                case elementType.Probe:
                case elementType.Probe_Orange:
                case elementType.Flower3:
                case elementType.DebugCube2:     return 20;

                case elementType.StartStick:
                case elementType.EndStick:       return 1;

                case elementType.Coin:           return 400;

                case elementType.Pill:
                case elementType.Pill_Green:     return 200;

                case elementType.DebugCube:
                case elementType.Flower:
                case elementType.Leaf:
                case elementType.LeafBlue:       return 40;

                case elementType.Flower2:        return 30;
            }

            return 0;
        }


        public static float Mass(this elementType value)
        {
            switch(value)
            {
                case elementType.Bouncy:     return 2;

                case elementType.Branch:     return .75f;

                case elementType.Coin:
                case elementType.Pill:
                case elementType.Pill_Green: return .25f;

                case elementType.Flower:
                case elementType.Flower3:
                case elementType.Leaf:       return .2f;

                case elementType.Flower2:    return .22f;

                case elementType.LeafBlue:   return .18f;
            }

            return 1;
        }


        public static float Damp(this elementType value)
        {
            switch(value)
            {
                case elementType.Stick:
                case elementType.WarpStickA:
                case elementType.WarpStickB:
                case elementType.WarpStickC:
                case elementType.WarpStickD:
                case elementType.StartStick:
                case elementType.EndStick:
                case elementType.ArrowStick:
                case elementType.Stick_SmallTip: return 15;

                case elementType.Bouncy:         return 14;

                case elementType.Branch:
                case elementType.Leaf:           return 8;

                case elementType.Probe:          return 5;

                case elementType.Probe_Orange:   return 4;

                case elementType.Coin:
                case elementType.Pill:
                case elementType.Pill_Green:     return 275;

                case elementType.Flower:
                case elementType.Flower2:
                case elementType.Flower3:        return 10;

                case elementType.LeafBlue:       return 7;
            }

            return 1;
        }


        public static float Accel(this elementType value)
        {
            switch(value)
            {
                case elementType.Stick:
                case elementType.WarpStickA:
                case elementType.WarpStickB:
                case elementType.WarpStickC:
                case elementType.WarpStickD:
                case elementType.StartStick:
                case elementType.EndStick:
                case elementType.ArrowStick:
                case elementType.Stick_SmallTip: return .05f;

                case elementType.Bouncy:
                case elementType.Coin:
                case elementType.Pill:
                case elementType.Pill_Green:     return .3f;

                case elementType.Branch:         return -0.05f;

                case elementType.Probe:          return -0.15f;

                case elementType.Probe_Orange:   return -0.17f;

                case elementType.Flower:
                case elementType.Flower2:
                case elementType.Flower3:
                case elementType.Leaf:
                case elementType.LeafBlue:       return -0.2f;
            }

            return 1;
        }


        public static float Lazyness(this elementType value)
        {
            switch(value)
            {
                case elementType.Coin:
                case elementType.Pill:
                case elementType.Pill_Green:
                case elementType.Flower:
                case elementType.Flower2:    return .175f;

                case elementType.Flower3:
                case elementType.Leaf:
                case elementType.LeafBlue:   return .195f;
            }

            return 1;
        }


        public static float Radius(this elementType value)
        {
            switch(value)
            {
                case elementType.StartStick:
                case elementType.EndStick:
                case elementType.ArrowStick:
                case elementType.Branch:       return .17f;

                case elementType.Bouncy:
                case elementType.Probe:
                case elementType.Probe_Orange: return 1;

                case elementType.Coin:
                case elementType.Pill:
                case elementType.Pill_Green:   return 1.25f;

                case elementType.DebugCube:
                case elementType.DebugCube2:   return .5f;

                case elementType.Flower:
                case elementType.Flower2:
                case elementType.Flower3:      return .7f;

                case elementType.Leaf:
                case elementType.LeafBlue:     return .8f;
            }

            return .075f;
        }


        public static int Value(this elementType value)
        {
            switch(value)
            {
                case elementType.Coin:       return 100;

                case elementType.Pill:       return 200;

                case elementType.Pill_Green: return 500;
            }

            return 0;
        }


        public static elementType GetOtherEnd(this elementType value)
        {
            switch(value)
            {
                case elementType.WarpStickA: return elementType.WarpStickB;
                case elementType.WarpStickB: return elementType.WarpStickA;
                case elementType.WarpStickC: return elementType.WarpStickD;
                case elementType.WarpStickD: return elementType.WarpStickC;
            }

            return value;
        }


        public static int MaxItems(this elementType value)
        {
            switch(value)
            {
                case elementType.Circular: return 20;
            }

            return 1;
        }


        public static float DefaultVScale(this elementType value)
        {
            switch(value)
            {
                case elementType.PingPong: return 10;

                case elementType.Circular:
                case elementType.Arc:      return 5;
            }

            return 0;
        }


        public static Color DebugColor(this elementType value)
        {
            switch(value)
            {
                case elementType.WarpStickA:
                case elementType.WarpStickB:
                case elementType.WarpStickC:
                case elementType.WarpStickD: return COLOR.green.lime;

                case elementType.Bouncy:
                case elementType.Circular:   return COLOR.red.tomato;

                case elementType.PingPong:   return COLOR.yellow.fresh;
            }

            return COLOR.blue.cornflower;
        }


        public const int LongestItemName        = 14;
        public const int LongestCollectableName = 10;
        public const int LongestFluffName       = 10;
        public const int LongestTrackName       = 8;


        public static string Name(this elementType value)
        {
            switch(value)
            {
                case elementType.Stick:          return "Stick";
                case elementType.WarpStickA:     return "WarpStickA";
                case elementType.WarpStickB:     return "WarpStickB";
                case elementType.WarpStickC:     return "WarpStickC";
                case elementType.WarpStickD:     return "WarpStickD";
                case elementType.StartStick:     return "StartStick";
                case elementType.EndStick:       return "EndStick";
                case elementType.ArrowStick:     return "ArrowStick";
                case elementType.Bouncy:         return "Bouncy";
                case elementType.InOutStick:     return "InOutStick";
                case elementType.Branch:         return "Branch";
                case elementType.Probe:          return "Probe";
                case elementType.Probe_Orange:   return "Probe_Orange";
                case elementType.Stick_SmallTip: return "Stick_SmallTip";
                case elementType.Coin:           return "Coin";
                case elementType.Pill:           return "Pill";
                case elementType.Pill_Green:     return "Pill_Green";
                case elementType.DebugCube:      return "DebugCube";
                case elementType.Flower:         return "Flower";
                case elementType.Flower2:        return "Flower2";
                case elementType.Flower3:        return "Flower3";
                case elementType.Leaf:           return "Leaf";
                case elementType.LeafBlue:       return "LeafBlue";
                case elementType.DebugCube2:     return "DebugCube2";
                case elementType.TrackGap:       return "TrackGap";
                case elementType.PingPong:       return "PingPong";
                case elementType.Circular:       return "Circular";
                case elementType.Arc:            return "Arc";
            }

            return "???";
        }


    }
}

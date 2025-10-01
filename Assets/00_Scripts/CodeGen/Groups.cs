using LevelElements;

public static class Groups
{
    static Groups()
    {
        Stick   = new Group(new []{ elementType.Stick,
                                    elementType.StartStick,
                                    elementType.EndStick,
                                    elementType.ArrowStick,
                                    elementType.Stick_SmallTip });

        Probes  = new Group(new []{ elementType.Probe,
                                    elementType.Probe_Orange });

        Pills   = new Group(new []{ elementType.Coin,
                                    elementType.Pill,
                                    elementType.Pill_Green });

        DebugCube = new Group(new []{ elementType.DebugCube,
                                    elementType.DebugCube2 });

        Flower  = new Group(new []{ elementType.Flower,
                                    elementType.Flower2,
                                    elementType.Flower3 });

        Leaves  = new Group(new []{ elementType.Leaf,
                                    elementType.LeafBlue });

        All = new[] { Stick, Probes, Pills, DebugCube, Flower, Leaves };
    }


    private static readonly Group Stick, Probes, Pills, DebugCube, Flower, Leaves;
    public static readonly Group[] All;


    public static Group Alias(this elementType elementType)
    {
        switch(elementType)
        {
            default: return null;

            case elementType.Stick:
            case elementType.StartStick:
            case elementType.EndStick:
            case elementType.ArrowStick:
            case elementType.Stick_SmallTip: return Stick;

            case elementType.Probe:
            case elementType.Probe_Orange: return Probes;

            case elementType.Coin:
            case elementType.Pill:
            case elementType.Pill_Green: return Pills;

            case elementType.DebugCube:
            case elementType.DebugCube2: return DebugCube;

            case elementType.Flower:
            case elementType.Flower2:
            case elementType.Flower3: return Flower;

            case elementType.Leaf:
            case elementType.LeafBlue: return Leaves;
        }
    }
}

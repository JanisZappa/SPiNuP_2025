


/*public static class ElementInfo
{
    private const info Stick          = info.IsItem | info.Shake | info.Grab | info.Button;
    private const info WarpStickA     = info.IsItem | info.Shake | info.Grab | info.Button;
    private const info WarpStickB     = info.IsItem | info.Shake | info.Grab;
    private const info WarpStickC     = info.IsItem | info.Shake | info.Grab | info.Button;
    private const info WarpStickD     = info.IsItem | info.Shake | info.Grab;
    private const info Bouncy         = info.IsItem | info.Shake | info.Button | info.Action;
    private const info InOutStick     = info.IsItem | info.Shake;
    private const info Branch         = info.IsItem | info.Shake | info.Grab | info.Button;
    private const info Probe          = info.IsItem | info.Shake | info.Button;
    private const info Stick_SmallTip = info.IsItem | info.Shake | info.Grab | info.Button;

    private const info Coin = info.IsCollectable | info.Button;

    private const info DebugCube  = info.IsFluff | info.Hide | info.Button;
    private const info Flower     = info.IsFluff | info.Shake | info.Hide | info.Button;
    private const info Flower2    = info.IsFluff | info.Shake | info.Hide | info.Button;
    private const info Flower3    = info.IsFluff | info.Shake | info.Hide | info.Button;
    private const info DebugCube2 = info.IsFluff | info.Hide | info.Button;
    private const info TrackGap   = info.IsFluff | info.Hide;

    private const info PingPong = info.IsTrack | info.Button;
    private const info Circular = info.IsTrack | info.Button;
    private const info Arc      = info.IsTrack | info.Button;




    public static info Info(this elementType elementType)
    {
        switch(elementType)
        {
            case elementType.Stick          : return Stick;
            case elementType.WarpStickA     : return WarpStickA;
            case elementType.WarpStickB     : return WarpStickB;
            case elementType.WarpStickC     : return WarpStickC;
            case elementType.WarpStickD     : return WarpStickD;
            case elementType.Bouncy         : return Bouncy;
            case elementType.InOutStick     : return InOutStick;
            case elementType.Branch         : return Branch;
            case elementType.Probe          : return Probe;
            case elementType.Stick_SmallTip : return Stick_SmallTip;

            case elementType.Coin : return Coin;

            case elementType.DebugCube  : return DebugCube;
            case elementType.Flower     : return Flower;
            case elementType.Flower2    : return Flower2;
            case elementType.Flower3    : return Flower3;
            case elementType.DebugCube2 : return DebugCube2;
            case elementType.TrackGap   : return TrackGap;

            case elementType.PingPong : return PingPong;
            case elementType.Circular : return Circular;
            case elementType.Arc      : return Arc;
        }

        return 0;
    }




    public static bool AnyMatch(this info a, info b)
    {
        return (a & b) != 0;
    }

    public static bool Matches(this info a, info b)
    {
        return b == (a & b);
    }




    public static bool AnyMatch(this elementType a, info b)
    {
        return a.Info().AnyMatch(b);
    }

    public static bool Matches(this elementType a, info b)
    {
        return a.Info().Matches(b);
    }
}*/

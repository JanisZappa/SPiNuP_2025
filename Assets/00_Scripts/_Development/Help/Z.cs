using UnityEngine;


public static class Z
{
    public static float W => Wall(.00f);
    public static float W05 => Wall(.05f);
    public static float W10 => Wall(.10f);
    public static float W15 => Wall(.15f);
    public static float W20 => Wall(.20f);
    public static float W25 => Wall(.25f);
    public static float W30 => Wall(.30f);
    public static float W35 => Wall(.35f);
    public static float W40 => Wall(.40f);

    public static float P => Plane(0);
    public static float P75 => Plane(.75f);

    public static float M => Max(0);


    private static float Wall(float offset)
    {
        return (Level.WallDepth + offset) * GameCam.CurrentSide.Sign;
    }

    private static float Plane(float offset)
    {
        return (Level.WallDepth + Level.PlaneOffset + offset) * GameCam.CurrentSide.Sign;
    }
    
    private static float Max(float offset)
    {
        return (Level.WallDepth + Level.PlaneOffset + Level.PlaneOffset + offset) * GameCam.CurrentSide.Sign;
    }
    
    public static float WallZ(float offset)
    {
        return (Level.WallDepth + offset) * GameCam.CurrentSide.Sign;
    }
    
    public static float PlaneZ(float offset)
    {
        return (Level.WallDepth + offset) * GameCam.CurrentSide.Sign;
    }
}
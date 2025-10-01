using UnityEngine;


public static class LightingSet
{
    public static Vector3 ShadowDir;
    public static Color   SunColor;
    public static Quaternion SunRot, BounceRot;
    public static Vector2 SunXFactor;
    
    private static Vector3 SunDir;
    private static Color FogColor, SkyColor;
    private static float SunIntensity, ShadowBrightness, CastShadowVis;
    
    
    private static readonly int Side               = Shader.PropertyToID("Side");
    private static readonly int SideMulti          = Shader.PropertyToID("SideMulti");
    private static readonly int WallCastSideMulti  = Shader.PropertyToID("WallCastSideMulti");
    private static readonly int MegaSideMulti      = Shader.PropertyToID("MegaSideMulti");
    
    private static readonly int Wall               = Shader.PropertyToID("Wall");
    private static readonly int GamePlane          = Shader.PropertyToID("GamePlane");
    private static readonly int ContactAdd         = Shader.PropertyToID("ContactAdd");
    
    private static readonly int Dir                = Shader.PropertyToID("ShadowDir");
    private static readonly int SunAngle           = Shader.PropertyToID("SunAngle");
    private static readonly int SunFactors         = Shader.PropertyToID("SunFactors");
    private static readonly int Color              = Shader.PropertyToID("SunColor");
    
    private static readonly int AntiSunColor       = Shader.PropertyToID("AntiSunColor");
    private static readonly int SkyColor1          = Shader.PropertyToID("SkyColor");
    private static readonly int FogColor1          = Shader.PropertyToID("FogColor");
    private static readonly int Ambient            = Shader.PropertyToID("Ambient");
    
    private static readonly int ShadowColor        = Shader.PropertyToID("ShadowColor");
    private static readonly int ShadowVis          = Shader.PropertyToID("CastShadowVis");
    
    
    public static void SetSceneLighting(_SceneLighting sceneLighting)
    {
        SetSunRot(Quaternion.Euler(sceneLighting.sunAngle.x, sceneLighting.sunAngle.y, 0));
        
        SunColor     = sceneLighting.sunColor;
        SunIntensity = sceneLighting.sunIntensity;
        
        Color sunColor     = SunColor * SunIntensity;
        Color antiSunColor = new Color(Mathf.Max(0, 1 - sunColor.r), Mathf.Max(0, 1 - sunColor.g), Mathf.Max(0, 1 - sunColor.b));
        
        Shader.SetGlobalColor(Color,        sunColor);
        Shader.SetGlobalColor(AntiSunColor, antiSunColor);
        
        SkyColor = sceneLighting.skyColor;
        Shader.SetGlobalColor(SkyColor1, SkyColor);
        
        FogColor = sceneLighting.fogColor;
        Shader.SetGlobalColor(FogColor1, sceneLighting.fogColor);

        ShadowBrightness = sceneLighting.shadowBrightness;
        CastShadowVis    = sceneLighting.shadowVisibility;

        RenderSettings.ambientLight = sceneLighting.ambientColor;
        Shader.SetGlobalColor(Ambient, sceneLighting.ambientColor);

        
    //  Shadow Colors  //
        Shader.SetGlobalFloat(ShadowVis, CastShadowVis * (GameCam.CurrentSide.front? .6f : .85f));
        
        const float shadowMulti = .2f;
        Vector3 shadowB = new Vector3(Mathf.Clamp01(ShadowBrightness * 2 * SkyColor.r) * shadowMulti,
                                      Mathf.Clamp01(ShadowBrightness * 2 * SkyColor.g) * shadowMulti,
                                      Mathf.Clamp01(ShadowBrightness * 2 * SkyColor.b) * shadowMulti);
        
        Shader.SetGlobalVector(ShadowColor, shadowB);
        
        
        UpdateShader(GameCam.CurrentSide.front, !Application.isPlaying);
    }
    
    
    public static void SetShadow(bool front)
    {
        Vector3 mirroredSun = Vector3.Reflect(SunDir, V3.forward);
        ShadowDir = front? SunDir : new Vector3(mirroredSun.x *.25f, mirroredSun.y, mirroredSun.z);
        Shader.SetGlobalVector(Dir, -ShadowDir);

        BounceRot = Quaternion.FromToRotation(Vector3.up, new Vector3(-ShadowDir.x, -ShadowDir.y).normalized);
    }


    public static void UpdateShader(bool front, bool none = false)
    {
        float side = none? -1 : front ? -1 : 1;
        
        Shader.SetGlobalFloat(Side,              side);
        Shader.SetGlobalFloat(SideMulti,         side * .5f + .5f);
        Shader.SetGlobalFloat(WallCastSideMulti, .65f + .35f * Mathf.Clamp01(-Side));
        Shader.SetGlobalFloat(MegaSideMulti,     1 + .1f * Mathf.Clamp01(Side));
        Shader.SetGlobalFloat(Wall,              none? .001f : Level.WallDepth);
        Shader.SetGlobalFloat(GamePlane,         none? 1     : Level.WallDepth + Level.PlaneOffset);
        Shader.SetGlobalFloat(ContactAdd,        .125f - .025f * Mathf.Clamp01(side));
    }
    
    
    public static void GetSceneLight(_SceneLighting sceneLighting)
    {
        sceneLighting.sunAngle         = SunRot.eulerAngles;
        sceneLighting.sunColor         = SunColor;
        sceneLighting.sunIntensity     = SunIntensity;
        sceneLighting.skyColor         = SkyColor;
        sceneLighting.ambientColor     = RenderSettings.ambientLight;
        sceneLighting.fogColor         = FogColor;
        sceneLighting.shadowBrightness = ShadowBrightness;
        sceneLighting.shadowVisibility = CastShadowVis;
    }


    public static void SetSunRot(Quaternion rot)
    {
        SunRot = rot;
        SunDir = SunRot * V3.forward;
        Shader.SetGlobalVector(SunAngle, -SunDir);
        
        SunXFactor = -(SunDir * (1f / Mathf.Abs(SunDir.z)));
        Shader.SetGlobalVector(SunFactors, SunXFactor);

        SetShadow(GameCam.CurrentSide.front);
    }


    public static Vector2 GetDepthOffset(float itemDepth)
    {
        float multi = itemDepth / MapCam.mapSizeA * 5;
        return new Vector2(SunXFactor.x * -Mathf.Sign(itemDepth) * multi, SunXFactor.y * multi);
    }
}

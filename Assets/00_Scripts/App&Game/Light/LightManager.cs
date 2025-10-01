using UnityEngine;


public class LightManager : MonoBehaviour
{
    public MapCam shadowCam, lightCam;
    
    private static readonly BoolSwitch Shadows = new("Visuals/Shadows", true);
    private static readonly BoolSwitch Lights  = new("Visuals/Lights", true);
    
    private static SceneLightLerp lightLerp;
    public static MaterialPropertyBlock PropBlock;
    
    
    private void Awake()
    {
        lightLerp = Resources.Load<SceneLightLerp>("Light/DefaultLightLerp");
        PropBlock = new MaterialPropertyBlock();
        transform.GetChild(0).GetComponent<SpriteRenderer>().GetPropertyBlock(PropBlock);
    }

    private void OnEnable()
    {
        GameCam.OnNewCamSide += GameCamOnOnNewCamSide;
    }
    
    private void OnDisable()
    {
        GameCam.OnNewCamSide -= GameCamOnOnNewCamSide;
    }
    

    private static void GameCamOnOnNewCamSide(bool front)
    {
        LightingSet.SetShadow(front);
        LightingSet.UpdateShader(front);
    }


    public void LateUpdate()
    {       
        if(Shadows != shadowCam.active)
            shadowCam.SetActive(Shadows);
        
        if(Lights != lightCam.active)
            lightCam.SetActive(Lights);
    }


    public static void SetSunAngleLerp(Quaternion rot, float lerp)
    {
        lightLerp.SetLerpedLighting(lerp, rot);
    }
}

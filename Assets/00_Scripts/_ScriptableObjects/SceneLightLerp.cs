using UnityEngine;


[CreateAssetMenu]
public class SceneLightLerp : ScriptableObject 
{
    [System.Serializable]
    public class SettingsLerp
    {
        private static _SceneLighting blendLighting;
        public static _SceneLighting BlendLighting
        {
            get
            {
                if(blendLighting == null)
                    blendLighting = Resources.Load<_SceneLighting>("Light/Blend"); 
                
                return blendLighting;
            }
        }

        public float t;
        public _SceneLighting a;
        public int pow;
        public bool inversePow;
    }

    public SettingsLerp[] lerps;


    public void SetLerpedLighting(float lerp)
    {
        LerpSet(lerp);
        
        LightingSet.SetSceneLighting(SettingsLerp.BlendLighting);
    }
    
    
    public void SetLerpedLighting(float lerp, Quaternion sunRot)
    {
        LerpSet(lerp);   
            
        SettingsLerp.BlendLighting.sunAngle = sunRot.eulerAngles;
        LightingSet.SetSceneLighting(SettingsLerp.BlendLighting);
        LightingSet.SetSunRot(sunRot);
    }


    private void LerpSet(float lerp)
    {
        int count = lerps.Length;
        SettingsLerp low = lerps[0];
        for (int i = 0; i < count; i++)
        {
            SettingsLerp sL = lerps[i];
            if (sL.t >= lerp)
            {
                if(i == 0)
                    SettingsLerp.BlendLighting.Set(sL.a);
                else
                {
                    lerp = Mathf.InverseLerp(low.t, sL.t, lerp);

                    if (low.inversePow)
                        lerp = 1 - Mth.IntPow(1 - lerp, low.pow);
                    else
                        lerp = Mth.IntPow(lerp, low.pow);
                
                    SettingsLerp.BlendLighting.SetBlendBetween(low.a, sL.a, lerp);
                }
                break;
            }
            
            low = sL;
        }
    }
    
    public static void SceneChangeSet()
    {
        Resources.Load<SceneLightLerp>("Light/DefaultLightLerp").SetLerpedLighting(.5f);
    }
}

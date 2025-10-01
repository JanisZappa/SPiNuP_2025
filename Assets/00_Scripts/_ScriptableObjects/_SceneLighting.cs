using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(_SceneLighting))]
public class SceneLightingEditor : Editor
{
      public override void OnInspectorGUI()
     {
         _SceneLighting myTarget = (_SceneLighting)target;

         if (GUILayout.Button("Override from Scene"))
         {
             myTarget.OverrideFromScene();
             EditorUtility.SetDirty(myTarget);
         }
             

         EditorGUILayout.Space(); 
         DrawDefaultInspector();
         EditorGUILayout.Space();
         

         if(GUILayout.Button("Set To Scene"))
         {
             myTarget.SetToScene();
             EditorUtility.SetDirty(myTarget);
         }
     }
}
#endif


[CreateAssetMenuAttribute(menuName = "Scriptable/SceneLighting")]
public class _SceneLighting : ScriptableObject
{
    [HideInInspector] public Vector2 sunAngle;
    
    public Color sunColor;
    public float sunIntensity;

    [Space(10)]
    public Color skyColor;
    public Color ambientColor;
    public Color fogColor;

    [Space(10)]
    [Range(0, 1)]
    public float shadowBrightness;

    [Range(0, 1)] 
    public float shadowVisibility = 1;


    public void OverrideFromScene()
    {
        LightingSet.GetSceneLight(this);
    }


    public void SetToScene()
    {
        LightingSet.SetSceneLighting(this);
    }


    public void SetBlendBetween(_SceneLighting a, _SceneLighting b, float lerp)
    {
        sunColor     = Color.Lerp(a.sunColor, b.sunColor, lerp);
        sunIntensity = Mathf.Lerp(a.sunIntensity, b.sunIntensity, lerp);
        
        skyColor     = Color.Lerp(a.skyColor, b.skyColor, lerp);
        ambientColor = Color.Lerp(a.ambientColor, b.ambientColor, lerp);
        fogColor     = Color.Lerp(a.fogColor, b.fogColor, lerp);
        
        shadowBrightness = Mathf.Lerp(a.shadowBrightness, b.shadowBrightness, lerp);
        shadowVisibility = Mathf.Lerp(a.shadowVisibility, b.shadowVisibility, lerp);

        sunAngle = Vector2.Lerp(a.sunAngle, b.sunAngle, lerp);
    }
    
    public void Set(_SceneLighting a)
    {
        sunColor     = a.sunColor;
        sunIntensity = a.sunIntensity;
        
        skyColor     = a.skyColor;
        ambientColor = a.ambientColor;
        fogColor     = a.fogColor;
        
        shadowBrightness = a.shadowBrightness;
        shadowVisibility = a.shadowVisibility;

        sunAngle = a.sunAngle;
    }
}

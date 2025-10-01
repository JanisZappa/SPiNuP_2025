using UnityEngine;

[CreateAssetMenu]
public class HouseZone : ScriptableObject 
{
    public float minArea, xShift;

    [Space] 
    public Color debugColor;
    
    public bool windows;
    
    [Space]
    public PadRules extrudePadRules;
    
    [Space]
    public float elementPad;
    

    [System.Serializable]
    public struct PadRules
    {
        public int top, sides, bottom;
    }
}

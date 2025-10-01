using UnityEngine;


public class BendLine : MonoBehaviour 
{
    [Range(-1,1)]
    public float circleBend;
    [Range(0, 1)]
    public float pivot;

    [Space(10)]
    public float lineLength;
   
    private const int segments = 100;

    
    public void Awake()
    {
        DRAW.Enabled    = true;
        DRAW.EditorDraw = true;
    }

    
    private void Update()
    {
        Vector2 lineStart = V2.down * lineLength * .5f;
        Vector2 lineEnd   = V2.up * lineLength * .5f;
        
        DRAW.Line(segments, i => GetPosOnCircle(lineStart, lineEnd, (float) i / (segments - 1), circleBend, pivot)).SetColor(COLOR.red.tomato);
        DRAW.Vector(V2.down * lineLength * .25f + V2.left * lineLength * .25f, V2.up * lineLength * .5f).SetColor(Color.cyan);
    }
    

    private static Vector2 GetPosOnCircle(Vector2 lineStart, Vector2 lineEnd, float lineLerp, float circleLerp, float pivot)
    {
        if ( f.Same(circleLerp, 0) )
            return Vector3.Lerp(lineStart, lineEnd, lineLerp);
        
        float   length  = (lineEnd - lineStart).magnitude;
        float   radius  = 1 / circleLerp / 2 / Mth.π * length;
        Vector2 center  = new Vector2((lineStart.x + lineEnd.x) / 2 + radius, lineStart.y + length * (1 - pivot));
        float   segment = circleLerp * (1 - lineLerp - pivot) * Mth.π * 2;

        return center + new Vector2(Mathf.Cos(Mth.π + segment), Mathf.Sin(Mth.π + segment)) * radius;
    }
}
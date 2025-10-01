using UnityEngine;


public class CastShadow : ActorShadow
{
    public float scale = 1;


    protected override void UpdateTransform() 
    {
        Vector3 shadowDir    = LightingSet.ShadowDir;
        Vector3 tipOffset    = actor.TipOffset;
        float   vectorMulti  = Mathf.Abs(tipOffset.z / Mathf.Abs(shadowDir.z));
        Vector2 offset       = new Vector2(shadowDir.x * vectorMulti - tipOffset.x, shadowDir.y * vectorMulti - tipOffset.y);

        _transform.rotation = VectorRot(offset);
        
        const float multi = 1f / (256f / 20);
        _transform.localScale = new Vector3(scale * .95f, offset.magnitude * multi * scale, scale);
        
        //Debug.Log(depthOffset);
        _transform.localPosition = depthOffset;
    }
}

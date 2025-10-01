using UnityEngine;


public class ProbeShadow : ActorShadow 
{
    [Space(10)] 
    public float scale = 1;

    private Vector3 oldShadowDir;
    
    
    protected override void UpdateTransform () 
    {
        Vector3 shadowDir    = LightingSet.ShadowDir;
        if (oldShadowDir != shadowDir)
        {
            oldShadowDir = shadowDir;
            
            Vector3 tipOffset   = Vector3.forward * Level.PlaneOffset;
            float   vectorMulti = Mathf.Abs(tipOffset.z / Mathf.Abs(shadowDir.z));
            Vector2 offset      = new Vector2(shadowDir.x * vectorMulti - tipOffset.x, shadowDir.y * vectorMulti - tipOffset.y);

            _transform.localPosition = offset * .8f;
            _transform.rotation = VectorRot(offset);

            const float multi = 1f / (128f / 20);
            _transform.localScale = new Vector3(scale, scale + offset.magnitude * multi * scale, scale);
        }
    }
}

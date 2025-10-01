using UnityEngine;


public class CoinShadow : Shadow 
{
    [Space(10)] 
    public  float     scale = 1;
    public  Transform actor;
    private Vector3   oldShadowDir, offset;


    protected override void UpdateTransform () 
    {
        Vector3 shadowDir = LightingSet.ShadowDir;
        if (oldShadowDir != shadowDir)
        {
            Vector3 tipOffset   = Vector3.forward * Level.PlaneOffset;
            float   vectorMulti = Mathf.Abs(tipOffset.z / Mathf.Abs(shadowDir.z));

            offset       = new Vector2(shadowDir.x * vectorMulti - tipOffset.x, shadowDir.y * vectorMulti - tipOffset.y);
            oldShadowDir = shadowDir;

            _transform.localPosition = offset * .8f;
            _transform.rotation      = VectorRot(offset);
        }


        Vector3 pos = actor.position;
        
        float animOffset    = pos.x * 2 + pos.y * 2;
        float squeeze       = GTime.Now * GTime.LoopMulti * 32 + animOffset * 4 / 360 * 4;
        float squeezeFactor = Mathf.SmoothStep(1f, .7f, Mathf.Pow(Mathf.PingPong(squeeze, 1), 1.5f));
        float xDot          = Mathf.SmoothStep(0, 1, Mathf.Abs((_transform.rotation * V2.right).x));
        
        
        const float multi = 1f / (128f / 20);
        _transform.localScale = new Vector3( scale * Mathf.Lerp(1, squeezeFactor, xDot), 
                                            (scale + offset.magnitude * multi * scale) * Mathf.Lerp(squeezeFactor, 1, xDot), 
                                             scale );
    }
}

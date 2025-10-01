using System.Collections;
using UnityEngine;


public class bouncy : MonoBehaviour 
{
    private int impactNumber;
    public Vector2 Position;

    
    public void OnEnable()
    {
        Position = transform.position;
    }

    
	public void Impact(Vector2 impactV)
    {
        impactNumber++;
        StartCoroutine(SwingAnim(impactV));
    }

	
    private IEnumerator SwingAnim(Vector2 impactV)
    {
        int iNumber = impactNumber;

        float impactV_M = impactV.magnitude;

        float duration = impactV_M *.05f;
        float interval = impactV_M * .4f;

        Vector2 amplitude = impactV * .02f;

        float t = 0;
        while(iNumber == impactNumber && t < duration)
        {
            t += Time.deltaTime * collisiontest.SlowMoFactor;

            Vector2 swing = Mth.SmoothPP(t * interval + .5f) * amplitude;
            float dampLerp = t / duration;

            transform.position = Position + Vector2.Lerp(swing, V2.zero, dampLerp);
            yield return null;
        }
    }
}

using UnityEngine;


public class SceneryEyes : MonoBehaviour 
{
    public float range;
    public AnimationCurve eyeAnim;
    public float speed;
	private Vector3 startPos;


	private void Awake() 
    {
        startPos = transform.position;
	}

	private void Update () 
    {
        float sin = Mathf.Sin(GTime.Now * speed);
        float pos = Mathf.Sign(sin) * eyeAnim.Evaluate(Mathf.Abs(sin)) * range;
        transform.position = startPos + new Vector3(pos, 0, 0);
	}
}

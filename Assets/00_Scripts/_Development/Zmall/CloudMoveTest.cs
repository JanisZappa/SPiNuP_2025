using UnityEngine;


public class CloudMoveTest : MonoBehaviour
{
    [Range(0,1)]    
    public float offset;
    
    private void Update ()
    {
	    const float range = 220;
	    transform.position = V3.right * Mathf.Lerp(-range, range, Mathf.Repeat(GTime.Now * .01f + offset, 1));
	}
}

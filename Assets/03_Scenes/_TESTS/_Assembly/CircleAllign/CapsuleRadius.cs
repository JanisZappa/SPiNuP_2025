using UnityEngine;


public class CapsuleRadius : MonoBehaviour
{
    public CircleAllign allign;

    private void Update () 
    {
		transform.localScale = new Vector3(allign.animRadius * 2, transform.localScale.y, allign.animRadius * 2);
	}
}

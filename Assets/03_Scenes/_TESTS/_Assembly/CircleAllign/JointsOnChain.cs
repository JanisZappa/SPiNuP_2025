using UnityEngine;


public class JointsOnChain : MonoBehaviour
{
    public Transform[] joints;
    public CircleAllign allign;

    private Quaternion[] defaultRotations;

    [Space(10)] public float speed;
	

    private void Awake()
    {
        defaultRotations = new Quaternion[joints.Length];
        for (int i = 0; i < defaultRotations.Length; i++)
            defaultRotations[i] = joints[i].rotation;
    }

	
    private void Update ()
    {
        float angle = Time.realtimeSinceStartup * speed;
	    float distance = Mathf.Lerp(2, 4, Mathf.Abs(Mathf.Sin(angle / 360 * 2 * Mathf.PI))) * .5f;
        
	    Vector3[] points = allign.GetPointsOnCircle(joints.Length, 1, distance);
	    for (int i = 0; i < joints.Length; i++)
	    {
	        joints[i].position = points[i];
	        Quaternion rotA = i < joints.Length - 1 ? Quaternion.FromToRotation(Vector3.up, points[i + 1] - points[i]) 
	            : Quaternion.FromToRotation(Vector3.up, points[i] - points[i - 1]);

	        Quaternion rotB = Rot.Y(angle);
	        joints[i].rotation = rotA * rotB * defaultRotations[i];
	    }
	}
}

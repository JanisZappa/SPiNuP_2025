using UnityEngine;


public class AxisSway : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float speed, range, pow;
    
    private float t;
    
    
    private void Update()
    {
        t += Time.deltaTime * speed;
        float s = Mathf.Sin(t);
        s = (1f - Mathf.Pow(1f - Mathf.Abs(s), pow)) * Mathf.Sign(s);
        transform.localRotation = Quaternion.AngleAxis(s * range, axis);
    }
}

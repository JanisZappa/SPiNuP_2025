using UnityEngine;


public class RigHelp : MonoBehaviour
{
    [System.Serializable]
    public class Part
    {
        public Transform tip;
        public Transform root;
        public float     length;
        public Vector3   localPos;

        
        public void Setup()
        {
            localPos = root.localPosition;
            length   = Vector3.Distance(root.position, tip.position);
        }
    }

    public Part[] parts;
    
    [Space(10)]
    public Vector2      size;
    public CircleAllign allign;

    [Space(10)]
    [Range(-180f, 180f)]
    public float startAngle;
    [Range(-180f, 180f)]
    public float zRot;
    
    

    private void Awake()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].Setup();
            parts[i].root.GetChild(0).GetComponent<MeshFilter>().mesh.Colorize();
        }
    }

    
    private void LateUpdate()
    {
        float      distance  = Mathf.SmoothStep(size.x, size.y, Mathf.PingPong(zRot, 90) / 90f) * .5f;
        Quaternion zRotation = Rot.Y(zRot);
        
        for (int i = 0; i < parts.Length; i++)
        {
        //  zRot:     Rotate local RootPos  //
            Vector3 rotatedPartPos = zRotation * parts[i].localPos;

        //  Get Offset from main radius  //
            float radiusOffset = distance + rotatedPartPos.x;
            
            Vector3[] points = GetPointsOnCircle(parts[i].length, radiusOffset, rotatedPartPos.y, startAngle);
            
        //  Position Piece at Point 0  //
            parts[i].root.localPosition = points[0] + Vector3.forward * rotatedPartPos.z;
            
        //  Aim Piece At Point 1  //
            parts[i].root.rotation = Quaternion.FromToRotation(Vector3.up, points[1] - points[0]) * zRotation;
        }
    }


    private Vector3[] GetPointsOnCircle(float segmentLength, float radiusOffset, float startoffset, float startAngle)
    {
        Vector3[] returnPoints = new Vector3[2];
        
        float radius = allign.animRadius + radiusOffset;

        float angleA = GetAngleOnCircle(startoffset, radius);
        float angleB = GetAngleOnCircle(startoffset + segmentLength, radius);
        
        returnPoints[0] = V3.up.RotZ(startAngle + angleA, radius);
        returnPoints[1] = V3.up.RotZ(startAngle + angleB, radius);
		
        return returnPoints;
    }

    
    private static float GetAngleOnCircle(float length, float radius)
    {
        return 2 * Mathf.Asin(length / 2 / radius) * Mathf.Rad2Deg;
    }
}


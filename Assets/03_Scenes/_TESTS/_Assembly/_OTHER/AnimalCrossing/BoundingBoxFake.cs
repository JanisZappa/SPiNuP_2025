using UnityEngine;


public class BoundingBoxFake : MonoBehaviour 
{
	void Awake () {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.bounds = new Bounds( Vector3.zero, 1000f * Vector3.one );
	}
}

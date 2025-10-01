using UnityEngine;


public class MeshDrawTest : MonoBehaviour
{
	public Mesh mesh;

	private Vector3[] vertices;
	private int[]     triangles;
	
	
	private void Awake()
	{
		vertices  = mesh.vertices;
		triangles = mesh.triangles;
	}

	
	private void Update()
	{
		DRAW.Polygons(vertices, triangles, transform).SetColor(COLOR.red.tomato);
	}
}

using ShapeStuff;
using UnityEngine;


public class FillMesher : MonoBehaviour 
{
	public MeshFilter   meshFilter;
	public MeshRenderer meshRenderer;
	
	private readonly Vector3[] vertices = new Vector3[short.MaxValue], 
		                       normals  = new Vector3[short.MaxValue];
	
	private Mesh mesh;
	
	
	private void Awake()
	{
		mesh = new Mesh();
		meshFilter.mesh = mesh;

		mesh.vertices  = vertices;

		for (int i = 0; i < normals.Length; i++)
			normals[i] = Vector3.back;
		
		mesh.normals   = normals;
	}
	
	
	public void SetShape(Shape shape)
	{
		if (!gameObject.activeInHierarchy)
			return;
		
		if (!shape.loop || shape.Intersects(shape))
		{
			if (meshRenderer.enabled)
				meshRenderer.enabled = false;
			return;
		}

		if (!meshRenderer.enabled)
			meshRenderer.enabled = true;
		
		
		ShapeTriangulator.FillShape(shape, .09f);

		for (int i = 0; i < ShapeTriangulator.pointCount; i++)
			vertices[i] = ShapeTriangulator.pnts[i];
		
		mesh.vertices  = vertices;
		
		mesh.triangles = ShapeTriangulator.triangles.ToArray();
		
		mesh.RecalculateBounds();
	}
}

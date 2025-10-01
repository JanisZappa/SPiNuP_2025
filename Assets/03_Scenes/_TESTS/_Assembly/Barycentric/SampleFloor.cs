using System.Collections.Generic;
using GeoMath;
using UnityEngine;


public class SampleFloor : MonoBehaviour 
{
	private class TriColor
	{
		private readonly Vector2 v1, v2, v3;
		private readonly Color   c1, c2, c3;

		public TriColor(Vector2 v1, Vector2 v2, Vector2 v3, Color c1, Color c2, Color c3)
		{
			this.v1 = v1;
			this.v2 = v2;
			this.v3 = v3;
			this.c1 = c1;
			this.c2 = c2;
			this.c3 = c3;
		}

		
		public bool PickedColor(Vector2 p, Material material)
		{
			Vector3 w = Tri.BaryWeight(v1, v2, v3, p);

			if (w.x >= 0 && w.y >= 0 && w.z >= 0)
			{
				Color c = c1 * w.x + c2 * w.y + c3 * w.z;
				material.color = c;
				return true;
			}

			return false;
		}
	}
	
	
	private TriColor[] triColors;

	public Transform sphere;
	public Material material;
	

	private void Start ()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		
		Vector3[] verts  = mesh.vertices;
		Color[]   colors = mesh.colors;
		int[]     tris   = mesh.triangles;

		List<TriColor> tC = new List<TriColor>();
		for (int i = 0; i < tris.Length; i+=3)
			tC.Add(new TriColor(transform.TransformPoint(verts[tris[i]]    ).V2UseZ(), 
				                transform.TransformPoint(verts[tris[i + 1]]).V2UseZ(), 
				                transform.TransformPoint(verts[tris[i + 2]]).V2UseZ(), 
				                colors[tris[i]], colors[tris[i + 1]], colors[tris[i + 2]]));

		triColors = tC.ToArray();
	}
	

	private void Update ()
	{
		Vector2 p = sphere.position.V2UseZ();

		for (int i = 0; i < triColors.Length; i++)
			if (triColors[i].PickedColor(p, material))
				return;

		material.color = Color.gray;
	}
}

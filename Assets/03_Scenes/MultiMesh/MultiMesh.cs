using UnityEngine;


public class MultiMesh : MonoBehaviour
{
	private class CachedMesh
	{
		public readonly Vector3[] vertices;
		public readonly Vector3[] normals;
		public readonly Vector2[] uv;
 		public readonly int[]     triangles;

		public CachedMesh(Mesh mesh)
		{
			vertices  = mesh.vertices;
			normals   = mesh.normals;
			uv        = mesh.uv;
			triangles = mesh.triangles;
		}
	}


	public  GameObject[] sourceObjects;
	private CachedMesh[] _cachedMeshes;


	private MeshFilter meshFilter;
	private Mesh       mesh;

	private int verticeCount;
	private int triangleCount;
	private int meshPick;

	private readonly Vector3[] vertices  = new Vector3[ushort.MaxValue];
	private readonly Vector3[] normals   = new Vector3[ushort.MaxValue];
	private readonly Vector2[] uv        = new Vector2[ushort.MaxValue];

	private float t;
	

	private void OnEnable()
	{
		_cachedMeshes = new CachedMesh[sourceObjects.Length];

		for (int i = 0; i < _cachedMeshes.Length; i++)
		{
			_cachedMeshes[i] = new CachedMesh(sourceObjects[i].GetComponent<MeshFilter>().mesh);
			sourceObjects[i].SetActive(false);
		}

		mesh            = new Mesh { bounds = new Bounds(transform.position, Vector3.one) };
		meshFilter      = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		
		
		UpdateMesh();
		t = Time.realtimeSinceStartup;
	}


	private void Update()
	{
		if (Time.realtimeSinceStartup - t > .2f)
		{
			meshPick = (meshPick + 1) % _cachedMeshes.Length;
			UpdateMesh();
			t = Time.realtimeSinceStartup;
		}
		transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * 20, transform.up);
	}


	private void UpdateMesh()
	{
		CachedMesh cachedMesh = _cachedMeshes[meshPick];

		for (int i = 0; i < cachedMesh.vertices.Length; i++)
		{
			vertices[i] = cachedMesh.vertices[i];
			normals[i]  = cachedMesh.normals[i];
			uv[i]       = cachedMesh.uv[i];
		}
		
		mesh.vertices  = vertices;
		mesh.triangles = cachedMesh.triangles;
		mesh.normals   = normals;
		mesh.uv        = uv;
	}
}

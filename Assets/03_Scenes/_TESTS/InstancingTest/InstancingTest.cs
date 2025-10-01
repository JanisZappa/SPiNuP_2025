using UnityEngine;


public class InstancingTest : MonoBehaviour
{
	public MeshFilter[] filters;
	
	public Material material;

	private BaseMesh[] baseMeshes;
	
	private Matrix4x4[] animMatrixes;


	private void Awake()
	{
		baseMeshes = new BaseMesh[filters.Length];
		for (int i = 0; i < baseMeshes.Length; i++)
			baseMeshes[i] = new BaseMesh(filters[i]);
		
		animMatrixes = new Matrix4x4[100];
	}

	private void Update ()
	{
		Quaternion starSpin = Quaternion.AngleAxis(Time.realtimeSinceStartup * 45, Vector3.forward);
		
		for (int i = 0; i < baseMeshes.Length; i++)
		{
			Vector3 pos = baseMeshes[i].pos;
			Quaternion rot = baseMeshes[i].rot;

			for (int e = 0; e < animMatrixes.Length; e++)
			{
				const float step = 3.7f;
				float y = Mathf.Floor(e / 10f) * -step + 5 * step;
				float x = e % 10 * step - 5 * step;
				
				Quaternion tilt = Quaternion.AngleAxis(Mathf.SmoothStep(-13, 13, Mathf.PingPong(Time.realtimeSinceStartup * .5f + e * .22f, 1)), Vector3.right);

				if (i == 1)
					tilt *= starSpin;

				Vector3 animPos = new Vector3(x, y, 0) + tilt * pos;
				
				tilt *= rot;

				animMatrixes[e] = Matrix4x4.TRS(animPos, tilt, Vector3.one);
			}
				
			
			Graphics.DrawMeshInstanced(baseMeshes[i].mesh, 0, material, animMatrixes, animMatrixes.Length);
		}
	}


	private class BaseMesh
	{
		public Mesh mesh;
		public Vector3 pos;
		public Quaternion rot;

		public BaseMesh(MeshFilter filter)
		{
			this.mesh = filter.sharedMesh;
			pos = filter.transform.localPosition;
			rot = filter.transform.localRotation;
		}
	}
}

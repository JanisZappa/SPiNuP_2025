using System.Collections.Generic;
using UnityEngine;


public class NewSpinnerShadow : MonoBehaviour
{
	public MeshFilter mF;

	private Mesh mesh;
	private Vector3 size;
	private bool cast;

	private readonly List<Vector2> uvs    = new List<Vector2> {V2.zero, V2.zero, V2.zero, V2.zero};
	private readonly List<Vector2> uvs1   = new List<Vector2> {V2.zero, V2.zero, V2.zero, V2.zero};
	private readonly List<Vector2> uvs2   = new List<Vector2> {V2.zero, V2.zero, V2.zero, V2.zero};
	private readonly List<Color32> colors = new List<Color32> {Color.white, Color.white, Color.white, Color.white};

	private bool useOffset;
	
	private void Awake()
	{
		mF.mesh = mesh = new Mesh();
		
		mesh.SetVertices(new List<Vector3>{ new Vector2(.5f, -.5f), new Vector2(-.5f, -.5f), new Vector2(.5f, .5f), new Vector2(-.5f, .5f) });
		mesh.SetTriangles(new []{ 0, 1, 2, 2, 1, 3 }, 0);
		
		mesh.SetUVs(0, uvs);
		mesh.SetUVs(1, uvs1);
		mesh.SetUVs(2, uvs2);
		mesh.bounds = new Bounds(V3.zero, new Vector3(1, 1, 0));
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
			useOffset = !useOffset;
	}

	
	public void UpdateTransform(Vector2 pos, float bend, float turn, float angle, Vector3 squash)
	{
		if (cast)
		{
			Vector3 shadowDir   = LightingSet.ShadowDir;
			Vector3 tipOffset   = Vector3.forward * Level.PlaneOffset;
			float   vectorMulti = Mathf.Abs(tipOffset.z / Mathf.Abs(shadowDir.z));
			Vector2 offset      = new Vector2(shadowDir.x * vectorMulti - tipOffset.x, shadowDir.y * vectorMulti - tipOffset.y);
			pos += offset + (useOffset? new Vector2(4, 0) : V2.zero);
		}


		transform.position = pos;
		transform.localScale = V3.one * (size.y * squash.y + 16);
		
		
	//  Center  //
		for (int i = 0; i < 4; i++) uvs[i] = pos;
		mesh.SetUVs(0, uvs);
		
		
	//  Radius + Angle  //
		float radius = 1 / (Mathf.Sign(bend) * Mathf.Max(.01f, Mathf.Abs(bend))) / 2 / Mth.π * size.y;
		
		Vector2 radii = new Vector2(radius, angle);
		for (int i = 0; i < 4; i++)	uvs1[i] = radii;
		mesh.SetUVs(1, uvs1);
		
		
	//  Height + Thickness  //
		float thickLerp = Mth.SmoothPP(size.x * squash.x, size.z * squash.z, Mathf.Repeat(turn, 1) * 4) * .25f;
		Vector2 hT = new Vector2(size.y * squash.y * .5f, thickLerp);
		for (int i = 0; i < 4; i++)	uvs2[i] = hT;
		mesh.SetUVs(2, uvs2);
	}
	
	
	public void Setup(bool cast, Vector3 size)
	{
		this.cast = cast;


		const byte maxVis = 106;
		Color32 pickVis = new Color32((byte)(cast? maxVis : 0), (byte)(cast? 0 : maxVis), 0, 0);
		for (int i = 0; i < 4; i++) colors[i] = pickVis;
		mesh.SetColors(colors);

		this.size = size;
	}
}

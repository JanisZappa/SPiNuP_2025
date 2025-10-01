using System.Collections.Generic;
using UnityEngine;


public class SmoothBindBounce : MonoBehaviour
{
	public Mesh     mesh;
	public Material mat;
	public float    scale  = 1;
	public float collRot;
	
	[Space(10)]
	public int boneCount = 8;
	
	private          Transform[]   bones;
	private readonly List<Vector3> vertices = new List<Vector3>(ushort.MaxValue);


	private float squash = 1;

	private BoxCollider coll;
	private SkinnedMeshRenderer skinnedMeshRenderer;

	
	private readonly List<float> shakeTimes = new List<float>(100);
	
	private Vector3   size;
	private Vector3[] bindPos, pos, scaling;


	public Transform[] testSpheres;
	private Vector3[] testSpherePos;

	
	
	
	private void OnEnable()
	{
		coll = GetComponent<BoxCollider>();
		
		Rig();
		TestSetup();
		
		testSpherePos = new Vector3[testSpheres.Length];
		for (int i = 0; i < testSpherePos.Length; i++)
			testSpherePos[i] = testSpheres[i].localPosition;
	}
	
	
	private void Rig()
	{
		float   min    = mesh.bounds.min.y;
		float   max    = mesh.bounds.max.y;
		float   step   = mesh.bounds.size.y / (boneCount - 1);
		Vector3 bottom = mesh.bounds.center - Vector3.up * mesh.bounds.size.y * .5f;
		
		bones = new Transform[boneCount];
		for (int i = 0; i < boneCount; i++)
		{
			bones[i] = new GameObject().transform;
			bones[i].SetParent(transform, false);
			bones[i].localPosition = bottom + new Vector3(0, step * i, 0);
		}

		skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
		skinnedMeshRenderer.sharedMesh = mesh;
		skinnedMeshRenderer.bones      = bones;
		skinnedMeshRenderer.material   = mat;
		skinnedMeshRenderer.quality = SkinQuality.Bone2;
		skinnedMeshRenderer.Simplify(false);
		
		Matrix4x4[] bindPoses = new Matrix4x4[boneCount];
		for (int i = 0; i < boneCount; i++)
			bindPoses[i] = Matrix4x4.Translate(transform.position - bones[i].position);


		mesh.GetVertices(vertices);
		int verticeCount = vertices.Count;
		
		BoneWeight[] boneWeights = new BoneWeight[verticeCount];
		for (int i = 0; i < verticeCount; i++)
		{
			float lerp     = Mathf.InverseLerp(min, max, vertices[i].y) * (boneCount - 1);
			int   minIndex = Mathf.FloorToInt(lerp);
			int   maxIndex = Mathf.CeilToInt(lerp);
			
			if (minIndex == maxIndex)
			{
				boneWeights[i].boneIndex0 = minIndex;
				boneWeights[i].weight0    = 1;
				continue;
			}
			
			
			float boneAHeight = min + minIndex * step; 
			float boneBHeight = min + maxIndex * step;
			float boneLerp    = Mathf.InverseLerp(boneAHeight, boneBHeight, vertices[i].y);

			bool firstIsCloser = boneLerp < .5f;
			boneWeights[i].boneIndex0 = firstIsCloser? minIndex : maxIndex;
			boneWeights[i].weight0    = firstIsCloser? 1 - boneLerp : boneLerp;
			boneWeights[i].boneIndex1 = firstIsCloser? maxIndex : minIndex;
			boneWeights[i].weight1    = firstIsCloser? boneLerp : 1 - boneLerp;
		}
		
		mesh.bindposes   = bindPoses;
		mesh.boneWeights = boneWeights;
		mesh.MarkDynamic();
	}

	
	private void TestSetup()
	{
		transform.localScale = V3.one * scale;
		
		Vector3 bottom = transform.TransformPoint(mesh.bounds.center - Vector3.up * mesh.bounds.size.y * .5f);
		transform.position += transform.position - bottom;
		
		
		bindPos = new Vector3[boneCount];
		for (int i = 0; i < boneCount; i++)
			bindPos[i] = bones[i].localPosition;
		
		
		pos     = new Vector3[boneCount];
		scaling = new Vector3[boneCount];
		
		size = mesh.bounds.size;
	}




	private void Update()
	{
		if (mesh == null)
			return;


		Squash();


		for (int i = 0; i < boneCount; i++)
		{
			bones[i].localPosition = pos[i];
			bones[i].localScale = scaling[i].ZeroDamp();
		}

		coll.center = skinnedMeshRenderer.localBounds.center / scale;
		coll.size = Rot.Y(collRot) * (skinnedMeshRenderer.bounds.size / scale);
	}

	
	private void Squash()
	{
		squash = UpdateSquash();
		
		Vector3 scaleFactor = size.GetFactors(size.VolumeScaleY(squash));
		Vector3 powFactor   = scaleFactor.Pow(Mathf.PI * .75f).SetY(1) - Vector3.one;
		
		for (int i = 0; i < boneCount; i++)
		{
			float lerp      = -1 + i / (boneCount - 1f) * 2;
			float scaleLerp = Mathf.Pow(Mathf.Cos(Mathf.PI * lerp / 2), .5f).NaNChk();

			Vector3 dir = bindPos[i] - bindPos[0];
			pos[i]     = bindPos[0] + Vector3.Lerp(dir, dir.MultiBy(scaleFactor).SetY(dir.y), scaleLerp).MultiY(scaleFactor.y);
			scaling[i] = Vector3.one + Vector3.Lerp(V3.zero, powFactor, scaleLerp);
		}

		for (int i = 0; i < testSpheres.Length; i++)
		{
			Vector3 dir = testSpherePos[i];
			
			float lerp      = -1 + dir.y / size.y * 2;
			float scaleLerp = Mathf.Pow(Mathf.Cos(Mathf.PI * lerp / 2), .5f).NaNChk();
			
			Vector3 scl = Vector3.one + Vector3.Lerp(V3.zero, powFactor, scaleLerp);
			testSpheres[i].localPosition = Vector3.Lerp(dir, dir.MultiBy(scl).SetY(dir.y), scaleLerp).MultiY(scaleFactor.y);
		}
	}

	
	private float UpdateSquash()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit) && hit.collider == coll)
				shakeTimes.Add(Time.realtimeSinceStartup);
		}
		
		
	//	Trimm Shakes	//
		int count = shakeTimes.Count;
		for (int i = 0; i < count; i++)
			if (shakeTimes[i] < Time.realtimeSinceStartup - 5)
			{
				shakeTimes.RemoveAt(i);
				count--;
				i--;
			}

		float sq = 1;
		for (int i = 0; i < shakeTimes.Count; i++)
			sq += GPhysics.Oscillate(Time.realtimeSinceStartup - shakeTimes[i], 5, 2.3f, 9, .25f) * -.25f;

		return sq.NaNChk(1);
	}
}

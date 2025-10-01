using System.Collections.Generic;
using UnityEngine;


public class SmoothBind : MonoBehaviour
{
	public Mesh     mesh;
	public Material mat;
	public float    scale  = 1;
	public float    amount = 20;
	
	[Space(10)]
	public int boneCount = 8;
	
	[Range(0,1)]
	public float squashAmount = .5f;
	[Range(-90,90)]
	public float bendAmount;

	[Space(10)] 
	public bool autoRig = true;
	
	private          Transform[]   bones;
	private readonly List<Vector3> vertices = new List<Vector3>(ushort.MaxValue);
	
	
	
	private void OnEnable()
	{
		if(autoRig)
			RigIt();
	}

	

	public void RigIt()
	{
		Rig();
		TestSetup();
	}
	
	
	
	private void Rig()
	{
		float   min      = mesh.bounds.min.y;
		float   max      = mesh.bounds.max.y;
		float   fraction = mesh.bounds.size.y / (boneCount - 1);
		Vector3 bottom   = mesh.bounds.center - Vector3.up * mesh.bounds.size.y * .5f;
		
		bones = new Transform[boneCount];
		for (int i = 0; i < boneCount; i++)
		{
			bones[i] = new GameObject().transform;
			bones[i].SetParent(transform, false);
			bones[i].localPosition = bottom + new Vector3(0, fraction * i, 0);
		}

		SkinnedMeshRenderer skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
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
			
			
			float boneAHeight = min + minIndex * fraction; 
			float boneBHeight = min + maxIndex * fraction;
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


	
	private void Update()
	{
		if (mesh == null)
			return;
		
		
		for (int i = 0; i < boneCount; i++)
		{
			pos[i]     = Vector3.zero;
			rot[i]     = Rot.Zero;
			scaling[i] = Vector3.one;
		}


		Squash();
		Turn();
		Twist();
		Bend();

		
		for (int i = 0; i < boneCount; i++)
		{
			bones[i].localPosition = pos[i];
			bones[i].localRotation = rot[i];
			bones[i].localScale    = scaling[i].ZeroDamp();
		}
	}



	private void Turn()
	{
		for (int i = 0; i < boneCount; i++)
			rot[i] = Quaternion.Euler(0, (Time.realtimeSinceStartup + offset * 20) * -80, 0) * rot[i];
	}
	
	
	
	private void Twist()
	{
		float spin = Mathf.Sin(offset + Time.realtimeSinceStartup * 2) * amount;
		const float antiScale = 1f / 360 * -.7f;
		Vector3 twist = new Vector3(Mathf.Abs(spin) * antiScale, 0, Mathf.Abs(spin) * antiScale);
		
		for (int i = 0; i < boneCount; i++)
		{
			rot[i] = Quaternion.Euler(0, spin * i / (boneCount - 1f), 0) * rot[i];
			
			float lerp      = -1 + i / (boneCount - 1f) * 2;
			float scaleLerp = Mathf.Pow(Mathf.Cos(Mathf.PI * lerp / 2), 2.5f).NaNChk();
			
			scaling[i] += Vector3.Lerp(V3.zero, twist, scaleLerp);
		}
	}


	
	private void Squash()
	{
		float   squash      = Mathf.Sin(offset + Time.realtimeSinceStartup * 4) * squashAmount + 1;
		Vector3 scaleFactor = size.GetFactors(size.VolumeScaleY(squash));
		
		float   pow         = Mathf.SmoothStep(Mathf.PI * .5f, 1, Mathf.Abs(squash - 1) * .5f);
		Vector3 powFactor   = scaleFactor.Pow(pow).SetY(1) - Vector3.one;
		
		for (int i = 0; i < boneCount; i++)
		{
			Vector3 dir = bindPos[i] - bindPos[0];
			pos[i] = bindPos[0] + new Vector3(dir.x, dir.y * squash, dir.z);

			float lerp      = -1 + i / (boneCount - 1f) * 2;
			float scaleLerp = Mathf.Pow(Mathf.Cos(Mathf.PI * lerp / 2), .5f).NaNChk();
			
			scaling[i] += Vector3.Lerp(V3.zero, powFactor, scaleLerp);
		}
	}



	private void Bend()
	{
		float   bendFraction = bendAmount / (boneCount - 1);
		Vector3 lastPos   = pos[0];
		Vector3 unaltered = pos[0];
		
		for (int i = 0; i < boneCount; i++)
		{
			Vector3 dir = pos[i] - unaltered;
			Quaternion bendRot = Quaternion.AngleAxis(bendFraction * i, Rot.Y((Time.realtimeSinceStartup + offset) * 36) * Vector3.forward);
			unaltered = pos[i];
			pos[i] = lastPos + bendRot * dir;
			lastPos = pos[i];
			
			rot[i] = bendRot * rot[i];
		}
	}

	
	
	private float        offset;
	private Vector3[]    bindPos;
	private Vector3      size;
	
	private Vector3[]    pos;
	private Quaternion[] rot;
	private Vector3[]    scaling;
	
	
	
	private void TestSetup()
	{
		offset = Random.Range(-.4f, .4f);
		
		transform.localScale = V3.one * scale;
		
		Vector3 bottom = transform.TransformPoint(mesh.bounds.center - Vector3.up * mesh.bounds.size.y * .5f);
		transform.position += transform.position - bottom;
		
		bindPos = new Vector3[boneCount];
		for (int i = 0; i < boneCount; i++)
		{
			bindPos[i] = bones[i].localPosition;
		}

		
		pos     = new Vector3[boneCount];
		rot     = new Quaternion[boneCount];
		scaling = new Vector3[boneCount];
		
		size = mesh.bounds.size;
	}
}

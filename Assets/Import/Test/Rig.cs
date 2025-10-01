using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public partial class Rig : MonoBehaviour
{
	public BodyPartMesh head, eye, mouth, torso, arm, leg;


	private const int maxBones = 100, headParts = 4, bendParts = 5, partCount = bendParts;
	
	public readonly Transform[]    bones      = new Transform[maxBones];
	public readonly Vector3[]      bindPos    = new Vector3[maxBones];
	private readonly List<Color32> readColors = new List<Color32>(10000);
	private readonly List<Vector2> readUVs    = new List<Vector2>(10000);
	
	private readonly List<byte> colorMask = new List<byte>(10000), 
		                        shadeMask = new List<byte>(10000);
	
	private readonly CombineInstance[] combineInstances = new CombineInstance[partCount + headParts];
	
	private readonly Mesh[] sourceMeshes = new Mesh[4];

	public readonly RigHead rigHead = new RigHead();
	public readonly RigPart[] rigParts  = CollectionInit.Array<RigPart>(bendParts);
	
	private SkinnedMeshRenderer skin;
	private Mesh mesh;
	public CostumeColors cC;
	
	public RigPoser poser;
	public RigDebug debug;

	public int boneCount;
	
	
	public void Setup(Spinner spinner)
	{
		poser = new RigPoser(this, spinner);
		debug = new RigDebug(poser);

		
		for (int i = 0; i < bones.Length; i++)
		{
			bones[i] = (Application.isEditor? new GameObject("Bone " + i.ToString("D3")) : new GameObject()).transform;
			bones[i].SetParent(transform);
		}

		
		mesh = new Mesh();
		skin = GetComponent<SkinnedMeshRenderer>();
		
		SkinMesh();
	}


	private void SkinMesh()
	{
		sourceMeshes[0] = head.mesh;
		sourceMeshes[1] = torso.mesh;
		sourceMeshes[2] = arm.mesh;
		sourceMeshes[3] = leg.mesh;


	//  Setup  //
	    Vector3 armRoot = torso.Get(PartType.Arm), 
		        legRoot = torso.Get(PartType.Leg);
	
		Vector3 hands = (armRoot + arm.Get(PartType.Stick)).SetX(0);
		Vector3 feet  = (legRoot + leg.Get(PartType.Stick)).SetX(0);
		Vector3 shift = (feet + hands) * -.5f;
		
		
		const float limbBend = 1; //.333f;
		RigInfo[] rigInfos =
		{
			new RigInfo( torso, shift,                     4, 1),
			new RigInfo(   arm, shift + armRoot,           3, limbBend),
			new RigInfo(   arm, (shift + armRoot).FlipX(), 3, limbBend),
			new RigInfo(   leg, shift + legRoot,           3, limbBend),
			new RigInfo(   leg, (shift + legRoot).FlipX(), 3, limbBend)
		};
		
		
	//  Combine  //
		boneCount = 0;
		
	//  Head  //
	Vector3 headPos = shift + torso.Get(PartType.Head);
	
		RigInfo[] headInfos =
		{
			new RigInfo(head,  headPos, 1, 0),
			new RigInfo(eye,   headPos + head.Get(PartType.Eye),  1, 0),
			new RigInfo(eye,  (headPos + head.Get(PartType.Eye)).FlipX(),  1, 0),
			new RigInfo(mouth, headPos + head.Get(PartType.Mouth),  1, 0)
		};

		for (int i = 0; i < headInfos.Length; i++)
			headInfos[i] = headInfos[i].CombineSetup(ref combineInstances[i]);
		
		rigHead.Setup(headInfos, this);

		for (int i = 0; i < bendParts; i++)
			rigParts[i].Setup(rigInfos[i].CombineSetup(ref combineInstances[i + headParts]), this);
		
		
		mesh.Clear();
		mesh.CombineMeshes(combineInstances);
	
		
	//  Skin  //
		skin.sharedMesh = mesh;
		skin.bones      = bones;
		
		Matrix4x4[] bindPoses = new Matrix4x4[boneCount];
		for (int i = 0; i < boneCount; i++)
			bindPoses[i] = Matrix4x4.Translate(-bindPos[i]);


		mesh.GetUVs(0, readUVs);
		mesh.GetColors(readColors);
		colorMask.Clear();
		shadeMask.Clear();
		int colorCount = readColors.Count;
		
		
		for (int i = 0; i < colorCount; i++)
		{
			Vector2 uv = readUVs[i];
			colorMask.Add((byte)uv.x);
			shadeMask.Add((byte)uv.y);
		}
			

		int verticeCount = mesh.vertices.Length;
		BoneWeight[] boneWeights = new BoneWeight[verticeCount];
		for (int i = 0; i < verticeCount; i++)
		{
			float weight = readColors[i].a / 255f;
			boneWeights[i].boneIndex0 = readColors[i].g;
			boneWeights[i].weight0    = weight;
			boneWeights[i].boneIndex1 = readColors[i].b;
			boneWeights[i].weight1    = 1 - weight;
		}
		
		
		mesh.bindposes   = bindPoses;
		mesh.boneWeights = boneWeights;

		for (int i = boneCount; i < maxBones; i++)
			bones[i].gameObject.SetActive(false);
	
		
		poser.FinishSetup(mesh.bounds.size, hands + shift, feet + shift);
	}


	public void ColorizeMesh()
	{
		CostumeColors before = cC;
		while(cC == before)
			cC = CostumeColors.RandomScheme;
		
		mesh.Colorize(cC, colorMask, shadeMask);
	}


	public void SetCostume(CostumeColors costume)
	{
		cC = costume;
		
		mesh.Colorize(cC, colorMask, shadeMask);
	}
	

	public void ToggleSkin(bool show)
	{
		skin.enabled = show;
	}
}


public struct RigInfo
{
	public readonly PartType partType;
	public readonly Mesh    mesh;
	public readonly Vector3 pos;
	public readonly float   bendyness;
	public readonly int     boneCount;

	
	public RigInfo(BodyPartMesh bodyPart, Vector3 pos, int boneCount, float bendyness)
	{
		switch (bodyPart.partType)
		{
			case PartType.Arm:	partType = pos.x > 0 ? PartType.ArmR : PartType.ArmL;	break;
			case PartType.Leg:	partType = pos.x > 0 ? PartType.LegR : PartType.LegL;	break;
			default:            partType = bodyPart.partType;                           break;
		}
		
		mesh = Object.Instantiate(bodyPart.mesh);
		
		this.pos       = pos;
		this.boneCount = boneCount;
		this.bendyness = bendyness;
	}


	public RigInfo CombineSetup(ref CombineInstance combineInstance)
	{
		combineInstance.mesh      = mesh;
		combineInstance.transform = Matrix4x4.TRS(pos, Rot.Zero, new Vector3(pos.x > 0? -1 : 1, 1, 1));

		return this;
	}
}


public partial class Rig 
{
	private static void MeshExport(Mesh mesh, Transform t, string meshName)
	{
	#if UNITY_EDITOR
			
		StringBuilder sb = new StringBuilder();
		
		sb.Append("#" + meshName + ".obj" + 
				  "\n#" + System.DateTime.Now.ToLongDateString() + 
				  "\n#" + System.DateTime.Now.ToLongTimeString() + 
				  "\n#-------" + "\n\n");
		
		Vector3    pos = t.position;
		Quaternion rot = t.rotation;
		
		t.position = V3.zero;
		t.rotation = Quaternion.identity;
		
		Quaternion r = t.localRotation;

		for (int i = 0; i < mesh.vertices.Length; i++)
		{
			Vector3 v = t.TransformPoint(mesh.vertices[i]);
			sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, -v.z));
		}

		sb.Append("\n");
		for (int i = 0; i < mesh.normals.Length; i++)
		{
			Vector3 v = -(r * mesh.normals[i]);
			sb.Append(string.Format("vn {0} {1} {2}\n", -v.x, -v.y, v.z));
		}

		sb.Append("\n");
		for (var i = 0; i < mesh.uv.Length; i++)
		{
			Vector3 v = mesh.uv[i];
			sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
		}

		int[] triangles = mesh.GetTriangles(0);
		for (int i = 0; i < triangles.Length; i += 3)
		{
			sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
				triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
		}
		
		string fileName = EditorUtility.SaveFilePanel("Export .obj file", "", meshName, "obj");
		using (StreamWriter sw = new StreamWriter(fileName))
			sw.Write(sb.ToString());
		
		t.position = pos;
		t.rotation = rot;
			
	#endif
	}
}
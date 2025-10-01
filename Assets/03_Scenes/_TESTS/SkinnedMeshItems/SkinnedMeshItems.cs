using System;
using System.Collections.Generic;
using LevelElements;
using UnityEngine;


public class SkinnedMeshItems : MonoBehaviour 
{
	[Serializable]
	public class CollectedMeshes
	{
		public elementType type;
		public Mesh[] meshes;
		public Vector3[] offsets;
		public int instCount;

		public CollectedMeshes(elementType type, Mesh[] meshes, Vector3[] offsets, int instCount)
		{
			this.type      = type;
			this.meshes    = meshes;
			this.instCount = instCount;
		}
	}

	public CollectedMeshes[] collectedMeshes;
	public int boneCount;

	public Transform[] bones;
	public MeshFilter filter;
	
	private void Start () 
	{
		List<CollectedMeshes> collected = new List<CollectedMeshes>();
		
		foreach(elementType elementT in Enum.GetValues(typeof(elementType)))
			if (elementT.InstanceCount() > 0)
			{
				GameObject prefab = Resources.Load("Level/Items/" + elementT) as GameObject;
				if (prefab == null)
					continue;
				
				List<Mesh>    meshes  = new List<Mesh>();
				List<Vector3> offsets = new List<Vector3>();
				
				MeshFilter[] meshFilters = prefab.transform.GetComponentsInChildren<MeshFilter>();
				for (int i = 0; i < meshFilters.Length; i++)
				{
					if(meshFilters[i].sharedMesh.name.Contains("Star"))
						continue;
					
					meshes.Add(meshFilters[i].sharedMesh);
					offsets.Add(meshFilters[i].transform.localPosition);
				}
					

				collected.Add(new CollectedMeshes(elementT, meshes.ToArray(), offsets.ToArray(), elementT.InstanceCount()));
			}

		collectedMeshes = collected.ToArray();

		boneCount = 0;
		for (int i = 0; i < collectedMeshes.Length; i++)
			boneCount += collectedMeshes[i].instCount * collectedMeshes[i].meshes.Length;

		int rows = Mathf.FloorToInt(Mathf.Sqrt(boneCount / 16f * 9));
		int rowLength = Mathf.CeilToInt((float)boneCount / rows) - 1;
		bones = new Transform[boneCount];
		for (int i = 0; i < boneCount; i++)
		{
			GameObject bone = new GameObject("Bone" + i.ToString("D4"));
			bone.transform.SetParent(transform, false);
			bones[i] = bone.transform;
		}

		int boneIndex = 0;
		int triOffset = 0;

		List<Vector3> verts   = new List<Vector3>();
		List<Vector3> norms   = new List<Vector3>();
		List<Color>   cols    = new List<Color>();
		List<int>     tris    = new List<int>();
		List<int>     boneRef = new List<int>();

		for (int i = 0; i < collectedMeshes.Length; i++)
		{
			CollectedMeshes collection = collectedMeshes[i];
			
			for (int e = 0; e < collection.meshes.Length; e++)
			{
				Mesh mesh = collection.meshes[e];

				Vector3[] vertices = mesh.vertices;
				Vector3[] normals  = mesh.normals;
				Color[] colors     = mesh.colors;
				int[] triangles    = mesh.triangles;

				int vertCount = vertices.Length;

				for (int f = 0; f < collection.instCount; f++)
				{
					for (int j = 0; j < vertCount; j++)
					{
						verts.Add(vertices[j]);
						norms.Add(normals[j]);
						cols.Add(colors[j]);
						boneRef.Add(boneIndex);
					}

					for (int j = 0; j < triangles.Length; j++)
						tris.Add(triangles[j] + triOffset);

					triOffset += vertCount;
					boneIndex++;
				}
			}
		}
		
		Mesh finalMesh = new Mesh();
		
		SkinnedMeshRenderer mR = GetComponent<SkinnedMeshRenderer>();
		mR.sharedMesh = finalMesh;
		mR.bones      = bones;
		mR.rootBone = transform;
		
		finalMesh.SetVertices(verts);
		finalMesh.SetTriangles(tris, 0);
		finalMesh.SetNormals(norms);
		finalMesh.SetColors(cols);
		finalMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000);
		
		Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
		Matrix4x4[] bindPoses = new Matrix4x4[boneCount];
		for (int i = 0; i < boneCount; i++)
			bindPoses[i] = matrix;

		int totalVerticeCount = verts.Count;
		BoneWeight[] boneWeights = new BoneWeight[totalVerticeCount];
		for (int i = 0; i < totalVerticeCount; i++)
		{
			boneWeights[i].boneIndex0 = boneRef[i];
			boneWeights[i].weight0    = 1;
		}
		
		finalMesh.bindposes   = bindPoses;
		finalMesh.boneWeights = boneWeights;


		for (int i = 0; i < bones.Length; i++)
		{
			int y = Mathf.FloorToInt((float) i / rowLength);
            int x = i % rowLength;
            bones[i].transform.localPosition = new Vector3(x * 3, y * 3, 0);
		}

		filter.mesh = finalMesh;
	}
}

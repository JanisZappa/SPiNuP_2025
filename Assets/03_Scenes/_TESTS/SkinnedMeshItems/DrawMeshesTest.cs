using System;
using System.Collections.Generic;
using LevelElements;
using UnityEngine;


public class DrawMeshesTest : MonoBehaviour 
{
	[Serializable]
	public class CollectedMeshes
	{
		public static Material mat;
		private static int indexAdd;
		
		public elementType type;
		public Mesh[] meshes;
		public Vector3[] offsets;
		public int instCount;

		private readonly Vector3[] positions;

		public CollectedMeshes(elementType type, Mesh[] meshes, Vector3[] offsets, int instCount, int startIndex)
		{
			this.type      = type;
			this.meshes    = meshes;
			this.instCount = instCount;
			this.offsets   = offsets;
			
			positions = new Vector3[instCount];
			for (int i = 0; i < instCount; i++)
			{
				float index = i + startIndex;
				int y = Mathf.FloorToInt(index / 40);
				int x = (i + startIndex) % 40;
				positions[i] = new Vector3(x * 3, y * 3, 0);
			}
		}

		public void Draw()
		{
			for (int i = 0; i < meshes.Length; i++)
			{
				Mesh mesh = meshes[i];
				Vector3 offset = offsets[i];

				for (int e = 0; e < instCount; e++)
				{
					Graphics.DrawMesh(mesh, positions[e] + offset, Quaternion.identity, mat, 0, null, 0, null, false, false, false);
				}
					
			}
				
		}
	}

	public Material mat;
	public CollectedMeshes[] collectedMeshes;
	
	private void Start () 
	{
		List<CollectedMeshes> collected = new List<CollectedMeshes>();

		int startIndex = 0;
		foreach(elementType elementT in Enum.GetValues(typeof(elementType)))
			if (elementT.InstanceCount() > 0)
			{
				GameObject prefab = Resources.Load("Level/Items/" + elementT) as GameObject;
				if (prefab == null)
					continue;

				Transform main = prefab.transform;
				
				List<Mesh>    meshes  = new List<Mesh>();
				List<Vector3> offsets = new List<Vector3>();
				
				MeshFilter[] meshFilters = prefab.transform.GetComponentsInChildren<MeshFilter>();
				for (int i = 0; i < meshFilters.Length; i++)
				{
					meshes.Add(meshFilters[i].sharedMesh);
					offsets.Add(main.InverseTransformPoint(meshFilters[i].transform.position));
				}
					

				collected.Add(new CollectedMeshes(elementT, meshes.ToArray(), offsets.ToArray(), elementT.InstanceCount(), startIndex));
				startIndex += elementT.InstanceCount();
			}

		collectedMeshes = collected.ToArray();
		CollectedMeshes.mat = mat;
	}

	private void Update()
	{
		for (int i = 0; i < collectedMeshes.Length; i++)
			collectedMeshes[i].Draw();
	}
}

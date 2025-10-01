using System.Collections.Generic;
using ShapeStuff;
using UnityEngine;


[System.Serializable]
public class EdgeMesh : ScriptableObject
{
	public float height = 1;
	
	[SerializeField]  private Vector3[] baseVertices, baseNormals;
	[HideInInspector] public int[]     triangles;
	[HideInInspector] public byte[]    colorPick;
	[HideInInspector] public Vector3[] morphVertices, morphNormals;
	
	[HideInInspector] public LerpIndexes[] lerpIndexes;
	
	
	
	public void SetMesh (Mesh mesh)
	{
		baseVertices = mesh.vertices;
		baseNormals  = mesh.normals;
		triangles    = mesh.triangles;
			
		morphVertices = new Vector3[baseVertices.Length];
		morphNormals  = new Vector3[baseNormals.Length];
			
		Quaternion rot = Rot.Y(180);
		for (int i = 0; i < baseVertices.Length; i++)
		{
			baseVertices[i] = rot * baseVertices[i];
			baseNormals[i]  = rot * baseNormals[i];
		}


		for (int i = 0; i < baseVertices.Length; i++)
			if (f.Same(baseVertices[i].y, 0) || f.Same(baseVertices[i].y, 1))
				baseNormals[i] = Vector3.ProjectOnPlane(baseNormals[i], V3.up).normalized;
			
			
		colorPick = new byte[mesh.vertices.Length];
		for (int i = 0; i < baseVertices.Length; i++)
			colorPick[i] = (byte) (baseVertices[i].z < .001f ? 1 : 0);


			
		//	LerpIndexes	 //
		List<LerpIndexesList> lerpIndexesList = new List<LerpIndexesList>();
		for (int i = 0; i < baseVertices.Length; i++)
		{
			float lerp = baseVertices[i].y;
			bool addedIndex = false;
			for (int e = 0; e < lerpIndexesList.Count; e++)
				if (f.Same(lerpIndexesList[e].lerp, lerp))
				{
					lerpIndexesList[e].indexes.Add(i);
					addedIndex = true;
				}
				
			if(!addedIndex)
				lerpIndexesList.Add(new LerpIndexesList(lerp, i));
		}
			
			
		lerpIndexes = new LerpIndexes[lerpIndexesList.Count];
		for (int i = 0; i < lerpIndexes.Length; i++)
			lerpIndexes[i] = new LerpIndexes(lerpIndexesList[i]);
	}


	public void MorphEdge(Shape shape, float startLerp, float lerpStep, float widthMulti, float yFactor)
	{
		int lerpCount = lerpIndexes.Length;
		for (int e = 0; e < lerpCount; e++)
		{
			float shapeLerp    = Mathf.Repeat(startLerp + lerpStep * lerpIndexes[e].lerp, 1);

			int   segmentIndex = shape.GetSegmentIndex(shapeLerp);
			float segmentLerp  = shape.GetSegmentLerp(segmentIndex, shapeLerp);

			Vector3 point = shape.segments[segmentIndex].LerpPos(segmentLerp);
			float   rad   = shape.segments[segmentIndex].LerpRad(segmentLerp);
			Quaternion rot = Rot.Z(rad * Mathf.Rad2Deg);

			int vertCount = lerpIndexes[e].indexes.Length;
			for (int f = 0; f < vertCount; f++)
			{
				int index = lerpIndexes[e].indexes[f];
				morphVertices[index] = point + rot * new Vector3(baseVertices[index].x * widthMulti, 0, baseVertices[index].z);
				morphNormals[index]  = rot * new Vector3(baseNormals[index].x * widthMulti, baseNormals[index].y * yFactor, baseNormals[index].z);
			}
		}
	}
	
	
	public class LerpIndexesList
	{
		public readonly float lerp;
		public readonly List<int> indexes = new List<int>();

		public LerpIndexesList(float lerp, int firstIndex)
		{
			this.lerp = lerp;
			indexes.Add(firstIndex);
		}
	}
		
	
	[System.Serializable]
	public class LerpIndexes
	{
		public float lerp;
		public int[] indexes;

		public LerpIndexes(LerpIndexesList lerpIndexesList)
		{
			lerp    = lerpIndexesList.lerp;
			indexes = lerpIndexesList.indexes.ToArray();
		}
	}
}

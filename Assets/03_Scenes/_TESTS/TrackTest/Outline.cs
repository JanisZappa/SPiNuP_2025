using System.Collections.Generic;
using ShapeStuff;
using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Test
{
	[CreateAssetMenu]
	public class Outline : ScriptableObject
	{
		public Vector3[] vertices, normals;
		public Color32[] colors;
		public int[] lines;

		[Space(10)] public int length;
		public float width;


		private static readonly List<Vector3> verts = new(ushort.MaxValue),
			norms = new(ushort.MaxValue);

		private static readonly List<Color32> cols = new(ushort.MaxValue);
		private static readonly List<int> tris = new(ushort.MaxValue);


		public void GenerateShapeMesh(Shape shape, Mesh mesh, float tesselation, Color32 color = default(Color32),
			float zOffset = 0, Mesh tip = null)
		{
			bool flip = zOffset > 0;

			for (int i = 0; i < colors.Length; i++)
				colors[i] = new Color32(color.r, color.g, color.b, colors[i].a);

			int triLoops = shape.GetTesselation(tesselation) - (shape.loop ? 0 : 1);

			Vector3 z = new Vector3(0, 0, zOffset);

			verts.Clear();
			norms.Clear();
			tris.Clear();
			cols.Clear();

			for (int i = 0; i < shape.segmentCount; i++)
			{
				Shape.Segment segment = shape.segments[i];

				int segmentSteps = segment.GetTesselation(tesselation);
				float lerpStep = 1f / segmentSteps;

				for (int e = 0; e < segmentSteps; e++)
					UseSegmentLerp(segment, lerpStep * e, z, flip);
			}

			//  LastLoop  //
			UseSegmentLerp(shape.segments[shape.segmentCount - 1], 1, z, flip);


			for (int i = 0; i < triLoops; i++)
			{
				int offset = i * length;
				for (int e = 0; e < lines.Length; e += 2)
				{
					int a = offset + lines[e] + length;
					int b = offset + lines[e];
					int c = offset + lines[e + 1];
					int d = offset + lines[e + 1] + length;

					Profiler.BeginSample("TriShit");

					if (!flip)
					{
						tris.Add(a);
						tris.Add(b);
						tris.Add(c);
						tris.Add(c);
						tris.Add(d);
						tris.Add(a);
					}
					else
					{
						tris.Add(c);
						tris.Add(b);
						tris.Add(a);
						tris.Add(a);
						tris.Add(d);
						tris.Add(c);
					}


					Profiler.EndSample();
				}
			}


			//  Add Tips  //
			if (!shape.loop)
			{
				Vector3[] tipV = tip.vertices;
				Vector3[] tipN = tip.normals;
				int[] tipT = tip.triangles;

				//  StartTip  //
				{
					int soFar = verts.Count;

					int count = tipV.Length;
					Vector3 shapePos = shape.GetPoint(0).V3() + z;
					float rad = shape.GetDir(0).ToRadian() + Mth.π * .5f;
					Quaternion rot = Rot.Z(rad * Mathf.Rad2Deg) * Quaternion.AngleAxis(180, Vector3.forward) *
					                 Quaternion.AngleAxis(-90, Vector3.right);
					Matrix4x4 mat = Matrix4x4.TRS(shapePos, rot, new Vector3(1, flip ? -1 : 1, 1));
					Color32 c = new Color32(color.r, color.g, color.b, 255);

					for (int i = 0; i < count; i++)
					{
						verts.Add(mat.MultiplyPoint(tipV[i]));
						norms.Add(rot * tipN[i]);
						cols.Add(c);
					}

					count = tipT.Length;
					for (int i = 0; i < count; i += 3)
					{
						if (!flip)
						{
							tris.Add(tipT[i] + soFar);
							tris.Add(tipT[i + 1] + soFar);
							tris.Add(tipT[i + 2] + soFar);
						}
						else
						{
							tris.Add(tipT[i + 2] + soFar);
							tris.Add(tipT[i + 1] + soFar);
							tris.Add(tipT[i] + soFar);
						}
					}
				}
				//  EndTip  //
				{
					int soFar = verts.Count;

					int count = tipV.Length;
					Vector3 shapePos = shape.GetPoint(1).V3() + z;
					float rad = shape.GetDir(1).ToRadian() + Mth.π * .5f;
					Quaternion rot = Rot.Z(rad * Mathf.Rad2Deg) * Quaternion.AngleAxis(-90, Vector3.right);
					Matrix4x4 mat = Matrix4x4.TRS(shapePos, rot, new Vector3(1, flip ? -1 : 1, 1));
					Color32 c = new Color32(color.r, color.g, color.b, 255);

					for (int i = 0; i < count; i++)
					{
						verts.Add(mat.MultiplyPoint(tipV[i]));
						norms.Add(rot * tipN[i]);
						cols.Add(c);
					}

					count = tipT.Length;
					for (int i = 0; i < count; i += 3)
					{
						if (!flip)
						{
							tris.Add(tipT[i] + soFar);
							tris.Add(tipT[i + 1] + soFar);
							tris.Add(tipT[i + 2] + soFar);
						}
						else
						{
							tris.Add(tipT[i + 2] + soFar);
							tris.Add(tipT[i + 1] + soFar);
							tris.Add(tipT[i] + soFar);
						}
					}
				}
			}


			mesh.Clear();
			mesh.SetVertices(verts);
			mesh.SetNormals(norms);
			mesh.SetColors(cols);
			mesh.SetTriangles(tris, 0);
			mesh.RecalculateBounds();
		}


		public void GenerateShadowMesh(Shape shape, Mesh mesh, float tesselation)
		{
			int triLoops = shape.GetTesselation(tesselation) - (shape.loop ? 0 : 1);

			verts.Clear();
			norms.Clear();
			tris.Clear();
			cols.Clear();

			for (int i = 0; i < shape.segmentCount; i++)
			{
				Shape.Segment segment = shape.segments[i];

				int segmentSteps = segment.GetTesselation(tesselation);
				float lerpStep = 1f / segmentSteps;

				for (int e = 0; e < segmentSteps; e++)
					UseSegmentLerp(segment, lerpStep * e);
			}

			//  LastLoop  //
			UseSegmentLerp(shape.segments[shape.segmentCount - 1], 1);

			for (int i = 0; i < triLoops; i++)
			{
				int offset = i * length;
				for (int e = 0; e < lines.Length; e += 2)
				{
					int a = offset + lines[e] + length;
					int b = offset + lines[e];
					int c = offset + lines[e + 1];
					int d = offset + lines[e + 1] + length;

					Profiler.BeginSample("TriShit");

					tris.Add(a);
					tris.Add(b);
					tris.Add(c);
					tris.Add(c);
					tris.Add(d);
					tris.Add(a);

					Profiler.EndSample();
				}
			}

			mesh.Clear();
			mesh.SetVertices(verts);
			mesh.SetNormals(norms);
			mesh.SetColors(cols);
			mesh.SetTriangles(tris, 0);
			mesh.RecalculateBounds();
		}


		private void UseSegmentLerp(Shape.Segment segment, float lerp)
		{
			Vector3 shapePos = segment.LerpPos(lerp);
			float rad = segment.LerpRad(lerp);
			Quaternion rot = Rot.Z(rad * Mathf.Rad2Deg);
			Matrix4x4 mat = Matrix4x4.TRS(shapePos, rot, Vector3.one);

			for (int v = 0; v < length; v++)
			{
				Profiler.BeginSample("ListShit");

				verts.Add(mat.MultiplyPoint(vertices[v]));
				norms.Add(rot * normals[v]);
				cols.Add(new Color32(0, (byte)(255 - colors[v].a), 0, 0));

				Profiler.EndSample();
			}
		}


		private void UseSegmentLerp(Shape.Segment segment, float lerp, Vector3 z, bool flip)
		{
			Vector3 shapePos = segment.LerpPos(lerp).V3() + z;
			float rad = segment.LerpRad(lerp);
			Quaternion rot = Rot.Z(rad * Mathf.Rad2Deg);
			Matrix4x4 mat = Matrix4x4.TRS(shapePos, rot, new Vector3(1, 1, flip ? -1 : 1));

			for (int v = 0; v < length; v++)
			{
				Profiler.BeginSample("ListShit");

				verts.Add(mat.MultiplyPoint(vertices[v]));
				norms.Add(rot * normals[v]);
				cols.Add(colors[v]);

				Profiler.EndSample();
			}
		}
	}


#if UNITY_EDITOR
[CustomEditor(typeof(Outline))]
public class OutlineEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Get Mesh Edge"))
		{
			string fileName =
 EditorUtility.OpenFilePanel("Select", Application.dataPath + "/03_Scenes/TrackTest/Meshes/", "fbx");

			Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(fileName.Replace(Application.dataPath, "Assets"));
			if(mesh != null)
				GetMeshEdge(mesh, target as Outline, false);
		}
		if (GUILayout.Button("Get Mesh Edge (Colored)"))
		{
			string fileName =
 EditorUtility.OpenFilePanel("Select", Application.dataPath + "/03_Scenes/TrackTest/Meshes/", "fbx");

			Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(fileName.Replace(Application.dataPath, "Assets"));
			if(mesh != null)
				GetMeshEdge(mesh, target as Outline, true);
		}
		GUILayout.EndHorizontal();
	}


	private static void GetMeshEdge(Mesh mesh, Outline outline, bool colored)
	{
		outline.width = mesh.bounds.size.x;
		
		float min = mesh.bounds.min.z;
		
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		int[]     triangles = mesh.triangles;
		Color[]   colors = mesh.colors;
		
		if (colors.Length == 0)
		{
			colors = new Color[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
				colors[i] = new Color(1, 1, 1, 1);
		}
		
		
		List<EdgeSegment> segments = new List<EdgeSegment>();
		for (int i = 0; i < triangles.Length; i += 3)
		{
			int a = triangles[i], 
				b = triangles[i + 1], 
				c = triangles[i + 2];
			
			Vector3 pA = vertices[a], 
				    pB = vertices[b], 
				    pC = vertices[c];
			
			Vector3 nA = normals[a], 
				    nB = normals[b];
			
			bool edgeA = f.Same(pA.z, min), 
				 edgeB = f.Same(pB.z, min), 
				 edgeC = f.Same(pC.z, min);


			if (edgeA && edgeB)
			{
				Vector3 z = pA + Vector3.forward;
				bool switchIt = Vector3.Dot(Vector3.Cross(pA - z, pB - z), nA) > 0;
				
				segments.Add(switchIt? new EdgeSegment(a, b) : new EdgeSegment(b, a));
			}

			if (edgeB && edgeC)
			{
				Vector3 z = pB + Vector3.forward;
				bool switchIt = Vector3.Dot(Vector3.Cross(pB - z, pC - z), nB) > 0;
				
				segments.Add(switchIt? new EdgeSegment(b, c) : new EdgeSegment(c, b));
			}

			if (edgeA && edgeC)
			{
				Vector3 z = pA + Vector3.forward;
				bool switchIt = Vector3.Dot(Vector3.Cross(pA - z, pB - z), nA) > 0;
				
				segments.Add(switchIt? new EdgeSegment(a, c) : new EdgeSegment(c, a));
			}
		}
		
		
		List<EdgeSegment> result = new List<EdgeSegment>();
		List<EdgeSegment> sorted = new List<EdgeSegment>();

		//TODO? CleanThisUP
		
		int chains = 0;
		while (segments.Count > 0)
		{
			sorted.Clear();
			
			EdgeSegment frontier = segments.GetRemoveAt(0);
			sorted.Add(frontier);
			
			bool forward = true;
			while (segments.Count > 0)
			{
				bool foundNewFrontier = false;
				
				for (int i = 0; i < segments.Count; i++)
					if (frontier.Connected(segments[i], forward))
					{
						frontier = segments.GetRemoveAt(i);
					
						if(forward)
							sorted.Add(frontier);
						else
							sorted.Insert(0, frontier);
					
						foundNewFrontier = true;
						break;
					}
				
				if(foundNewFrontier)
					continue;
				
				if(!forward)
					break;
	
				forward = false;
				frontier = sorted[0];
			}

			Debug.Log(chains.ToString().B_Blue());
			chains++;

			while (sorted.Count > 0)
				result.Add(sorted.GetRemoveAt(0));
		}

		
		List<Vector3> verts = new(),
			          norms = new();
	     List<Color32> cols = new List<Color32>();
		
	     
		outline.lines = new int[result.Count * 2];

		Quaternion rot = Rot.X(-90);
		HLS tintColor = new HLS(1, .5f, 1);

		for (int i = 0; i < result.Count; i++)
		{
			outline.lines[i * 2] = verts.Count;
			outline.lines[i * 2 + 1] = verts.Count + 1;
			
			int index = result[i].a;
			
			verts.Add(rot * vertices[index].SetZ(0));
			norms.Add(rot * normals[index].SetZ(0));
			cols.Add(colored? tintColor.SetA(colors[index].r) : colors[index]);

			if (i == result.Count - 1 || !result[i].ConnectedAnySide(result[i + 1]))
			{
				index = result[i].b;
			
				verts.Add(rot * vertices[index].SetZ(0));
				norms.Add(rot * normals[index].SetZ(0));
				cols.Add(colored? tintColor.SetA(colors[index].r) : colors[index]);

				tintColor = tintColor.ShiftHue(.3f);
			}
		}
		
		outline.vertices = verts.ToArray();
		outline.normals = norms.ToArray();
		outline.colors = cols.ToArray();
		
		outline.length = outline.vertices.Length;

		EditorUtility.SetDirty(outline);
	}
	
	
	private struct EdgeSegment
	{
		public readonly int a, b;

		public EdgeSegment(int a, int b)
		{
			this.a = a;
			this.b = b;
		}
		
		
		public bool Connected(EdgeSegment other, bool after)
		{
			return after && b == other.a || !after && a == other.b;
		}
		
		
		public bool ConnectedAnySide(EdgeSegment other)
		{
			return a == other.a || b == other.b || 
			       a == other.b || b == other.a;
		}
	}
}
#endif
}
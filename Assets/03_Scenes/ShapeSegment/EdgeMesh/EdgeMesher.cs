using System.Collections.Generic;
using ShapeStuff;
using UnityEngine;
using UnityEngine.Profiling;


public class EdgeMesher : MonoBehaviour
{
	public ShapeMeshSet shapeMeshSet;
	
	public EdgeMesh topMesh;
	public EdgeMesh sideMesh;
	public EdgeMesh bottomMesh;
	
	
	[Space(10)]
	
	public float lengthMulti;
	public float widthMulti;

	[Space(10)] 
	
	public MeshFilter   meshFilter;
	public MeshRenderer meshRenderer;

	
	private Mesh mesh;

	private readonly List<Vector3> vertices = new List<Vector3>(5000);
	private readonly List<Vector3> normals  = new List<Vector3>(5000);
	private readonly List<Color32> colors   = new List<Color32>(5000);
	private readonly List<int> triangles    = new List<int>(15000);
	
	
	private Shape.Segment.FacingLerp[] facings = new Shape.Segment.FacingLerp[200];
	private int facingCount;
	
	private float topRatio = .5f;
	private float bottomRatio = .5f;
	private Shape shape;


	private float lM, wM, fO;
	private bool fill;
	
	
	private void Awake()
	{
		mesh = new Mesh();
		meshFilter.mesh = mesh;

		lM = lengthMulti;
		wM = widthMulti;
		fO = shapeMeshSet.fillOffset;
		fill = shapeMeshSet.fill;
	}


	public void Update()
	{
		bool input = false;
		if (Input.GetKey(KeyCode.Alpha1))
		{
			topRatio = Mathf.Clamp(topRatio - Time.deltaTime * .5f, .001f, .999f);
			input = true;
		}
		if (Input.GetKey(KeyCode.Alpha3))
		{
			topRatio = Mathf.Clamp(topRatio + Time.deltaTime * .5f, .001f, .999f);
			input = true;
		}
		if (Input.GetKey(KeyCode.Alpha4))
		{
			bottomRatio = Mathf.Clamp(bottomRatio - Time.deltaTime * .5f, .001f, .999f);
			input = true;
		}
		if (Input.GetKey(KeyCode.Alpha6))
		{
			bottomRatio = Mathf.Clamp(bottomRatio + Time.deltaTime * .5f, .001f, .999f);
			input = true;
		}
		if (Input.GetKey(KeyCode.Alpha2))
		{
			topRatio = .5f;
			input = true;
		}
		if (Input.GetKey(KeyCode.Alpha5))
		{
			bottomRatio = .5f;
			input = true;
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			shapeMeshSet.NewSeed();
			input = true;
		}

		if (input && shape != null)
		{
			SetShape(shape);
			return;
		}

		if(!f.Same(lM, lengthMulti) || !f.Same(wM, widthMulti) || !f.Same(fO, shapeMeshSet.fillOffset) || fill != shapeMeshSet.fill)
		{
			lM   = lengthMulti;
			wM   = widthMulti;
			fO   = shapeMeshSet.fillOffset;
			fill = shapeMeshSet.fill;
			SetShape(shape);
			
		}
	}


	public void SetShape(Shape shape)
	{
		if (!gameObject.activeInHierarchy)
			return;
		
		
		if (!shape.loop || shape.Intersects(shape))
		{
			if (meshRenderer.enabled)
				meshRenderer.enabled = false;
			return;
		}

		if (!meshRenderer.enabled)
			meshRenderer.enabled = true;

		this.shape = shape;
		
		vertices.Clear();
		normals.Clear();
		colors.Clear();
		triangles.Clear();
		mesh.SetTriangles(triangles, 0);

		int vIndex = 0;
		//	FillShape	//
		if (fill)
		{
			Profiler.BeginSample("EdgeMesher - Fill");

			ShapeTriangulator.FillShape(shape, shapeMeshSet.fillOffset * widthMulti);
			
			vIndex = ShapeTriangulator.pointCount;
			for (int i = 0; i < vIndex; i++)
			{
				colors.Add(new Color32(1, 0, 0, 0));
				vertices.Add(ShapeTriangulator.pnts[i]);
				normals.Add(Vector3.back);
			}

			int fillTriangleCount = ShapeTriangulator.triangles.Count;
			for (int i = 0; i < fillTriangleCount; i++)
				triangles.Add(ShapeTriangulator.triangles[i]);

			Profiler.EndSample();
		}


		//	Edges	//
		Profiler.BeginSample("EdgeMesher - Edge");
		
		facingCount = shape.GetFacingDirections(topRatio, bottomRatio, ref facings);
		float shapeLength = shape.length;
		float xSign = shape.clockwise ? -1 : 1;

		EdgeMesh gapBefore = shapeMeshSet.GetBridge(facingCount - 1);
		
		for (int i = 0; i < facingCount; i++)
		{
			EdgeMesh gapNext = shapeMeshSet.GetBridge(facingCount - 1);
			float   gapLerpA = gapBefore.height * .5f / shapeLength;
			float   gapLerpB = gapNext.height * .5f / shapeLength;
			
			float sideLerpLength = (i == facingCount - 1 ? facings[0].lerp + 1 : facings[i + 1].lerp) - facings[i].lerp - gapLerpA * .5f - gapLerpB * .5f;
			float sideLength     = shapeLength * sideLerpLength;
			int   steps          = Mathf.Max(1, Mathf.RoundToInt(sideLength / lengthMulti));
			float yFactor        = Mathf.Clamp01(1f / (sideLength / steps));
			float lerpStep       = sideLerpLength / steps;
			
			
			for (int s = 0; s < steps; s++)
			{
				EdgeMesh edge = shapeMeshSet.GetEdge(facings[i].sideID, i, s);
				edge.MorphEdge(shape, facings[i].lerp + gapLerpA * .5f + lerpStep * s, lerpStep, widthMulti * xSign, yFactor);
				
				for (int e = 0; e < edge.morphVertices.Length; e++)
				{
					vertices.Add(edge.morphVertices[e]);
					normals.Add(edge.morphNormals[e]);
					colors.Add(new Color32(edge.colorPick[e], 0, 0, 0));
				}

				if (!shape.clockwise)
					for (int e = 0; e < edge.triangles.Length; e++)
						triangles.Add(edge.triangles[e] + vIndex);
				else
					for (int e = 0; e < edge.triangles.Length; e += 3)
					{
						triangles.Add(edge.triangles[e + 2] + vIndex);
						triangles.Add(edge.triangles[e + 1] + vIndex);
						triangles.Add(edge.triangles[e] + vIndex);
					}

				vIndex += edge.morphVertices.Length;
			}

			{
				gapNext.MorphEdge(shape, facings[i].lerp + gapLerpA * .5f + sideLerpLength, gapLerpB * (1f / gapNext.height), widthMulti * xSign, 1);
				
				for (int e = 0; e < gapNext.morphVertices.Length; e++)
				{
					vertices.Add(gapNext.morphVertices[e]);
					normals.Add(gapNext.morphNormals[e]);
					colors.Add(new Color32(gapNext.colorPick[e], 0, 0, 0));
				}

				if (!shape.clockwise)
					for (int e = 0; e < gapNext.triangles.Length; e++)
						triangles.Add(gapNext.triangles[e] + vIndex);
				else
					for (int e = 0; e < gapNext.triangles.Length; e += 3)
					{
						triangles.Add(gapNext.triangles[e + 2] + vIndex);
						triangles.Add(gapNext.triangles[e + 1] + vIndex);
						triangles.Add(gapNext.triangles[e] + vIndex);
					}

				vIndex += gapNext.morphVertices.Length;
			}

			gapBefore = gapNext;
		}
		
		Profiler.EndSample();
		
		
		//	Assignment to Mesh	//
		Profiler.BeginSample("EdgeMesher - Mesh");
		
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetColors(colors);
		mesh.SetTriangles(triangles, 0);
		
		mesh.RecalculateBounds();
		
		Profiler.EndSample();
	}
}
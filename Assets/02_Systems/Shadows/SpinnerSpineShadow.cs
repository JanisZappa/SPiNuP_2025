using System.Collections.Generic;
using UnityEngine;


public class SpinnerSpineShadow : MonoBehaviour
{
	private static readonly BoolSwitch showShadowMesh = new ("Char/Shadow Mesh", false);
	
	public Sprite sprite;
	public MeshFilter filter;
	public MeshRenderer meshRenderer;
	
	private Mesh mesh;
	
	private readonly int[]     map     = new int[meshPointCount];
	private readonly Vector3[] offsets = new Vector3[meshPointCount];
	
	private readonly List<Vector3> verts = new();
	private readonly List<Vector2> uvs   = new();
	private List<int> triangles;
	
	private float currentHeight;
	
	
	private const int pointCount = 70;
	private readonly Vector3[] castPoints = new Vector3[pointCount];
	
	private readonly Quaternion[] contactRot = new Quaternion[pointCount];

	private const int columns        = 50;
	private const int meshPointCount = (pointCount + 2) * columns;

	
	private void Start()
	{
	    meshRenderer.SetPropertyBlock(LightManager.PropBlock);
		
		mesh = new Mesh();
		filter.mesh = mesh;

		verts.Clear();
		uvs.Clear();
		List<Vector3> normals = new List<Vector3>(meshPointCount);
		List<Color32> colors  = new List<Color32>(meshPointCount);
		for (int i = 0; i < meshPointCount; i++)
		{
			verts.Add(V3.zero);
			  uvs.Add(V2.zero);
			
			normals.Add(V3.back);
			 colors.Add(new Color32(255, 0, 0, 0));
		}
		
		triangles = new List<int>();
		for (int i = 0; i < meshPointCount - columns; i += columns)
			for (int e = 0; e < columns - 1; e++)
			{
				triangles.Add(i + e);
				triangles.Add(i + e + 1);
				triangles.Add(i + e + columns);
				
				triangles.Add(i + e + columns + 1);
				triangles.Add(i + e + columns);
				triangles.Add(i + e + 1);
			}
		
		
		
		mesh.SetVertices(verts);
		mesh.SetNormals(normals);
		mesh.SetTriangles(triangles, 0);
		mesh.SetColors(colors);
		mesh.bounds = new Bounds(V3.zero, V3.one * 100000);

		
	//  Map Vertices to "Bones"  //
		for (int i = 0; i < meshPointCount; i += columns)
		{
			int mapIndex = (int) Mathf.Clamp((float)i / columns - 1, 0, pointCount - 1);

			for (int e = 0; e < columns; e++)
				map[i + e] = mapIndex;
		}
	}
	
	
	public void PosePoints(RigPoser poser)
	{
    //  Update Sprite Size etc if neccesary  //
		if(!f.Same(poser.height, currentHeight))
			CalcSpriteSizes(poser.height);
		
		
		Transform parent = poser.transform;
		float     sign   = Mathf.Sign(poser.transform.position.z);
		
		
	//  Shadow Shift  //
		Vector3 shadowDir = LightingSet.ShadowDir;
		float   zOffset   = Mathf.Abs(parent.position.z) - Level.WallDepth;
		Vector3 castShift = shadowDir * (zOffset * -sign / shadowDir.z);
		
		
		
		float height = poser.height * poser.squashScale.y;
		float step   = height / (pointCount - 1);

		for (int i = 0; i < pointCount; i++)
		{
			Placement plc = poser.GetBendPlacement(new Vector3(0, -height * .5f + step * i, 0));
			castPoints[i] = castShift + parent.TransformPoint(plc.pos);
			contactRot[i] = Quaternion.FromToRotation(V3.up, (parent.rotation * plc.rot * V3.up).SetZ(0).normalized);
		}

		
		
	//  Set Vertices  //
		for (int i = 0; i < meshPointCount; i++)
			verts[i] = castPoints[map[i]] + contactRot[map[i]] * offsets[i];
		
		mesh.SetVertices(verts);
		
		
	//  Mesh Debugging  //
		if (!showShadowMesh)
			return;
		
		float z = (Level.WallDepth + .025f) * sign;
		
		DRAW.Line(castPoints).SetColor(COLOR.red.tomato).SetDepth(z);
		
		Color cA = COLOR.green.spring.A(.5f);
		Color cB = COLOR.yellow.fresh.A(.5f);
		for (int i = 0; i < triangles.Count; i += 3)
		{
			Color c = Color.Lerp(cA, cB, (float) i / triangles.Count);
			DRAW.Vector(verts[triangles[i]], verts[triangles[i + 1]] - verts[triangles[i]]).SetDepth(z).SetColor(c);
			DRAW.Vector(verts[triangles[i + 1]], verts[triangles[i + 2]] - verts[triangles[i + 1]]).SetDepth(z).SetColor(c);
			DRAW.Vector(verts[triangles[i + 2]], verts[triangles[i]] - verts[triangles[i + 2]]).SetDepth(z).SetColor(c);
		}
	}
	
	
	private void CalcSpriteSizes(float height)
	{
		currentHeight = height;

	//  Get Sprite UVs  //
		float minU = float.MaxValue, minV = float.MaxValue, maxU = float.MinValue, maxV = float.MinValue;
		for (int i = 0; i < sprite.uv.Length; i++)
		{
			minU = Mathf.Min(minU, sprite.uv[i].x);
			minV = Mathf.Min(minV, sprite.uv[i].y);
			maxU = Mathf.Max(maxU, sprite.uv[i].x);
			maxV = Mathf.Max(maxV, sprite.uv[i].y);
		}


		float marginX = sprite.textureRect.width / sprite.pixelsPerUnit * .5f;
		float spriteY = sprite.textureRect.height / sprite.pixelsPerUnit * (2.56466f / height);
		float marginY = (spriteY - height) * .5f;
		
		
	//  Top and Bottom  //
		for (int i = 0; i < columns; i++)
		{
			int topIndex = meshPointCount - columns + i;
			
			float xLerp   = (float) i / (columns - 1);
			float xOffset = Mathf.Lerp(marginX, -marginX, xLerp);
			
			offsets[i]        = new Vector3(xOffset, -marginY, 0);
			offsets[topIndex] = new Vector3(xOffset,  marginY, 0);

			float u = Mathf.Lerp(maxU, minU, xLerp);
			uvs[i]        = new Vector2(u, minV);
			uvs[topIndex] = new Vector2(u, maxV);
		}
		
		/*{
			offsets[0]                  = new Vector3( marginX, -marginY, 0);
			offsets[1]                  = new Vector3(-marginX, -marginY, 0);
			offsets[meshPointCount - 2] = new Vector3( marginX,  marginY, 0);
			offsets[meshPointCount - 1] = new Vector3(-marginX,  marginY, 0);

			uvs[0]                  = new Vector2(maxU, minV);
			uvs[1]                  = new Vector2(minU, minV);
			uvs[meshPointCount - 2] = new Vector2(maxU, maxV);
			uvs[meshPointCount - 1] = new Vector2(minU, maxV);
		}*/


		float step      = (spriteY - marginY * 2) / (pointCount - 1);
		float lerpMulti = 1f / spriteY;
		for (int i = 0; i < pointCount; i++)
			for (int e = 0; e < columns; e++)
			{
				int index = columns + i * columns + e;
				
				float xLerp = (float) e / (columns - 1);
				float yLerp = (marginY + i * step) * lerpMulti;
				
				offsets[index] = new Vector3(Mathf.Lerp(marginX, -marginX, xLerp), 0, 0);
				    uvs[index] = new Vector2(Mathf.Lerp(maxU, minU, xLerp), Mathf.Lerp(minV, maxV, yLerp));
			}
		/*{
			
			offsets[2 + i * 2] = new Vector3( marginX, 0, 0);
			offsets[3 + i * 2] = new Vector3(-marginX, 0, 0);

			float v = Mathf.Lerp(minV, maxV, );
			uvs[2 + i * 2] = new Vector2(maxU, v);
			uvs[3 + i * 2] = new Vector2(minU, v);
		}*/
		
		mesh.SetUVs(0, uvs);
	}
}

using UnityEngine;


namespace ShapeStuff
{
	public class ShapeMesher
	{
		private readonly Vector3[] vertices  = new Vector3[ushort.MaxValue];
		private int[] triangles;
		

		public void SetShape(Shape shape)
		{
			const int   tesselation = 10;
			const float width       = .2f, halfWidth   = width * .5f;
			
			int steps       = shape.GetTesselation(tesselation);
			int stepCounter = 0;
			
			for (int i = 0; i < shape.segmentCount; i++)
			{
				int segmentSteps = shape.segments[i].GetTesselation(tesselation);
				float lerpStep = 1f / segmentSteps;
				for (int e = 0; e < segmentSteps; e++)
				{
					vertices[stepCounter * 2]     = shape.segments[i].LerpPos(lerpStep * e, -halfWidth);
					vertices[stepCounter * 2 + 1] = shape.segments[i].LerpPos(lerpStep * e, halfWidth);
					stepCounter++;
				}
			}

			if (!shape.loop)
			{
				vertices[stepCounter * 2]     = shape.segments[shape.segmentCount - 1].LerpPos(1, -halfWidth);
				vertices[stepCounter * 2 + 1] = shape.segments[shape.segmentCount - 1].LerpPos(1, halfWidth);
			}


			int triangleSteps = shape.loop ? steps : steps - 1;
			triangles          = new int[triangleSteps * 6];
			int repeat   = steps * 2;
			
			for (int i = 0; i < triangleSteps; i++)
			{
				triangles[i * 6]     = i * 2             % repeat;
				triangles[i * 6 + 1] = (i + 1) * 2       % repeat;
				triangles[i * 6 + 2] = ((i + 1) * 2 + 1) % repeat;
				triangles[i * 6 + 3] = i * 2             % repeat;
				triangles[i * 6 + 4] = ((i + 1) * 2 + 1) % repeat;
				triangles[i * 6 + 5] = (i * 2 + 1)       % repeat;
			}
		}


		public void DrawTriangles()
		{
			DRAW.Polygons(vertices, triangles).SetColor(COLOR.purple.orchid.A(.3f));
		}
	}
}
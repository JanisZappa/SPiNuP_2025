using ShapeStuff;
using UnityEngine;

namespace Test
{
	public class TrackTest : MonoBehaviour
	{
		public Outline outline;
		[Space(10)] [Range(1, 100)] public int tesselation;
		[Space(10)] public Transform[] stick;

		private readonly Shape shape = new Shape();

		private Mesh genMesh;

		private int t;


		private void OnEnable()
		{
			GetComponent<MeshFilter>().mesh = genMesh = new Mesh();
			GenerateMesh();
		}


		private void GenerateMesh(bool newShape = true)
		{
			if (newShape)
				shape.CreateByNumber(Random.Range(0, 10));

			outline.GenerateShapeMesh(shape, genMesh, tesselation);
			t = tesselation;
		}


		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
				GenerateMesh();

			if (t != tesselation)
				GenerateMesh(false);

			for (int i = 0; i < stick.Length; i++)
			{
				Vector2 pos =
					shape.GetPoint(
						Mathf.Repeat(Time.realtimeSinceStartup / GTime.LoopTime + i * (1f / stick.Length), 1)) * 2.5f;
				stick[i].position = transform.TransformPoint(new Vector3(pos.x, pos.y, 0));
			}
		}
	}
}
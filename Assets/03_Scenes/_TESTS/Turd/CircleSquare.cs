using GeoMath;
using UnityEngine;

namespace Test
{
	public class CircleSquare : MonoBehaviour
	{
		public float radius;

		[Range(0, 1)] public float lerp;

		public bool animate;
		public float speed = 1;

		public float t;

		private readonly Line[] lines = new Line[4];
		private readonly Arc[] arcs = new Arc[4];

		private float dotT;

		[Space(10)] public int id;


		private void Awake()
		{
			dotT = Random.Range(0, 4f);
		}


		private void Update()
		{
			Circle circle = new Circle(Vector2.zero, radius);
			float sideLength = circle.Get_Circumference * .25f;

			if (Input.GetKeyDown(KeyCode.Space))
				dotT = Random.Range(0, 4f);

			dotT += Time.deltaTime;
			HLS dotColor = new HLS(dotT * .25f).SetLuminace(.7f).SetSaturation(.8f);

			//circle.Draw(100).SetColor(LineBlack);
			//DRAW.Rectangle(Vector2.zero, Vector2.one * sideLength).SetColor(LineBlack);


			if (animate)
			{
				t += Time.deltaTime * speed;
				lerp = Mth.SmoothPP(0, 1, t);
			}

			DRAW.Shape shape = DRAW.Shape.Get(101);
			if (lerp < .01f)
			{
				//	Just Draw Lines  //
				lines[0] = new Line(new Vector2(-sideLength * .5f, -sideLength * .5f),
					new Vector2(-sideLength * .5f, sideLength * .5f));
				lines[1] = new Line(new Vector2(-sideLength * .5f, sideLength * .5f),
					new Vector2(sideLength * .5f, sideLength * .5f));
				lines[2] = new Line(new Vector2(sideLength * .5f, sideLength * .5f),
					new Vector2(sideLength * .5f, -sideLength * .5f));
				lines[3] = new Line(new Vector2(sideLength * .5f, -sideLength * .5f),
					new Vector2(-sideLength * .5f, -sideLength * .5f));

				for (int i = 0; i < 100; i++)
				{
					float pointLerp = i * .04f;
					int segment = Mathf.FloorToInt(pointLerp);
					float segmentLerp = pointLerp % 1;
					shape.Set(i, lines[segment].LerpPos(segmentLerp).V3(id * .1f));
				}
			}
			else
			{
				float bend = lerp * -.25f;
				Arc helpArc = new Arc(new Line(new Vector2(0, -sideLength * .5f), new Vector2(0, sideLength * .5f)),
					bend);
				float animRadius = helpArc.Get_ChordLength * .5f + helpArc.Get_Height;


				//	|  //
				arcs[0] = new Arc(
					new Line(new Vector2(-animRadius, -sideLength * .5f), new Vector2(-animRadius, sideLength * .5f)),
					bend);
				arcs[1] = new Arc(
					new Line(new Vector2(-sideLength * .5f, animRadius), new Vector2(sideLength * .5f, animRadius)),
					bend);
				arcs[2] = new Arc(
					new Line(new Vector2(animRadius, sideLength * .5f), new Vector2(animRadius, -sideLength * .5f)),
					bend);
				arcs[3] = new Arc(
					new Line(new Vector2(sideLength * .5f, -animRadius), new Vector2(-sideLength * .5f, -animRadius)),
					bend);

				/*for (int i = 0; i < 4; i++)
					arcs[i].Draw(100).SetColor(LineBlack);*/


				for (int i = 0; i < 100; i++)
				{
					float pointLerp = i * .04f;
					int segment = Mathf.FloorToInt(pointLerp);
					float segmentLerp = pointLerp % 1;
					shape.Set(i, arcs[segment].LerpPos(segmentLerp).V3(id * .1f));
				}
			}

			shape.Copy(0, 100);
			shape.SetColor(dotColor).Fill(1, true);
		}

		private static readonly Color LineBlack = Color.black.A(.2f);
	}
}
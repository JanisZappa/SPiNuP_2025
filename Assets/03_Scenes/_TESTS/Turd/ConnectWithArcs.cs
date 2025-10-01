using GeoMath;
using UnityEngine;

namespace Test
{
	public class ConnectWithArcs : MonoBehaviour
	{
		public Vector2 p1, p2;
		[Space(10)] public float r1;
		public float r2;

		[Space(10)] public bool animate;
		public float animRange;
		private static float animTime;

		[Space(10)] public bool drawPebbles;
		private static bool drawHelpers = true;

		private float s1, s2;

		[Space(10)] [Range(0, 2)] public float multi;
		public bool approximateMulti;


		private void Awake()
		{
			s1 = r1;
			s2 = r2;

			DT.Init();
		}


		private void Update()
		{
			DT.Count = 0;

			if (Input.GetKeyDown(KeyCode.H))
				drawHelpers = !drawHelpers;

			if (animate)
			{
				animTime += Time.deltaTime;
				r1 = s1 + Mth.SmoothPP(-animRange, animRange, animTime * .15f);
				r2 = s2 + Mth.SmoothPP(-animRange, animRange, animTime * .22f);
			}

			SolveThing(new Line(p1, p1 + r1.ToRadDir()), new Line(p2, p2 + r2.ToRadDir()), drawPebbles, ref multi,
				approximateMulti);
		}


		private static void SolveThing(Line line1, Line line2, bool drawPebbles, ref float multi, bool approximateMulti)
		{
			Vector2 p1 = line1.l1, p2 = line2.l1;

			Line perp1 = line1.GetPerpendicular();
			Line perp2 = line2.GetPerpendicular();


			float l1 = 0, l2 = 0, l3 = 0, l4 = 0;

			if (drawHelpers)
			{
				//  Direction Lines  //
				WhiteLine(line1.l1, line1.GetL2(), 100);
				WhiteLine(line2.l1, line2.GetL2(), 100);

				//  Perpendicular Lines  //
				BlackLine(perp1.l1, perp1.GetL2(), 100);
				BlackLine(perp2.l1, perp2.GetL2(), 100);
			}


			Vector2 dir1Perp2Contact, dir2Perp1Contact;
			if (line1.Contact(perp2, out dir1Perp2Contact, true) && line2.Contact(perp1, out dir2Perp1Contact, true))
			{
				Dot(dir1Perp2Contact);
				Dot(dir2Perp1Contact);
				//DRAW.Vector(dir2Perp1Contact, dir1Perp2Contact - dir2Perp1Contact).SetColor(Color.black);

				Vector2 perp1ToP1 = p1 - dir2Perp1Contact;
				Vector2 perp2ToP2 = p2 - dir1Perp2Contact;

				float radius1 = perp1ToP1.magnitude;
				float radius2 = perp2ToP2.magnitude;

				if (DT.Go)
					DT.Get(radius1.ToString("F2")).WorldPos(p1 + perp1ToP1 * -.5f).Color(TextColor).Dir(Dir.Center);
				if (DT.Go)
					DT.Get(radius2.ToString("F2")).WorldPos(p2 + perp2ToP2 * -.5f).Color(TextColor).Dir(Dir.Center);

				Vector2 perp1ToP2 = p2 - dir2Perp1Contact;
				Vector2 perp2ToP1 = p1 - dir1Perp2Contact;

				if (DT.Go)
					DT.Get(perp1ToP2.magnitude.ToString("F2")).WorldPos(p2 + perp1ToP2 * -.5f).Color(TextColor)
						.Dir(Dir.Center);
				if (DT.Go)
					DT.Get(perp2ToP1.magnitude.ToString("F2")).WorldPos(p1 + perp2ToP1 * -.5f).Color(TextColor)
						.Dir(Dir.Center);

				if (approximateMulti)
				{
					multi = ApproxMulti(p1, dir2Perp1Contact - p1, p2, dir1Perp2Contact - p2);
					if (DT.Go) DT.Get("Multi: " + multi.ToString("F3")).Color(TextColor).List_L();
				}

				radius1 *= multi;
				radius2 *= multi;

				Vector2 c1 = p1 + (dir2Perp1Contact - p1) * multi;
				Vector2 c2 = p2 + (dir1Perp2Contact - p2) * multi;

				Line cutLine = new Line(c1, c2);

				if (approximateMulti)
				{
					float angle1 = Vector2.Angle(p1 - c1, c2 - c1);
					if (Vector2.Dot(line1.dir, c2 - c1) < 0)
						angle1 = 360 - angle1;


					float angle2 = Vector2.Angle(p2 - c2, c1 - c2);
					if (Vector2.Dot(-line2.dir, c1 - c2) < 0)
						angle2 = 360 - angle2;

					if (DT.Go) DT.Get("\nAngle1: " + angle1).Color(TextColor).List_L();
					if (DT.Go) DT.Get("Angle2: " + angle2).Color(TextColor).List_L();

					new Arc(new Line(p1, p1 + line1.dir.SetLength(Mth.π * radius1 * (angle1 / 180))), -angle1 / 360, 0)
						.Draw(100).SetColor(Color.black);
					new Arc(new Line(p2, p2 - line2.dir.SetLength(Mth.π * radius2 * (angle2 / 180))), -angle2 / 360, 0)
						.Draw(100).SetColor(Color.black);
				}




				DRAW.Circle(c1, radius1, 200).SetColor(LineBlack);
				DRAW.Circle(c2, radius2, 200).SetColor(LineBlack);

				//WhiteLine(c1, c2);

				/*WhiteLine(c1, dir1Perp2Contact);
				WhiteLine(c2, dir2Perp1Contact);*/

				//DRAW.Circle(dir1Perp2Contact, (p1 - dir1Perp2Contact).magnitude, 200).SetColor(LineBlack);

				Circle a = new Circle(p1, (p1 - p2).magnitude);
				Circle b = new Circle(p2, (p2 - p1).magnitude);

				/*a.Draw(200).SetColor(Color.black.A(.2f));
				  b.Draw(200).SetColor(Color.black.A(.2f));

				Vector2 hitA, hitB;
				int hitCount;
				if (a.Contact(b, out hitCount, out hitA, out hitB))
					BlackLine(hitA, hitB);*/

				/*float p1HitLength = (p1 - dir1Perp2Contact).magnitude;
				float p2HitLength = (p2 - dir2Perp1Contact).magnitude;
				DrawText.Get("Dir1 - HitLength: " + p1HitLength.ToString("F3")).LeftList().Color(TextColor);
				DrawText.Get("Dir2 - HitLength: " + p2HitLength.ToString("F3")).LeftList().Color(TextColor);*/

				//Debug.Log(radius1 + " .. " + radius1 * multi + " .. " + (radius1 * multi) / (dir2Perp1Contact - p1).magnitude);
				//Dot(p1 + (dir2Perp1Contact - p1) * (radius1 / (radius1 + radius2)));

				//Dot(p1 + (dir2Perp1Contact - p1) * (line1.GetAngle(line2) / 90));

				//BlackLine(c1, c2);


				/*Vector2 m1 = p1 + (dir2Perp1Contact - p1) * .5f;
				Vector2 m2 = p2 + (dir1Perp2Contact - p2) * .5f;
				BlackLine(m1, m2);*/

				/*BlackLine(p1, p2);
				BlackLine(dir1Perp2Contact, dir2Perp1Contact);*/




				float dirAverageLength = 0;
				Vector2 dirHit = Vector2.zero;
				Vector2 averageDir = (line1.dir + line2.dir).normalized;
				if (line1.Contact(line2, out dirHit, true))
				{
					Dot(dirHit);

					Line average = new Line(dirHit, dirHit + averageDir);
					WhiteLine(average.l1, average.GetL2(), 100);

					Vector2 cutHit = Vector2.zero;
					if (cutLine.Contact(average, out cutHit, true))
					{
						Dot(cutHit);
					}




					Vector2 perp1Hit, perp2Hit;
					if (average.Contact(perp1, out perp1Hit, true) && average.Contact(perp2, out perp2Hit, true))
					{
						//Dot(perp1Hit);
						//Dot(perp2Hit);

						dirAverageLength = (perp1Hit - perp2Hit).magnitude;
						if (DT.Go)
							DT.Get("\nDirAverageLength: " + dirAverageLength.ToString("F3")).List_L().Color(TextColor);


						float cutRatio = (perp1Hit - cutHit).magnitude / dirAverageLength;
						if (DT.Go) DT.Get("\n!!! Cut Ratio !!!: " + cutRatio.ToString("F3")).List_L().Color(TextColor);
					}

					l1 = (dirHit - p1).magnitude;
					l2 = (dirHit - p2).magnitude;

					if (DT.Go) DT.Get(l1.ToString("F5") + " - P1 To DirHit   ").List_R().Color(TextColor);
					if (DT.Go) DT.Get(l2.ToString("F5") + " - P2 To DirHit   ").List_R().Color(TextColor);
				}


				float perpAverageLength = 0;
				Vector2 perHit = Vector2.zero;
				Vector2 averagePerp = (perp1.dir + perp2.dir).normalized;
				if (perp1.Contact(perp2, out perHit, true))
				{
					Dot(perHit);

					Line average = new Line(perHit, perHit + averagePerp);
					//WhiteLine(average.l1, average.GetL2(), 100);

					Vector2 dirHit1, dirHit2;
					if (average.Contact(line1, out dirHit1, true) && average.Contact(line2, out dirHit2, true))
					{
						//Dot(dirHit1);
						//Dot(dirHit2);

						perpAverageLength = (dirHit1 - dirHit2).magnitude;
						if (DT.Go)
							DT.Get("PerpAverageLength: " + perpAverageLength.ToString("F3")).List_L().Color(TextColor);
					}

					l3 = (perHit - p1).magnitude;
					l4 = (perHit - p2).magnitude;

					if (DT.Go) DT.Get(l3.ToString("F5") + " - P1 To PerpHit").List_R().Color(TextColor);
					if (DT.Go) DT.Get(l4.ToString("F5") + " - P2 To PerpHit").List_R().Color(TextColor);
				}

				DRAW.Circle(dirHit, (perHit - dirHit).magnitude, 300).SetColor(LineHue);
				DRAW.Circle(perHit, (dirHit - perHit).magnitude, 300).SetColor(LineHue);




				float averageRatio = dirAverageLength / perpAverageLength;
				if (DT.Go) DT.Get("AverageRatio: " + averageRatio.ToString("F3")).List_L().Color(TextColor);


				float combined = l1 + l2 + l3 + l4;
				if (DT.Go) DT.Get("\n" + combined.ToString("F5") + " - Combined").List_R().Color(TextColor);
				float p1p2Ratio = (l2 + l4) / combined;
				if (DT.Go) DT.Get(p1p2Ratio.ToString("F5") + " - P1P2 Ratio").List_R().Color(TextColor);
				float dirPerpRatio = (l3 + l4) / combined;
				if (DT.Go) DT.Get(dirPerpRatio.ToString("F5") + " - DirPerp Ratio").List_R().Color(TextColor);
				float p1Ratio = l1 / (l1 + l3);
				if (DT.Go) DT.Get("\n" + p1Ratio.ToString("F5") + " - P1 Ratio").List_R().Color(TextColor);
				float p2Ratio = l2 / (l2 + l4);
				if (DT.Go) DT.Get(p2Ratio.ToString("F5") + " - P2 Ratio").List_R().Color(TextColor);

				float biggest = Mathf.Max(l1, Mathf.Max(l2, Mathf.Max(l3, l4)));
				float smallest = Mathf.Min(l1, Mathf.Min(l2, Mathf.Min(l3, l4)));
				float bigSmallA = smallest / biggest;
				if (DT.Go) DT.Get("\n" + bigSmallA.ToString("F5") + " - Big Small Ratio A").List_R().Color(TextColor);
				float bigSmallB = smallest / (biggest + smallest);
				if (DT.Go) DT.Get(bigSmallB.ToString("F5") + " - Big Small Ratio A").List_R().Color(TextColor);


				/*float factor = radius2 / radius1;

				Vector2 factorPoint = dir1Perp2Contact + (dir2Perp1Contact - dir1Perp2Contact) * factor;
				DrawPoint(factorPoint);

				radius1 = (dir2Perp1Contact - factorPoint).magnitude;
				radius2 = (dir1Perp2Contact - factorPoint).magnitude;

				Vector2 circleCenter1 = p1 - perp1ToP1.normalized * radius1;
				Vector2 circleCenter2 = p2 - perp2ToP2.normalized * radius2;

				/*DRAW.Circle(circleCenter1, radius1, 100).SetColor(Color.black.A(.2f));
				DRAW.Circle(circleCenter2, radius2, 100).SetColor(Color.black.A(.2f));#1#


				DRAW.Vector(p1, factorPoint - p1).SetColor(Color.black.A(.2f));

				Vector2 midPoint = p1 + (factorPoint - p1) * .5f;
				Line line = new Line(midPoint, midPoint + (factorPoint - p1).Rot90(false));
				Vector2 hit;
				if (line.Contact(perp1, out hit, true))
				{
					DrawPoint(hit);
					DRAW.Circle(hit, (factorPoint - hit).magnitude, 100).SetColor(Color.black.A(.2f));
				}



				DRAW.Vector(p1, p2 - p1).SetColor(Color.white.A(.2f));*/
			}

			float pathLength = 0;
			if (false)
			{
				Vector2 perpContact;
				if (perp1.Contact(perp2, out perpContact, true))
				{
					if (drawHelpers)
						DRAW.Circle(perpContact, .075f, 20).SetColor(Color.black).Fill(.3f, true);

					Vector2 toP1 = p1 - perpContact;
					Vector2 toP2 = p2 - perpContact;

					float oAngle = Vector2.Angle(toP1, toP2);
					float bend = (360 - oAngle) / 360;

					if (toP1.sqrMagnitude < toP2.sqrMagnitude)
					{
						float arcRadius = toP1.magnitude;

						if (drawHelpers && false)
							DRAW.Circle(perpContact, arcRadius, 100).SetColor(Color.black.A(.2f));

						float arcLength = arcRadius * 2 * Mth.π * ((360 - oAngle) / 360);
						Arc arc = new Arc(new Line(p1, p1 + line1.dir * arcLength), bend);
						arc.Draw(100).SetColor(Color.black);
					}
					else
					{
						float arcRadius = toP2.magnitude;

						if (drawHelpers && false)
							DRAW.Circle(perpContact, arcRadius, 100).SetColor(Color.black.A(.2f));

						float arcLength = arcRadius * 2 * Mth.π * bend;
						Arc arc = new Arc(new Line(p2, p2 + line2.dir * arcLength), bend, 0);
						//arc.Draw(100).SetColor(Color.black);

						float otherLength = toP1.magnitude;
						float otherRadius = (otherLength - arcRadius) * .5f;
						float otherArcLength = otherRadius * Mth.π;

						Vector2 arc2Point = perpContact + toP1.normalized * arcRadius;

						if (drawHelpers)
							DRAW.Circle(arc2Point, .075f, 20).SetColor(Color.black).Fill(.3f, true);

						Arc otherArc = new Arc(new Line(arc2Point, arc2Point - line1.dir * otherArcLength), -.5f, 0);
						//otherArc.Draw(100).SetColor(Color.black);


						pathLength = arcLength + otherArcLength;

						if (drawPebbles)
						{
							float pathStepper = 0;
							HLS pebble = Color.red.ToHLS();
							while (pathStepper < pathLength)
							{
								Vector2 drawPoint = pathStepper < arcLength
									? arc.LerpPos(pathStepper / arcLength)
									: otherArc.LerpPos((pathStepper - arcLength) / otherArcLength);

								DRAW.Circle(drawPoint, .075f, 20).SetColor(pebble.ShiftHue(pathStepper * .1f))
									.Fill(1, true);

								pathStepper += .3f;
							}
						}
					}
				}
			}


			Color endColor = Color.red.ToHLS().ShiftHue(pathLength * .1f);
			DRAW.Circle(p1, .075f, 20).SetColor(endColor).Fill(1);
			DRAW.Arrow(p1, line1.dir, .15f).SetColor(endColor).Fill(1);
			if (DT.Go) DT.Get("P1").WorldPos(p1).Color(endColor).Shift(10, 0).Dir(Dir.Right);

			DRAW.Circle(p2, .075f, 20).SetColor(Color.red).Fill(1);
			DRAW.Arrow(p2, line2.dir, .15f).SetColor(Color.red).Fill(1);
			if (DT.Go) DT.Get("P2").WorldPos(p2).Color(Color.red).Shift(10, 0).Dir(Dir.Right);

			if (DT.Go) DT.Get("Dir Dot: " + Vector2.Dot(line1.dir, line2.dir).ToString("F2")).Color(TextColor).List_L();


			/*for (int i = 0; i < 10; i++)
				DrawText.Get("This is Line " + i).Color(Color.red.ToHLS().ShiftHue(i * .1f)).LeftList();

			for (int i = 0; i < 10; i++)
				DrawText.Get("This is Line " + i).Color(Color.red.ToHLS().ShiftHue(i * .1f)).RightList();*/
		}


		private static readonly Color TextColor = new HLS(0, .35f, 0, 1);
		private static readonly Color LineBlack = Color.black.A(.2f);
		private static readonly Color LineWhite = Color.white.A(.2f);

		private static Color LineHue
		{
			get { return new HLS(1, .5f, .2f).ShiftHue(animTime * .05f); }
		}


		private static void Dot(Vector2 point)
		{
			DRAW.Circle(point, .055f, 20).SetColor(Color.black).Fill(.3f, true);
		}


		private static void WhiteLine(Vector2 p1, Vector2 p2, float multi = 1)
		{
			Vector2 dir = p2 - p1;
			Vector2 center = p1 + dir * .5f;
			p1 = center - dir * .5f * multi;
			p2 = center + dir * .5f * multi;
			DRAW.Vector(p1, (p2 - p1)).SetColor(LineWhite);
		}


		private static void BlackLine(Vector2 p1, Vector2 p2, float multi = 1)
		{
			Vector2 dir = p2 - p1;
			Vector2 center = p1 + dir * .5f;
			p1 = center - dir * .5f * multi;
			p2 = center + dir * .5f * multi;
			DRAW.Vector(p1, (p2 - p1)).SetColor(LineBlack);
		}


		private static float ApproxMulti(Vector2 p1, Vector2 dir1, Vector2 p2, Vector2 dir2)
		{
			float m = 3;
			float step = m * .5f;
			float r1 = dir1.magnitude;
			float r2 = dir2.magnitude;

			for (int i = 0; i < 20; i++)
			{
				m += new Circle(p1 + dir1 * m, r1 * m).Contact(new Circle(p2 + dir2 * m, r2 * m)) ? -step : step;
				step *= .5f;
			}

			return m;
		}


		private class DT
		{
			private Vector2 pos, offset;
			private string text;
			private Color color;
			private Dir dir;
			private bool leftList, rightList, world;


			private DT Setup(string text)
			{
				this.text = text;
				color = UnityEngine.Color.white;
				dir = _defaultDir;
				offset = Vector2.zero;
				leftList = false;
				rightList = false;
				world = false;
				return this;
			}


			public DT Color(Color color)
			{
				this.color = color;
				return this;
			}


			public DT WorldPos(Vector2 pos)
			{
				world = true;
				rightList = false;
				leftList = false;

				this.pos = pos;
				return this;
			}


			public DT ScreenPos(Vector2 pos)
			{
				this.pos = new Vector2(pos.x, Screen.height - pos.y);
				return this;
			}


			public DT Dir(Dir dir)
			{
				this.dir = dir;
				return this;
			}


			public DT Shift(int x, int y)
			{
				offset = new Vector2(x, y);
				return this;
			}


			public DT List_L()
			{
				leftList = true;
				rightList = false;
				world = false;
				return this;
			}


			public DT List_R()
			{
				rightList = true;
				leftList = false;
				world = false;
				return this;
			}


			public void Draw()
			{
				GUI.color = color;
				Vector2 size = style.CalcSize(new GUIContent(text));

				Vector2 anchorDir;
				if (leftList)
				{
					pos = new Vector2(0, leftY);
					leftY += size.y;
					anchorDir = GetAnchorDir(Test.Dir.BottomRight);
				}
				else if (rightList)
				{
					pos = new Vector2(Screen.width, rightY);
					rightY += size.y;
					anchorDir = GetAnchorDir(Test.Dir.BottomLeft);
				}
				else
					anchorDir = GetAnchorDir(dir);

				Vector2 drawPos;
				if (world)
				{
					Vector2 screenPos = cam.WorldToScreenPoint(pos).V2();
					drawPos = new Vector2(screenPos.x, Screen.height - screenPos.y);
				}
				else
					drawPos = pos;


				Vector2 anchorShift = new Vector2(
					Mathf.Approximately(anchorDir.x, 0) ? 0 : Mathf.Sign(anchorDir.x) * size.x * .5f,
					Mathf.Approximately(anchorDir.y, 0) ? 0 : Mathf.Sign(anchorDir.y) * size.y * .5f);

				GUI.Label(new Rect(drawPos - size * .5f + anchorShift + offset, size), text, style);

				GUI.color = UnityEngine.Color.white;
			}


			private static Vector2 GetAnchorDir(Dir dir)
			{
				switch (dir)
				{
					default: return Vector2.zero;
					case Test.Dir.TopLeft: return new Vector2(-1, -1);
					case Test.Dir.Top: return new Vector2(0, -1);
					case Test.Dir.TopRight: return new Vector2(1, -1);

					case Test.Dir.Left: return new Vector2(-1, 0);
					case Test.Dir.Right: return new Vector2(1, 0);

					case Test.Dir.BottomLeft: return new Vector2(-1, 1);
					case Test.Dir.Bottom: return new Vector2(0, 1);
					case Test.Dir.BottomRight: return new Vector2(1, 1);
				}
			}


			//  Static //
			private static Camera cam;
			public static int Count;
			public static readonly DT[] texts = CollectionInit.Array<DT>(100);
			public static GUIStyle style;

			private static Dir _defaultDir = Test.Dir.TopRight;

			public static float leftY, rightY;

			public const bool Go = true;


			public static DT Get(string text)
			{
				return texts[Count++].Setup(text);
			}


			public static void Init()
			{
				cam = Camera.main;
			}
		}


		private void OnGUI()
		{
			if (DT.style == null)
				DT.style = new GUIStyle(GUI.skin.label)
				{
					fontStyle = FontStyle.Bold,
					fontSize = 10,
					padding = new RectOffset(3, 3, 1, 1)
				};

			DT.leftY = 0;
			DT.rightY = 0;


			for (int i = 0; i < DT.Count; i++)
				DT.texts[i].Draw();
		}
	}


	public enum Dir
	{
		TopLeft,
		Top,
		TopRight,
		Left,
		Center,
		Right,
		BottomLeft,
		Bottom,
		BottomRight
	}
}
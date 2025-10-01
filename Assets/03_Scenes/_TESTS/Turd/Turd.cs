using GeoMath;
using UnityEngine;

namespace Test
{
	public class Turd : MonoBehaviour
	{
		public Line line;
		public float thickness;

		public Circle[] circles;

		[Space(10)] [Range(0, 1)] public float animSpeed;

		private Color draw;

		private Line fallLine, turnLine;
		private bool turnOnA;
		private FlyPath flyPath;
		private float flightTime;
		private float maxBend;

		private int collisionCircle;
		private float turnSpeed, turnTime, turnAngle;

		[Space(10)] public Line testLine;

		private float measure;
		private int measureCount;


		private enum TestState
		{
			None,
			Falling,
			LineToTangent,
			ArcToSurface
		}

		private TestState state;
		private bool turning;
		private Line flatLine;
		private float flatLineLerp, flatBend, flatBendTime, flatBendSpeed;

		private bool showHelpers = true;

		private Line hitReaction;
		private float hitReactionSpin, hitReactionTime;


		private void OnEnable()
		{
			draw = new Color(.1f, .1f, .1f, 1);

			maxBend = CalculateMaxBend(line.Length, thickness);
		}


		private void Update()
		{
			line = line.Rotate(Time.deltaTime * 80);

			//ArcTipTest();
			NewTest();

			if (Input.GetKeyDown(KeyCode.H))
				showHelpers = !showHelpers;

			if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
			{
				state = state == TestState.None ? TestState.Falling : TestState.None;

				if (state == TestState.Falling)
				{
					fallLine = line;
					flyPath = new FlyPath(fallLine.l1 + fallLine.dir * .5f,
						Rot.Z(Random.Range(0, 360)) * Vector2.up * 10);
					flightTime = 0;
				}
			}


			switch (state)
			{
				case TestState.Falling:
				{
					flightTime += Time.deltaTime * animSpeed;
					fallLine = fallLine.SetPos(flyPath.GetPos(flightTime));

					if (FoundCollision())
					{
						PreciseCollision();

						//  Check if Line has to Turn on Endpoints  //
						Vector2 center = circles[collisionCircle].center;
						Vector2 linePoint = fallLine.ClosestPoint(center);
						Vector2 impactMV = flyPath.GetMV(flightTime);

						ImpactPhysics(fallLine, impactMV,
							center + (linePoint - center).SetLength(circles[collisionCircle].radius),
							(linePoint - center).normalized);


						if (linePoint == fallLine.l1 || linePoint == fallLine.GetL2())
						{
							state = TestState.LineToTangent;

							turnOnA = linePoint == fallLine.l1;
							turnLine = fallLine;

							Vector2 dir = linePoint - center;
							Vector2 tangent = dir.Rot90();
							Vector2 turnDir = (tangent * Vector2.Dot(fallLine.dir, tangent)).normalized;

							turnAngle = -fallLine.dir.normalized.Angle_Sign(turnDir);


							float totalTurnCircumference = fallLine.Length * 2 * Mth.π;
							float totalTurnTime = totalTurnCircumference / impactMV.magnitude;

							turnSpeed = 1f / (Mathf.Abs(turnAngle) / 360 * totalTurnTime);
							turnTime = 0;
							turning = true;
						}
						else
						{
							turning = false;
							turnAngle = 0;
							state = TestState.ArcToSurface;
						}

						//  ArcStuff  //
						flatLineLerp = Mathf.Clamp01(fallLine.GetClosestLerp(center));
						flatLine = fallLine.Rotate(turnAngle, turnOnA ? 0 : 1);

						float flatRadius = circles[collisionCircle].radius + thickness;
						float circleDirSign = Mathf.Sign(Vector2.Dot(flatLine.dir.Rot90().normalized,
							(center - flatLine.l1).normalized));
						flatBend = flatLine.GetBend(flatRadius * circleDirSign);
						flatBend = Mathf.Min(Mathf.Abs(flatBend), maxBend) * Mathf.Sign(flatBend);

						flatBendTime = 0;

						float arcTravel = Mathf.Abs(flatBend) * flatLine.Length * Arc.LineTipPathRatio;
						flatBendSpeed = impactMV.magnitude / arcTravel;
					}
				}
					break;


				case TestState.LineToTangent:
				{
					turnTime += Time.deltaTime * turnSpeed * animSpeed;
					turnLine = fallLine.Rotate(Mathf.Lerp(0, turnAngle, turnTime), turnOnA ? 0 : 1);

					if (turnTime >= 1)
						state = TestState.ArcToSurface;
				}
					break;


				case TestState.ArcToSurface:
				{
					flatBendTime += Time.deltaTime * flatBendSpeed * animSpeed;
				}
					break;
			}




			//  Draw MotionVectors  //
			if (state != TestState.None && showHelpers)
			{
				Vector2 mV = flyPath.GetMV(flightTime) * .1f;
				DRAW.Arrow(fallLine.l1, -mV, .1f, true).SetColor(Color.white).Fill(1);
				DRAW.Arrow(fallLine.GetL2(), -mV, .1f, true).SetColor(Color.white).Fill(1);

				flyPath.Draw(flightTime, flightTime + .4f, 20, true).SetColor(draw.A(.3f)).Fill(.3f);
			}




			if ((state == TestState.LineToTangent || state == TestState.ArcToSurface) && showHelpers)
			{
				//  Draw Impact Point and Normal  //
				Vector2 center = circles[collisionCircle].center;
				Vector2 linePoint = fallLine.ClosestPoint(center);
				Vector2 dir = linePoint - center;
				Vector2 hitPoint = center + dir.SetLength(circles[collisionCircle].radius);
				DRAW.Circle(hitPoint, .05f, 30).SetColor(Color.red).Fill(1);
				DRAW.Vector(hitPoint, dir.SetLength(thickness)).SetColor(Color.red);

				//  Draw turnline if closest point is an endpoint of line  //
				if (turning)
				{
					DRAW.Circle(linePoint, .1f, 30).SetColor(Color.yellow);

					Line previewLine = fallLine.Rotate(turnAngle, turnOnA ? 0 : 1);
					DRAW.GapVector(previewLine.l1, previewLine.dir, 10);

					fallLine.Rotate(Mth.SmoothPP(0, turnAngle, Time.realtimeSinceStartup), turnOnA ? 0 : 1).Draw()
						.SetColor(Color.cyan);
				}


				//  Wiggle Arc  //
				Arc wiggleArc = new Arc(flatLine, flatBend, flatLineLerp).SetBend(
					Mth.SmoothPP(-flatBend, flatBend, Time.realtimeSinceStartup * .5f), flatLineLerp);
				wiggleArc.DrawShell(thickness, true).SetColor(Color.white.A(.5f));
				wiggleArc.DrawTips().SetColor(Color.white).Fill(.5f, true);


				//  Draw ImpactSpin  //
				hitReactionTime += Time.deltaTime;
				hitReaction.Rotate(GPhysics.Get_SpinAngle_Deg(hitReactionSpin, hitReactionTime))
					.DrawShell(thickness, true).SetColor(HLS.Get(hitReactionTime * .25f));
			}




			Line drawLine;
			switch (state)
			{
				default: drawLine = line; break;
				case TestState.Falling: drawLine = fallLine; break;
				case TestState.LineToTangent: drawLine = turnLine; break;
			}

			if (state != TestState.ArcToSurface)
			{
				drawLine.DrawShell(thickness, true).SetColor(draw);
				drawLine.DrawTips().SetColor(Color.yellow).Fill(1);
			}
			else
			{
				Arc flatArc = new Arc(flatLine, Mathf.Lerp(0, flatBend, flatBendTime), flatLineLerp);
				flatArc.DrawShell(thickness, true).SetColor(draw);
				flatArc.DrawTips().SetColor(Color.yellow).Fill(1);
			}

			for (int i = 0; i < circles.Length; i++)
				DRAW.Circle(circles[i].center, circles[i].radius, 100).SetColor(draw);
		}


		private static float CalculateMaxBend(float length, float thickness)
		{
			float bend = .5f;
			float step = .5f;

			for (int i = 0; i < 10; i++)
			{
				float radius = 1 / bend / 2 / Mth.π * length;
				float circumference = 2 * radius * Mth.π;
				float arcLengthFraction = Mathf.Asin(thickness / radius) / Mth.π;
				float nonArcLength = (1 - arcLengthFraction) * circumference;

				if (i == 9)
					break;

				step *= .5f;
				bend += nonArcLength < length ? -step : step;
			}

			return bend;
		}


		private static float CalculateMaxBend2(float length, float chord)
		{


			return 0;
		}


		private void PreciseCollision()
		{
			float stepTime = .1f;
			flightTime -= stepTime;

			for (int i = 0; i < 10; i++)
			{
				fallLine = fallLine.SetPos(flyPath.GetPos(flightTime));

				if (i == 9)
					return;

				stepTime *= .5f;
				flightTime += FoundCollision() ? -stepTime : stepTime;
			}
		}


		private bool FoundCollision()
		{
			for (int i = 0; i < circles.Length; i++)
			{
				float itemRadiusSqr = Mth.IntPow(thickness + circles[i].radius, 2);
				if (fallLine.SqrDistance(circles[i].center) <= itemRadiusSqr)
				{
					collisionCircle = i;
					return true;
				}
			}

			return false;
		}


		private void NewTest()
		{
			testLine.Draw().SetColor(draw);

			float lineLerp = 0; //Mathf.PingPong(Time.realtimeSinceStartup * .1f, 1);

			float animBend = Mth.SmoothPP(Time.realtimeSinceStartup * .25f * animSpeed);
			Arc testArc = new Arc(testLine, animBend, lineLerp);
			testArc.Draw(100).SetColor(draw);


			Color lineColor = Color.Lerp(Color.white, Color.magenta, .5f);
			float rotLineAngle = (180 - 180 * lineLerp) * animBend;

			Line rotLine = testLine.Rotate(rotLineAngle, lineLerp);
			rotLine.Draw().SetColor(lineColor);


			Color green = lineColor.ToHLS().ShiftHue(.5f);
			testArc.Rotate(-rotLineAngle, 0).Draw(100).SetColor(green);
		}


		private void ArcTipTest()
		{
			Line turnTestLine = testLine.Rotate(Time.realtimeSinceStartup * 45f * animSpeed, 0);

			DRAW.Vector(turnTestLine.l1, turnTestLine.dir).SetColor(draw);
			float animBend = Mth.SmoothPP(Time.realtimeSinceStartup * .5f * animSpeed);

			Arc testArc = new Arc(turnTestLine, animBend, 0);
			testArc.Draw(100);

			DRAW.Circle(testArc.LerpPos(1), .1f, 30).Fill();

			{
				float length = 0;
				Vector2 p = turnTestLine.GetL2();
				const float step = 1f / 9999;
				for (int i = 1; i < 10000; i++)
				{
					Vector2 newP = new Arc(turnTestLine, i * step, .5f).LerpPos(1);
					length += Vector2.Distance(p, newP);
					p = newP;
				}

				measure += length;
				measureCount++;

				Debug.Log(length + " ... " + turnTestLine.Length + " ... " +
				          measure / measureCount / turnTestLine.Length);
			}
			{
				DRAW.Shape shape = DRAW.Shape.Get(100);
				const float step = 1f / 99;
				for (int i = 0; i < 100; i++)
					shape.Set(i, new Arc(turnTestLine, i * step, 0).LerpPos(1));

				shape.SetColor(Color.magenta);
			}
			{
				DRAW.Shape shape = DRAW.Shape.Get(100);
				const float step = 1f / 99;
				for (int i = 0; i < 100; i++)
					shape.Set(i, new Arc(turnTestLine, i * -step, 0).LerpPos(0));

				shape.SetColor(Color.magenta);
			}
		}


		private void ImpactPhysics(Line hitLine, Vector2 mV, Vector2 hitPoint, Vector2 normal)
		{
			hitReaction = hitLine;

			Vector2 center = hitReaction.l1 + hitReaction.dir * .5f;

			float velAlongNormal = Vector2.Dot(mV, normal);

			float hitDirCross = (hitPoint - center).Cross(normal);

			const float e = .55f;
			const float mass = 10;
			const float colliderMass = 100000;
			const float inertia = 8;

			float impulse = -(1 + e) * velAlongNormal /
			                (
				                1 / mass + 1 / colliderMass
				                         + Mth.IntPow(hitDirCross, 2) / inertia
				                /* + Mathf.Pow(Extensions.CrossProduct(stickRadiusVector, Tri.HitNormalInverse), 2) / inertia */
				                //Stick inertia is infinite
			                );

			hitReactionSpin = hitDirCross / inertia * impulse * Mathf.Rad2Deg / 35f;
			hitReactionTime = 0;
		}
	}
}
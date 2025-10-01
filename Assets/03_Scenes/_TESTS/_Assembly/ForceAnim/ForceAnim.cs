using UnityEngine;

namespace Test
{
	public class ForceAnim : MonoBehaviour
	{
		public float startValue, endValue, lastValue;

		[Space] public float force;

		[Space] public float damp;
		public float multi, multiB;

		[Range(0, 1)] public float stepTwo, testEnd;

		[Space] [Range(0, 1)] public float accelMid;

		public float testLerp;

		[Range(-1, 1)] public float speed;

		private readonly PendulumMotion pendulumA = new PendulumMotion();
		//pendulumB = new PendulumMotion();

		private readonly AccelMotion accel = new AccelMotion();


		private void Update()
		{
			const float graphSize = 40;

			Vector3 right = Vector3.right * graphSize;

			//  Graph Stuff  //
			{
				DRAW.Arrow(Vector3.zero, right, .25f);
				DRAW.Arrow(Vector3.zero, Vector3.up * graphSize * .25f, .25f);
				DRAW.Arrow(Vector3.zero, Vector3.up * graphSize * -.25f, .25f);

				int min = Mathf.CeilToInt(-graphSize * .25f) + 1;
				int max = Mathf.FloorToInt(graphSize * .25f);
				for (int i = min; i < max; i++)
					DRAW.Vector(Vector3.up * i, Vector3.left * .5f);
			}


			//  Setup Pendulums  //;
			pendulumA.Setup(startValue, force, endValue, multi, damp);

			float startB = pendulumA.GetValue(stepTwo * graphSize);
			float oneForceB = pendulumA.GetForce(stepTwo * graphSize);

			accel.Setup(startB, oneForceB, lastValue, (1f - stepTwo) * graphSize);

			//  Draw em  //
			pendulumA.DrawRange(0, stepTwo * graphSize, Color.green);
			accel.DrawRange(0, (1f - stepTwo) * graphSize, COLOR.purple.magenta, stepTwo * graphSize);

			{
				float lerp = testEnd > stepTwo ? testEnd - stepTwo : testEnd;
				CoolMotion motion = testEnd > stepTwo ? accel as CoolMotion : pendulumA;

				float sin = motion.GetValue(lerp * graphSize);

				Vector2 checkPos = new Vector2(testEnd * graphSize, sin);
				Vector2 checkSlope = motion.GetMVOne(lerp * graphSize).normalized;

				DRAW.Vector(checkPos, checkSlope * 50).SetColor(COLOR.blue.cornflower.A(.25f)).Fill(1);

				Vector2 turned = checkSlope.Rot90();

				DRAW.Vector(checkPos + turned * -.5f, turned).SetColor(COLOR.blue.cornflower.A(.25f)).Fill(1);
			}

			{
				testLerp = Mth.Repeat(0, 1, testLerp + Time.deltaTime * .15f * speed);

				DRAW.Vector(new Vector3(testLerp * graphSize, -50f * graphSize), Vector3.up * 100 * graphSize)
					.SetColor(COLOR.yellow.fresh.A(.25f));

				float animX = graphSize * testLerp;

				Vector2 checkPos, checkSlope;

				if (testLerp < testEnd)
				{
					float lerp = testLerp > stepTwo ? testLerp - stepTwo : testLerp;
					CoolMotion motion = testLerp > stepTwo ? accel as CoolMotion : pendulumA;

					float sin = motion.GetValue(lerp * graphSize);

					checkPos = new Vector2(animX, sin);
					checkSlope = motion.GetMVOne(lerp * graphSize);
				}
				else
				{
					float lerp = testEnd > stepTwo ? testEnd - stepTwo : testEnd;
					CoolMotion motion = testEnd > stepTwo ? accel as CoolMotion : pendulumA;

					float sin = motion.GetValue(lerp * graphSize);

					checkPos = new Vector2(testEnd * graphSize, sin);
					checkSlope = motion.GetMVOne(lerp * graphSize);

					float endLerp = testLerp - testEnd;
					checkPos += new Vector2(endLerp * graphSize, checkSlope.y * endLerp * graphSize);
				}

				DRAW.Circle(checkPos, .2f, 40).SetColor(COLOR.yellow.fresh).Fill(1, true);
				DRAW.Arrow(checkPos, checkSlope, .25f).SetColor(COLOR.yellow.fresh).Fill(1);

				DRAW.MultiCircle(new Vector3(graphSize + 2, checkPos.y), 1.5f, 3, .4f, 7).SetColor(COLOR.yellow.fresh);
			}
		}


		[System.Serializable]
		public class PendulumMotion : CoolMotion
		{
			private float sinScale, sinX, xOffset, slopeMulti, end;

			public void Setup(float startValue, float startForce, float endValue, float multi, float damp)
			{
				this.endValue = endValue;

				xOffset = startForce < 0 ? Mathf.PI : 0;

				float dir = startValue - endValue;

				sinScale = Mathf.Abs(dir) + Mathf.Abs(startForce) * multi;
				sinX = Mathf.Asin(dir * (1f / sinScale) * -1) * -Mathf.Sign(startForce);
				end = damp > 0 ? 1f * sinScale / damp : float.MaxValue;

				float cosSlope = Mathf.Cos(sinX) * Mathf.Sign(startForce);

				slopeMulti = cosSlope / startForce;

				return;
				float frequency = 1f / sinScale / slopeMulti;
				Debug.Log(frequency);
			}


			private float GetAmplitude(float x)
			{
				return Mathf.SmoothStep(0, 1, 1 - Mathf.InverseLerp(0, end, x));
			}


			public override float GetValue(float x)
			{
				return Mathf.Sin(sinX + xOffset + x / sinScale / slopeMulti) * sinScale * GetAmplitude(x)
				       + endValue;
			}


			public override float GetForce(float x)
			{
				const float mul = 1000f, frac = 1f / mul;

				return (GetValue(x + frac) - GetValue(x)) * mul;
			}


			public override void DrawRange(float min, float max, Color color, float offset = 0)
			{
				const int pointCount = 1000;

				DRAW.Shape shape = DRAW.Shape.Get(pointCount);

				for (int i = 0; i < pointCount; i++)
				{
					float lerp = i * (1f / (pointCount - 1));
					float x = Mathf.Lerp(min, max, lerp);
					shape.Set(i, new Vector3(x + offset, GetValue(x))).SetColor(color);
				}

				float gapLineLength = max - min;
				int gaps = Mathf.FloorToInt(gapLineLength);
				DRAW.GapVector(new Vector3(min + offset, endValue), Vector3.right * gapLineLength, gaps)
					.SetColor(COLOR.orange.coral);
			}
		}

//https://www.calculatorsoup.com/calculators/physics/displacement_v_a_t.php
		[System.Serializable]
		public class AccelMotion : CoolMotion
		{
			public float startValue, startForce, accel;

			public void Setup(float startValue, float startForce, float endValue, float duration)
			{
				this.startValue = startValue;
				this.startForce = startForce;
				this.endValue = endValue;

				float displacement = endValue - startValue;

				accel = (2 * displacement) / (duration * duration) - (2 * startForce) / duration;
			}


			public override float GetValue(float x)
			{
				return startValue + startForce * x + .5f * accel * (x * x);
			}


			public override float GetForce(float x)
			{
				return startForce + accel * x;
			}
		}

		[System.Serializable]
		public class AccelStopMotion : CoolMotion
		{
			public float startValueA,
				startForceA,
				accelA,
				startValueB,
				startForceB,
				accelB;

			private float mid;

			public void Setup(float startValue, float startForce, float midFraction, float endValue, float duration)
			{
				startValueA = startValue;
				startForceA = startForce;
				this.endValue = endValue;

				mid = duration * midFraction;

				startValueB = startValue + (endValue - startValue) * .5f;

				{
					float displacement = startValueB - startValue;
					float time = mid;

					accelA = (2 * displacement) / (time * time) - (2 * startForceA) / time;
				}

				startForceB = GetForce(mid);

				{
					float displacement = endValue - startValueB;
					float time = duration - mid;

					accelB = Mathf.Pow((2 * Mathf.Abs(displacement)) / (time * time), 2) * -Mathf.Sign(startForceB);
				}
			}


			public override float GetValue(float x)
			{
				if (x <= mid)
					return startValueA + startForceA * x + .5f * accelA * (x * x);

				x -= mid;

				return startValueB + startForceB * x + .5f * accelB * (x * x);
			}


			public override float GetForce(float x)
			{
				if (x <= mid)
					return startForceA + accelA * x;

				x -= mid;

				return startForceB + accelB * x;
			}
		}


		public abstract class CoolMotion
		{
			protected float endValue;
			public abstract float GetValue(float x);

			public abstract float GetForce(float x);


			public Vector2 GetMVOne(float x)
			{
				return new Vector2(1, GetForce(x));
			}

			public virtual void DrawRange(float min, float max, Color color, float offset = 0)
			{
				const int pointCount = 1000;

				DRAW.Shape shape = DRAW.Shape.Get(pointCount);

				for (int i = 0; i < pointCount; i++)
				{
					float lerp = i * (1f / (pointCount - 1));
					float x = Mathf.Lerp(min, max, lerp);
					shape.Set(i, new Vector3(x + offset, GetValue(x))).SetColor(color);
				}

				float gapLineLength = max - min;
				int gaps = Mathf.FloorToInt(gapLineLength);
				DRAW.GapVector(new Vector3(min + offset, endValue), Vector3.right * gapLineLength, gaps)
					.SetColor(COLOR.orange.coral);
			}
		}
	}
}
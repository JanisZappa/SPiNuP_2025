using UnityEngine;

namespace Test
{
	public class EyeTest : MonoBehaviour
	{
		public Eye[] eyes;

		[Space(10)] public Vector2 eyeSize;
		public Vector2 pupilSize;

		[Space(10)] public float targetDistance;

		private float shock, focus;

		private static float blinkTime;


		private void Awake()
		{
			for (int i = 0; i < 2; i++)
				eyes[i].Setup(transform);
		}


		private void Update()
		{
			float headAngle = Mathf.Lerp(-180, 180, Mathf.PerlinNoise(Time.realtimeSinceStartup * .05f, .24f));
			float headAngle2 = Mathf.Lerp(-3, 3, Mathf.PerlinNoise(Time.realtimeSinceStartup * 1.65f, .24f));
			transform.rotation = Quaternion.AngleAxis(headAngle + headAngle2, Vector3.forward);


			shock = Mathf.Clamp01(shock + (Input.GetMouseButton(1) ? 10 : -10) * Time.deltaTime);
			focus = Mathf.Clamp01(focus + (Input.GetMouseButton(2) ? 10 : -10) * Time.deltaTime);

			float shakyTime = Time.realtimeSinceStartup * 12;
			Vector2 animEyeSize = eyeSize * (1 + shock * .15f) * (1 - focus * Mth.SmoothPP(-.02f, .01f, shakyTime));
			Vector2 animPupilSize = pupilSize * (1 - shock * .4f) * (1 + focus * Mth.SmoothPP(.24f, .31f, shakyTime));


			Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).SetZ(targetDistance);
			Vector3 localTargetPos = transform.InverseTransformPoint(targetPos);

			float blink = GetBlink();
			for (int i = 0; i < 2; i++)
				eyes[i].LookAt(animEyeSize, animPupilSize, localTargetPos, blink);
		}


		private static float GetBlink()
		{
			float now = Time.realtimeSinceStartup;

			float t = (now - blinkTime) * 5;

			if (t < 0)
				return 1;

			if (t > 1)
			{
				blinkTime = now + Random.Range(.6f, 3f);
				return 1;
			}

			return Mathf.Pow(1 - Mathf.PingPong(t * 2, 1), 2);

			//return 1 - Mathf.Pow(1 - Mathf.Clamp01(Mathf.PingPong(Time.realtimeSinceStartup * 7, 10)), 2);
		}


		[System.Serializable]
		public class Eye
		{
			public Transform eye, pupil;
			public Vector3 rootPos;
			[Space(10)] public bool left;

			private Transform head;

			private const float lookSqueeze = .04f,
				topBlinkRatio = .98f;


			public void Setup(Transform head)
			{
				this.head = head;
			}


			public void LookAt(Vector2 eyeSize, Vector2 pupilSize, Vector3 localTargetPos, float blink)
			{
				const float eyeDepth = 3;

				Vector3 eyeCenter = rootPos + new Vector3(0, 0, eyeDepth);
				Vector3 dir = (localTargetPos - eyeCenter).normalized * eyeDepth;
				Vector3 pupilPos = rootPos + dir.SetZ(0);


				float pupilXLerp = Mathf.InverseLerp(-eyeSize.x * .5f + eyeCenter.x, eyeSize.x * .5f + eyeCenter.x,
					pupilPos.x);
				float pupilYLerp = Mathf.InverseLerp(-eyeSize.y * .5f + eyeCenter.y, eyeSize.y * .5f + eyeCenter.y,
					pupilPos.y);

				float eyeSqueeze = blink * (.85f + pupilYLerp * .25f);

				float ySize = eyeSize.y *
				              (1 + Mathf.Lerp(-lookSqueeze, lookSqueeze, !left ? 1 - pupilXLerp : pupilXLerp));
				float yPos = ySize * -.5f * (1 - eyeSqueeze) * topBlinkRatio +
				             ySize * .5f * (1 - eyeSqueeze) * (1 - topBlinkRatio);
				float yScale = ySize * eyeSqueeze;

				Vector2 eyeP = eyeCenter + new Vector3(0, yPos);
				Vector2 eyeS = new Vector2(eyeSize.x, yScale);

				eye.localPosition = rootPos + new Vector3(0, yPos);
				eye.localScale = new Vector3(eyeS.x, eyeS.y, .2f);


				//  Pupil  //
				pupilPos = new Vector2(Mathf.Clamp(pupilPos.x - eyeP.x, eyeS.x * -.5f, eyeS.x * .5f),
					Mathf.Clamp(pupilPos.y - eyeP.y, eyeS.y * -.5f, eyeS.y * .5f));

				float xMinSquish = Mathf.Max(0, -(pupilPos.x - pupilSize.x * .5f + eyeS.x * .5f));
				float xMaxSquish = Mathf.Max(0, pupilPos.x + pupilSize.x * .5f - eyeS.x * .5f);
				float yMinSquish = Mathf.Max(0, -(pupilPos.y - pupilSize.y * .5f + eyeS.y * .5f));
				float yMaxSquish = Mathf.Max(0, pupilPos.y + pupilSize.y * .5f - eyeS.y * .5f);

				pupil.localPosition = eyeP.V3() + new Vector3(pupilPos.x - xMaxSquish * .5f + xMinSquish * .5f,
					pupilPos.y - yMaxSquish * .5f + yMinSquish * .5f, -.2f);

				pupil.localScale = new Vector3(pupilSize.x - xMaxSquish - xMinSquish,
					pupilSize.y - yMaxSquish - yMinSquish, .2f);
			}


			private void DebugLines(Vector3 eyeCenter, Vector3 dir, Vector3 pupilPos)
			{
				DRAW.Vector(head.TransformPoint(eyeCenter),
					head.TransformDirection(dir)).SetColor(Color.red);

				DRAW.Circle(pupilPos, .2f, 20).SetColor(Color.magenta).Fill(1);
			}
		}
	}
}
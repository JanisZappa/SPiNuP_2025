using UnityEngine;
using UnityEngine.UI;

namespace Test
{
	public class BenchmarkKing : MonoBehaviour
	{
		public Text uiText;

		private void Update()
		{
			if (false)
			{
				Vector2 v = new Vector2(46f, 8644f);

				Vector2 result;
				const float step = .25456f;
				const int steps = 1000000;

				float t1 = Time.realtimeSinceStartup;

				for (int i = 0; i < steps; i++)
					result = Quaternion.AngleAxis(step * i, Vector3.forward) * v;

				t1 = Time.realtimeSinceStartup - t1;

				float t2 = Time.realtimeSinceStartup;

				for (int i = 0; i < steps; i++)
					result = v.Rot(step * i);

				t2 = Time.realtimeSinceStartup - t2;

				uiText.text = t1.ToString() + "\n" + t2.ToString();
			}

			if (false)
			{
				Quaternion result;
				const float step = .25456f;
				const int steps = 1000000;

				float t1 = Time.realtimeSinceStartup;

				for (int i = 0; i < steps; i++)
					result = Quaternion.AngleAxis(step * i, Vector3.forward);

				t1 = Time.realtimeSinceStartup - t1;

				float t2 = Time.realtimeSinceStartup;

				for (int i = 0; i < steps; i++)
					result = Rot.Z(step * i);

				t2 = Time.realtimeSinceStartup - t2;

				uiText.text = t1.ToString() + "\n" + t2.ToString();
			}

			if (false)
			{
				float result;
				const float step = .25456f;
				const int steps = 1000000;

				float t1 = Time.realtimeSinceStartup;

				for (int i = 0; i < steps; i++)
					result = Mathf.SmoothStep(0, 1, Mathf.PingPong(i * step, 1));

				t1 = Time.realtimeSinceStartup - t1;

				float t2 = Time.realtimeSinceStartup;

				for (int i = 0; i < steps; i++)
					result = Mth.SmoothPP(0, 1, i * step);

				t2 = Time.realtimeSinceStartup - t2;

				uiText.text = t1.ToString() + "\n" + t2.ToString();
			}

			if (false)
			{
				float result;
				const float step = .25456f;
				const int steps = 100000;

				float t1 = Time.realtimeSinceStartup;

				for (int i = 0; i < steps; i++)
					result = Mathf.Pow(step, Random.Range(2, 31));

				t1 = Time.realtimeSinceStartup - t1;

				float t2 = Time.realtimeSinceStartup;

				for (int i = 0; i < steps; i++)
					result = Mth.IntPow(step, Random.Range(2, 31));

				t2 = Time.realtimeSinceStartup - t2;

				uiText.text = t1.ToString() + "\n" + t2.ToString();
			}

			if (false)
			{
				Vector2 result;
				const float step = .25456f;
				const int steps = 100000;

				Vector2 source = new Vector2(.35f, .345f);
				Vector2 point = new Vector2(100, 45f);

				float t1 = Time.realtimeSinceStartup;

				for (int i = 0; i < steps; i++)
					result = source + (point - source).normalized * i;

				t1 = Time.realtimeSinceStartup - t1;

				float t2 = Time.realtimeSinceStartup;

				for (int i = 0; i < steps; i++)
					result = source.AimPos(point, i);

				t2 = Time.realtimeSinceStartup - t2;

				uiText.text = t1.ToString() + "\n" + t2.ToString();
			}

			if (true)
			{
				/*IEnumerable<elementType> values = Enum.GetValues(typeof(elementType)).Cast<elementType>();
				List<elementType> allTypes = new List<elementType>();
				foreach (elementType elementType in values)
					allTypes.Add(elementType);

				elementType[] types = allTypes.ToArray();
				int count = types.Length;

				float t1 = Time.realtimeSinceStartup;

				for (int i = 0; i < 100000; i++)
					(types[i % count]).Matches(info.IsItem);

				t1 = Time.realtimeSinceStartup - t1;

				float t2 = Time.realtimeSinceStartup;

				for (int i = 0; i < 100000; i++)
					Mask.IsItem.Fits(types[i % count]);

				t2 = Time.realtimeSinceStartup - t2;

				uiText.text = t1.ToString() + "\n" + t2.ToString();*/
			}
		}
	}


	public class WrapArray<T>
	{
		private readonly T[] array;
		private readonly int arrayLength;

		private int min, max;

		private int first
		{
			get { return min % arrayLength; }
		}

		private int last
		{
			get { return (max - 1) % arrayLength; }
		}

		private int length
		{
			get { return max - min; }
		}

		public WrapArray(int arrayLength)
		{
			this.arrayLength = arrayLength;
			array = new T[arrayLength];
		}
	}
}
using UnityEngine;

namespace Test
{
	public class PowTest : MonoBehaviour
	{
		public float value;
		public float power;

		[Space(10)] public float defaultResult;
		public float defaultTime;

		[Space(10)] public float myResult;
		public float myTime;

		private void Update()
		{
			/*if (Input.GetKeyDown(KeyCode.Space))
			{
				defaultTime = 0;
				for (int i = 0; i < 1000000; i++)
				{
					float t       = Time.realtimeSinceStartup;
					defaultResult = Mathf.Pow(value, power);
					defaultTime  += Time.realtimeSinceStartup - t;
				}


				myTime = 0;
				for (int i = 0; i < 1000000; i++)
				{
					float t  = Time.realtimeSinceStartup;
					myResult = GMath.Pow(value, power);
					myTime  += Time.realtimeSinceStartup - t;
				}
			}*/
		}
	}
}
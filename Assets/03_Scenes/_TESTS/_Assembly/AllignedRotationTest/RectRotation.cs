using UnityEngine;

namespace Test
{
	public class RectRotation : MonoBehaviour
	{
		public float speed;
		private float angle;

		private void Update()
		{
			angle += Time.deltaTime * speed;

			Transform t = transform;
			t.rotation = Rot.Y(angle);

			Vector3 s = t.localScale;
			t.localPosition = V3.right * Mathf.Lerp(s.x, s.z, Mathf.Abs(Mathf.Sin(angle / 360 * 2 * Mathf.PI))) * .5f;
		}
	}
}
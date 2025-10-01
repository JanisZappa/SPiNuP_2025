using UnityEngine;

namespace Test
{
	public class RotCompare : MonoBehaviour
	{
		public Transform a, b;

		private float angle;

		private void Update()
		{
			angle -= Time.deltaTime * 90;

			a.rotation = Quaternion.AngleAxis(angle, Vector3.right);
			b.rotation = Rot.X(angle);
		}
	}
}
using System.Collections;
using UnityEngine;

namespace Test
{
	public class ActivateInRow : MonoBehaviour
	{
		private CircleSquare[] row;
		public float radius;


		private void Start()
		{
			row = GetComponentsInChildren<CircleSquare>();
			for (int i = 0; i < row.Length; i++)
				row[i].gameObject.SetActive(false);

			StartCoroutine(YieldActivate());
		}


		private IEnumerator YieldActivate()
		{
			float offset = 0;
			for (int i = 0; i < row.Length; i++)
			{
				offset += Time.deltaTime;

				row[i].id = i;
				row[i].t = offset + i * .12f;
				row[i].radius = radius;
				row[i].gameObject.SetActive(true);

				radius *= .85f;


				yield return null;
			}
		}
	}
}
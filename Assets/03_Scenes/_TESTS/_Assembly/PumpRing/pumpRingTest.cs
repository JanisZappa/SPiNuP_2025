using UnityEngine;

namespace Test
{
	public class pumpRingTest : MonoBehaviour
	{
		public GameObject ring;
		public int rings;
		public float scale;
		private Material[] mats;

		private float anim;
		private static readonly int Scale = Shader.PropertyToID("_scale");
		private static readonly int Blob = Shader.PropertyToID("_blob");
		private static readonly int Lerp = Shader.PropertyToID("_lerp");


		private void Start()
		{
			mats = new Material[rings];
			mats[0] = ring.GetComponent<MeshRenderer>().material;
			ring.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) *
			                          Quaternion.AngleAxis(Random.Range(-2f, 2f), Vector3.forward);

			for (int i = 1; i < rings; i++)
			{
				GameObject inst = Instantiate(ring);
				inst.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) *
				                          Quaternion.AngleAxis(Random.Range(-2f, 2f), Vector3.forward);
				mats[i] = inst.GetComponent<MeshRenderer>().material;
			}
		}


		private void Update()
		{
			anim += Time.deltaTime * .2f;

			for (int i = 0; i < rings; i++)
			{
				float lerp = (i * 1f / rings + anim) % 1;
				mats[i].SetFloat(Scale, (1 - Mathf.Pow(1 - lerp, 3)) * scale);
				mats[i].SetFloat(Blob, Mathf.SmoothStep(0, 2, Mathf.PingPong(lerp * 2, 1)));

				mats[i].SetFloat(Lerp, -.45f + lerp * 1.65f);
			}
		}
	}
}
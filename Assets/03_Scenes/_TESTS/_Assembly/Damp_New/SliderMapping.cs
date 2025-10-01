using UnityEngine;

namespace Test
{
    public class SliderMapping : MonoBehaviour
    {
        public Transform a, b, c;

        [Space(10)] public float speed;
        [Space(10)] public float length;

        private float r;

        public Vector2 remap;

        private void Update()
        {
            Vector3 pointA = V3.left * length * .5f;
            Vector3 pointB = V3.right * length * .5f;

            r += Time.deltaTime * speed;
            float aRange = Mth.SmoothPP(0, 1, r);
            float bRange = Mth.DampedRange(aRange, remap.x, remap.y);

            a.position = Vector3.Lerp(pointA, pointB, aRange) + V3.up * 2;
            b.position = Vector3.Lerp(pointA, pointB, bRange) + V3.down * 2;

            c.position = Vector3.Lerp(pointA, pointB, Ease.FF(Mathf.PingPong(r, 1))) + V3.down * 4;
        }
    }
}
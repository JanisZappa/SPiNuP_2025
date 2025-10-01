using UnityEngine;

namespace Test
{
    public class SmallestFitCircle : MonoBehaviour
    {
        [Range(2, 20)] public int segments = 3;
        public float segmentLength;

        [Space(10)] [Range(0, 1)] public float lerp;

        private float t;


        private void Update()
        {
            t += Time.deltaTime * .3f;


            float angle = 2 * Mathf.PI / segments;
            float halfAngle = angle * .5f;
            float opposite = segmentLength * .5f;
            float radius = opposite / Mathf.Sin(halfAngle);


            DRAW.Circle(Vector2.zero, radius, 100).SetColor(Color.white);

            HLS hls = Color.yellow.ToHLS();
            Vector2 last = (Vector2.up * radius).RotRad(t);
            float angleStep = angle * lerp;

            Vector2 dir = (Vector2.left * segmentLength).RotRad(t + angleStep * .5f);

            for (int i = 0; i < segments; i++)
            {
                DRAW.Vector(last, dir).SetColor(hls.ShiftHue(i * .1f));
                last += dir;
                dir = dir.RotRad(angleStep);
            }

            DRAW.Vector(Vector3.right * segmentLength * -.5f, Vector3.right * segmentLength).SetColor(Color.yellow);
        }
    }
}

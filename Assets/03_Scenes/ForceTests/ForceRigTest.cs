using UnityEngine;

namespace Test
{
    public class ForceRigTest : MonoBehaviour
    {
        public float maxLength;
        [Range(0, 1)] public float split;
        public Vector2 headOffset;
        private Vector2 root, goal, animRoot;

        private QuaternionForce dirA, dirB;
        private Vector2Force rootForce, headForce;
        private Vector2 steer;

        private float t;


        private void Start()
        {
            root = transform.position;
            goal = root + Vector2.down * .001f;

            dirA = new QuaternionForce(GPhysics.ForceFPS).SetSpeed(350).SetDamp(10);
            dirB = new QuaternionForce(GPhysics.ForceFPS).SetSpeed(500).SetDamp(6);
            rootForce = new Vector2Force(GPhysics.ForceFPS).SetSpeed(150).SetDamp(10).SetValue(root);
            headForce = new Vector2Force(GPhysics.ForceFPS).SetSpeed(500).SetDamp(3).SetValue(root + headOffset);
        }


        private void Update()
        {
            const float step = 1f / 200;
            float dt = Time.deltaTime;
            if (t > 0)
            {
                UpdateStep(t);
                dt -= t;
            }

            while (dt > step)
            {
                UpdateStep(step);
                dt -= step;
            }

            if (dt > 0)
            {
                UpdateStep(dt);
                t = step - dt;
            }


            DRAW.Circle(headForce.Value, 8, 40).SetColor(COLOR.yellow.greenish);

            float c = maxLength * split;
            float a = maxLength * (1f - split);
            Vector2 arm1 = dirA.Value * Vector3.forward * c;
            Vector2 arm2 = dirB.Value * Vector3.forward * a;

            Vector2 smoothRoot = rootForce.Value;
            DRAW.Vector(smoothRoot, arm1).SetColor(COLOR.purple.orchid);
            DRAW.Vector(smoothRoot + arm1, arm2).SetColor(COLOR.turquois.bright);
        }


        private void UpdateStep(float dt)
        {
            steer = Vector2.Lerp(steer, new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), dt * 50);
            Vector2 smoothRoot = rootForce.Update(root + steer * 2, dt);

            Vector2 newPos = goal + steer * dt * 300;
            Vector2 dir = newPos - smoothRoot;

            float mag = dir.magnitude;
            Vector2 dN = dir.normalized;
            float lerp = Mathf.Clamp(Mathf.Clamp01(mag / maxLength), .15f, .995f);
            dir = dN * maxLength * lerp;

            goal = smoothRoot + dir;

            float c = maxLength * split;
            float b = maxLength * lerp;
            float a = maxLength * (1f - split);

            float aS = a * a;
            float bS = b * b;
            float cS = c * c;

            float angleA = Mathf.Acos((bS + cS - aS) / (2 * b * c));
            float angleB = Mathf.Acos((aS + cS - bS) / (2 * a * c));
            float angleC = Mathf.PI - angleA - angleB;

            Vector2 dA = dN.RotRad(angleA);
            Vector2 dB = dN.RotRad(-angleC);

            Quaternion tA = Quaternion.LookRotation(dA, Vector3.forward);
            Quaternion tB = Quaternion.LookRotation(dB, Vector3.forward);

            dirA.Update(tA, dt, true);
            dirB.Update(tB, dt, true);


            headForce.Update(smoothRoot + headOffset, dt);
        }
    }
}
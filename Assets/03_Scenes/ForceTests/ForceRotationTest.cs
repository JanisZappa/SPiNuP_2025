using UnityEngine;

namespace Test
{
    public class ForceRotationTest : MonoBehaviour
    {
        private Vector3Force posForce;
        private QuaternionForce rotForce;
        private bool dirFixed;

        private Vector3 pos;
        private Quaternion goalRot, fixedRot;
        private float speed;


        public void Init()
        {
            Quaternion baseRot = Random.rotation;

            posForce = new Vector3Force(GPhysics.ForceFPS).SetSpeed(Random.Range(150, 270f))
                .SetDamp(Random.Range(7, 12f));
            rotForce = new QuaternionForce(GPhysics.ForceFPS).SetSpeed(Random.Range(70, 470f))
                .SetDamp(Random.Range(2, 4f)).SetValue(baseRot);
            speed = Random.Range(360f, 960);

            pos = transform.localPosition;
            goalRot = baseRot;
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                dirFixed = !dirFixed;

                if (dirFixed)
                {
                    fixedRot = Quaternion.Slerp(goalRot, Quaternion.identity, 1f);
                }
                else
                {
                    goalRot = Quaternion.Slerp(fixedRot, goalRot, 1f);
                }
            }


            /*Vector3 force;
            if (!dirFixed)
            {
                Vector3 target = new Vector3(Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal"), 0) * 720;
                force = angularForce.Update(target, Time.deltaTime);
            }
            else
            {
                Quaternion toNormal = Quaternion.FromToRotation(rot * Vector3.forward, Vector3.forward);
                Vector3 e = toNormal.eulerAngles;
                Vector3 e2 = angularForce.Value;
                Vector3 s = new Vector3(
                    Mathf.LerpAngle(e.x, e2.x, 0f),
                    Mathf.LerpAngle(e.y, e2.y, 0f),
                    Mathf.LerpAngle(e.z, e2.z, 0f));

                force = angularForce.Update(e, Time.deltaTime);
            }


            rot = Quaternion.Euler(force * Time.deltaTime) * rot;
            transform.rotation = rot;*/

            Quaternion target =
                Quaternion.Euler(new Vector3(Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal"), 0) * speed *
                                 Time.deltaTime);
            goalRot = target * goalRot;

            transform.rotation = rotForce.Update(dirFixed ? fixedRot : goalRot, Time.deltaTime);

            float
                z = 0; //Mathf.Pow(Mathf.Abs(Mathf.Sin((Time.realtimeSinceStartup * 4 + pos.x * .5f) * 3)), 200) * -40;
            transform.localPosition =
                pos + posForce.Update(
                    (dirFixed
                        ? Vector3.zero
                        : new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * 1.5f).SetZ(z),
                    Time.deltaTime) * .25f;
        }
    }
}
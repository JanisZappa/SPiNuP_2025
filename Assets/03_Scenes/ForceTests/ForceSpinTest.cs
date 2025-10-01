using UnityEngine;

namespace Test
{
    public class ForceSpinTest : MonoBehaviour
    {
        private readonly FloatForce pauseForce = new FloatForce(GPhysics.ForceFPS);

        private float target;


        private void Start()
        {
            pauseForce.SetSpeed(Random.Range(100, 370f)).SetDamp(Random.Range(3, 8f)).SetValue(0).SetForce(0);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                target += 180;

            transform.rotation = Quaternion.Euler(0, pauseForce.Update(target, Time.deltaTime), 0);
        }
    }
}
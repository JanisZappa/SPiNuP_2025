using UnityEngine;

namespace Test
{
    public class ForceFollowTest : MonoBehaviour
    {
        Vector2Force posForce, aimForce;

        private void Start()
        {
            posForce = new Vector2Force(GPhysics.ForceFPS).SetSpeed(Random.Range(100, 270f))
                .SetDamp(Random.Range(3, 8f));
            aimForce = new Vector2Force(GPhysics.ForceFPS).SetSpeed(Random.Range(100, 270f))
                .SetDamp(Random.Range(3, 8f));
        }


        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (new Plane(Vector3.forward, Vector3.zero).Raycast(ray, out float enter))
            {
                Vector2 target = (ray.origin + ray.direction * enter).V2();
                Vector2 newPos = posForce.Update(target, Time.deltaTime);
                transform.position = newPos;

                Vector2 offset = aimForce.Update(target - newPos, Time.deltaTime);
                transform.rotation = Quaternion.Euler(offset.y, offset.x, 0);
            }
        }
    }
}
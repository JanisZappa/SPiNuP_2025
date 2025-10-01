using GeoMath;
using UnityEngine;

namespace Test
{
    public class LineSide : MonoBehaviour
    {
        public Line line;
        public float radAngle;

        private void Update()
        {
            line.Draw();

            radAngle = line.dir.RadAngle((Camera.main.ScreenToWorldPoint(Input.mousePosition).V2() - line.l1));
        }
    }
}

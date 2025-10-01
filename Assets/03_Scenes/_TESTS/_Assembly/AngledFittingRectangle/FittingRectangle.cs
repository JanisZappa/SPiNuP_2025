using GeoMath;
using UnityEngine;

namespace Test
{
    public class FittingRectangle : MonoBehaviour
    {
        public float width;
        public float height;
        private float angle;


        private void OnEnable()
        {
            DRAW.Enabled = true;
        }


        private void Update()
        {
            DRAW.Rectangle(V3.zero, new Vector2(width, height)).SetColor(COLOR.red.firebrick);

            angle = Mathf.Lerp(angle, angle + Input.GetAxis("Horizontal") * -15, Time.deltaTime * 10);

            DRAW.Rectangle(V3.zero, Rectangle.FitRotationSize(new Vector2(width, height), angle), angle)
                .SetColor(COLOR.yellow.fresh).Fill();
        }
    }
}
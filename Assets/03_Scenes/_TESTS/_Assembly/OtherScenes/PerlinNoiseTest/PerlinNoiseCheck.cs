using UnityEngine;


public class PerlinNoiseCheck : MonoBehaviour {

    public int squareSize;
    public float xOffset;
    public float yOffset;
    [Space(10)]
    [Range(-2,2)]
    public float xMulti;
    [Range(-2, 2)]
    public float yMulti;

    [Space(10)]
    public AnimationCurve curve;

    private Camera cam;

    
    private void OnEnable()
    {
        DRAW.Enabled = true;
        cam = Camera.main;
    }

    
    private void Update ()
    {

        xOffset += Input.GetAxis("Horizontal") * -Time.deltaTime;
        yOffset += Input.GetAxis("Vertical") * -Time.deltaTime;

	        for(int x = 0; x < squareSize; x++ )
                for(int y = 0; y < squareSize; y++ )
                {
                    float value = curve.Evaluate(Mathf.PerlinNoise(xOffset + x * xMulti, yOffset + y * yMulti));
                    Color c = Color.Lerp(COLOR.red.hot, Color.white, value);
                float size = Mathf.Lerp(.2f, .85f, value);
                float angle = Mathf.Lerp(135f, 0, value);
                    DRAW.Rectangle(new Vector3(x - (squareSize * .5f), y - (squareSize * .5f), 0), Vector2.one * size, angle).SetColor(c);
                }

            if(cam != null)
                cam.orthographicSize = squareSize * .5f + 4;
	}
}

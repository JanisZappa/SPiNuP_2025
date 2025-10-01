using UnityEngine;


public class ColorPacking : MonoBehaviour 
{
	[Range(0,4095)]
    public int colorID;
    [Range(0,3)]
    public int matCap;

    [Header("Floats")] 
    public float r, g;
    
    [Header("Results")]
    public int resultID, resultMatCap;
    
	
    private void Update ()
    {
	    {
		    int matCapOffset = Mathf.FloorToInt(colorID / 256.0f);
		    int colorValue   = colorID % 256;
		    int matCapValue  = matCapOffset * 16 + matCap;

		    r = colorValue  / 256.0f;
		    g = matCapValue / 256.0f;
	    }

	    {
		    int colorValue   = Mathf.FloorToInt(r * 256.0f);
		    int matCapValue  = Mathf.FloorToInt(g * 256.0f);
		    int matCapOffset = Mathf.FloorToInt(matCapValue / 16.0f);

		    resultID     = matCapOffset * 256 + colorValue;
		    resultMatCap = matCapValue % 16;
	    }
    }
}

using UnityEngine;


public class LightColorTestObject : MonoBehaviour
{
	public Color color;
	public MeshRenderer mR;


	private void OnEnable()
	{
		UpdateColor();
	}

	private void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Space))
			UpdateColor();
		
		LightColorTest.UpdateMe(this);
	}


	private void UpdateColor()
	{
		const float yShift = 1f / 18;
		color = new Color(1, 0, 0, 1).ToHLS().ShiftHue(Random.Range(0, 18) * yShift);
	}
}

using UnityEngine;


public class OffsetTest : MonoBehaviour
{
	public Transform cylinder, cube;
	public float cylinderRadius;
	public Vector3 cubeSize;

	private float spinAngle;

	private float leanX, leanY;


	private void Update()
	{
		spinAngle += Time.deltaTime * 100;
		Quaternion spin = Rot.Z(spinAngle);
		
		
		leanX = leanX.AxisΔ("Horizontal", 40).Clamp(-45, 45);
		leanY = leanY.AxisΔ("Vertical",   40).Clamp(-45, 45);
		Quaternion lean = Quaternion.Euler(leanY, -leanX, 0);
		
		
		cube.localScale = cubeSize;
		cylinder.localScale = new Vector3(cylinderRadius * 2, 1, cylinderRadius * 2);

		Vector3 spinOffset = new Vector3(cubeSize.x * .5f, cubeSize.y * .5f, 0) + new Vector3(1, 1, 0).normalized * cylinderRadius;

		
		cube.position = lean * (spin * -spinOffset);
		cube.rotation = lean * spin;

		cylinder.rotation = lean * Rot.X(90);
	}
}

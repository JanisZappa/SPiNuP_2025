using UnityEngine;


[ExecuteInEditMode]
public class AnimalCrossingTiler : MonoBehaviour
{
	public Transform cam;

	
	private Vector3 pos, smoothPos;

	private static readonly int ToWorld     = Shader.PropertyToID("_ToWorld");
	private static readonly int ToLocal     = Shader.PropertyToID("_ToLocal");
	private static readonly int WorldNormal = Shader.PropertyToID("_ToWorldNormal");
	private static readonly int MakeFlat    = Shader.PropertyToID("_MakeFlat");
	private static readonly int Center      = Shader.PropertyToID("_Center");
	
	private int tileCount;

	private bool flat;
	
	
	private void Start ()
	{
		Shader.SetGlobalFloat(MakeFlat, flat? 1 : 0);
	}
	
	
	private void Update ()
	{	
		if (Input.GetKeyDown(KeyCode.F))
		{
			flat = !flat;
			Shader.SetGlobalFloat(MakeFlat, flat? 1 : 0);
		}
		
		
		pos += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * 17; //-12;
		smoothPos = Vector3.Lerp(smoothPos, pos, Time.deltaTime * 12);
		
		Shader.SetGlobalVector(Center, smoothPos);

		cam.transform.position = smoothPos;
		Quaternion rot = Quaternion.AngleAxis(0, Vector3.up);

		cam.transform.rotation = rot;
		
		
		Shader.SetGlobalMatrix(ToWorld, transform.localToWorldMatrix);
		Shader.SetGlobalMatrix(ToLocal, transform.worldToLocalMatrix);
		Shader.SetGlobalMatrix(WorldNormal, Matrix4x4.Rotate(transform.rotation * Quaternion.AngleAxis(0, Vector3.up)));
	}
}

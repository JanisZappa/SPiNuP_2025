using UnityEngine;


public class PrintColorValue : MonoBehaviour 
{
	private void Start ()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Color color = mesh.colors[0];
		Debug.Log(color.b);
	}
}

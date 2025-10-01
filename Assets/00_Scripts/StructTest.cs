using UnityEngine;


public class StructTest : MonoBehaviour {

	private struct MyStruct
	{
		public int a;
		public float b;

		public MyStruct(int a, float b)
		{
			this.a = a;
			this.b = b;
		}
	}

	private MyStruct s = new MyStruct(0, 0);
	
	private void Update () 
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			s.a++;
			s.b += 2.354f;
			
			Debug.Log(s.a);
			Debug.Log(s.b);
		}
	}
}

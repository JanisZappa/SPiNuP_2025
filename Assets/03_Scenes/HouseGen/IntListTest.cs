using UnityEngine;

public class IntListTest : MonoBehaviour 
{
	private readonly UniqueList list = new UniqueList(10);
	
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.A))
			list.Add(Random.Range(0, 10));
		
		if(Input.GetKeyDown(KeyCode.R))
			list.Remove(Random.Range(0, 10));
		
		if(Input.GetKeyDown(KeyCode.T))
			list.RemoveAt(Random.Range(0, 10));
		
		/*List<int> banana = new List<int>();
		banana.Remove(0);*/
	}
}

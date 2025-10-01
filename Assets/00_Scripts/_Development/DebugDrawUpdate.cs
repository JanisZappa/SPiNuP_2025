using Future;
using UnityEngine;


public class DebugDrawUpdate : MonoBehaviour
{
	private static DebugDrawUpdate Inst;

	public static BoolSwitch DrawAnything = new ("Dev/Draw Update",
		true, b =>
		{
			if (b && !Inst)
				Inst = new GameObject("DebugDrawUpdate").AddComponent<DebugDrawUpdate>();
			
			if(!b && Inst)
				Destroy(Inst.gameObject);
		}
	);

	private void LateUpdate () 
	{
		SpinnerDebug.DebugUpdate();
		  LevelDebug.DebugUpdate();
		  Prediction.ShowPrediction();
	}
}

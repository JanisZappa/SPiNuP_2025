using Future;
using UnityEngine;


public class DebugDrawUpdate : MonoBehaviour
{
	private static DebugDrawUpdate Inst;

	public static BoolSwitch DrawAnything = new ("Dev/Draw Update", true);


	private void LateUpdate () 
	{
        if(!DrawAnything)
            return;
        
		SpinnerDebug.DebugUpdate();
		  LevelDebug.DebugUpdate();
		  Prediction.ShowPrediction();
	}
}

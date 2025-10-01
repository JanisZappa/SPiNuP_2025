using UnityEngine;


public static class DebugLogToggle 
{
	public static readonly BoolSwitch LogsEnabled =
		new("Dev/Debug Log", true, b =>
		{
			Debug.unityLogger.logEnabled = b;
		});
}

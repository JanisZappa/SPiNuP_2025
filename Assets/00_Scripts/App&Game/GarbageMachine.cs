using System;
using System.Runtime.InteropServices;


public static class GarbageMachine 
{
//  Unity engine function to disable the GC  //
	[DllImport("__Internal")]
	private static extern void GC_disable();
 
//  Unity engine function to enable the GC  //
	[DllImport("__Internal")]
	private static extern void GC_enable();


	public static void Collect()
	{
		#if !UNITY_EDITOR_WIN && !UNITY_STANDALONE
		GC_enable();
		#endif
		
		GC.Collect();
		
		#if !UNITY_EDITOR_WIN && !UNITY_STANDALONE
		GC_disable();
		#endif
	}
}

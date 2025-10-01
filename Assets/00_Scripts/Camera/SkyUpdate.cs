using UnityEngine;


public class SkyUpdate : Singleton<SkyUpdate>
{
	[Header("Z Size Before: 200")]
	
	public Vector3 size;
	public float zOffset;
	
	private static bool front;
	private static readonly int SkyCenter = Shader.PropertyToID("SkyCenter");
	private static readonly int SkySize   = Shader.PropertyToID("SkySize");
	
	
	private void LateUpdate()
	{
		Shader.SetGlobalVector(SkyCenter, GameCam.CurrentPos.V3(GameCam.CurrentSide.Sign * zOffset));
		Shader.SetGlobalVector(SkySize, size);
	}
}

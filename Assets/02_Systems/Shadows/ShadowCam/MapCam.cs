using GeoMath;
using UnityEngine;


public class MapCam : MonoBehaviour
{
	public Camera        cam;
	public RenderTexture mapTex;

	public bool mapA;

	public LayerMask[] masks;

	[HideInInspector]
	public bool active;

	
	private static Vector2 mapCenterA, mapCenterB;
	public static float mapSizeA;
	private static float mapSizeB;
	
	private static readonly int MapA = Shader.PropertyToID("MapA"), 
		                        MapB = Shader.PropertyToID("MapB");

	public void SetActive(bool active)
	{
		this.active = active;
		
		Shader.SetGlobalTexture(mapA? "ShadowTex" : "LightTex", active? mapTex : null);
		cam.enabled = active;
	}


	private void LateUpdate()
	{
		if (!GameManager.Running || !active)
			return;


		Vector2 center = mapA? mapCenterA : mapCenterB;
		float   size   = mapA? mapSizeA   : mapSizeB;
		
		Vector3 pos = new Vector3(Mathf.Round(center.x), Mathf.Round(center.y), -10);

		transform.position   = pos;
		cam.orthographicSize = size;

		Shader.SetGlobalVector(mapA? MapA : MapB, new Vector4(pos.x - size, pos.y - size, .5f / size, size));

		cam.cullingMask = masks[GameCam.CurrentSide.front ? 0 : 1];
	}


	public static void UpdateMapQuads(Bounds2D shadow, Bounds2D light)
	{
		mapCenterA = shadow.Center;
		mapSizeA   = shadow.MaxSide * .5f + .5f;

		mapCenterB = light.Center;
		mapSizeB   = light.MaxSide * .5f + .5f;
	}


	public static void MapOutlineDRAW()
	{
		DRAW.Rectangle(mapCenterA, V2.one * mapSizeA * 2).SetColor(COLOR.yellow.fresh).SetDepth(Z.W10);
		DRAW.Rectangle(mapCenterB, V2.one * mapSizeB * 2).SetColor(COLOR.orange.coral).SetDepth(Z.W10);
	}
}

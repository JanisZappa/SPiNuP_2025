using System;
using UnityEngine;


public class SunDir : MonoBehaviour
{
	[Range(0, 1)]
	public float lerpA, lerpB;

	[Space(10)] 
	public float sunRange;
	public float heightMulti, heightMultiB;


	[Space(10)] public bool animLight, useSystemTime;
	public float animSpeed;
	[Range(.05f, .95f)] public float t;

	[Space(10)] 
	public bool setSunDir, draw;

	public float animTime;

	[Space(10)] [Range(0, 1)] public float sunPowLerp;
	
	private static readonly int SunPowLerp             = Shader.PropertyToID("SunPowLerp");
	private static readonly int BottomLightBounceMulti = Shader.PropertyToID("BottomLightBounceMulti");
	private static readonly int HeightLightAdd         = Shader.PropertyToID("HeightLightAdd");
	
	
	public static float DayTimeLerp;



	private void LateUpdate()
	{
		if (!GameManager.Running)
			return;

		if (Input.GetKeyDown(KeyCode.Alpha5))
			animLight = !animLight;


		if (animLight)
			animTime += Time.deltaTime * animSpeed * .005f;
		else if (useSystemTime)
		{
			const float minHour = 5.04f,
				maxHour = 20.4f,
				minSecond = minHour * 60 * 60,
				maxSecond = maxHour * 60 * 60;

			DateTime now = DateTime.Now;
			float currentSecond = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;

			animTime = Mathf.InverseLerp(minSecond, maxSecond, currentSecond);
		}
		else
			animTime = t;


		DayTimeLerp = Mathf.Repeat(animTime, 1);

		float range = sunRange + GameCam.CurrentPos.y * heightMulti;
		Vector3 sunDir = GetDir(Mathf.Lerp(-range * .5f, range * .5f, DayTimeLerp));


		if (setSunDir)
		{
			float colorLerp = Mathf.Lerp(DayTimeLerp, .5f, GameCam.CurrentPos.y * heightMultiB);
			LightManager.SetSunAngleLerp(Quaternion.FromToRotation(Vector3.forward, -sunDir), colorLerp);

			sunPowLerp = Mathf.PingPong(DayTimeLerp * 2, 1);
			Shader.SetGlobalFloat(SunPowLerp, sunPowLerp);
			Shader.SetGlobalFloat(BottomLightBounceMulti, .75f + (1 - sunPowLerp) * 1.25f);
			Shader.SetGlobalFloat(HeightLightAdd, Mathf.Clamp01((1 - sunPowLerp) * .2f));
		}

		
		if (draw)
		{
			if (!setSunDir)
				DRAW.GapVector(GameCam.CurrentPos.V3(Z.W), LightingSet.SunRot * Vector3.back * 20, 40).SetColor(COLOR.green.lime);

			DRAW.GapVector(GameCam.CurrentPos.V3(Z.W), sunDir * 20, 40).SetColor(COLOR.red.firebrick);

			const int steps = 100;
			const float angle = 360f / steps;

			for (int i = 0; i < steps; i++)
				DRAW.Vector(GameCam.CurrentPos.V3(Z.W), GetDir(i * angle) * 20).SetColor(COLOR.yellow.fresh.A(.15f));
		}
	}
	

	private Vector3 GetDir(float angle)
	{
		Quaternion rot = Rot.Z(angle);
		Vector3    dir = rot * Vector3.up;

		float tiltLerp = Mathf.Abs(Vector3.Dot(Vector3.up, dir));

		return Vector3.Slerp(dir, Vector3.back, Mathf.SmoothStep(lerpA, lerpB, tiltLerp));
	}
}
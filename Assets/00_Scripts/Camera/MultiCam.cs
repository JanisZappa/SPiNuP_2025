using System.Collections;
using UnityEngine;


public class MultiCam : Singleton<MultiCam>
{
	public Camera mainCam;
	
	[Space(10)]
	public AnimationCurve shakeTest;

	private static Vector2 bgShake;


	public void CamUpdate(Quaternion camRot)
	{
	//	BackCam Position	//
		//TODO? Shake Stuff
		//backCam.transform.localPosition = backCam.transform.InverseTransformDirection(bgShake.V3());
	}
	

	public static void Shake(float force)
	{
		return;
		Inst.StartCoroutine(Inst.ShakeBack(force));
	}
	
	
	private IEnumerator ShakeBack(float force)
	{
		float count = 0;
		Vector2 backDir = V2.up.RotRad(Random.Range(0, Mth.FullRad));
		while (count < 1)
		{
			count += Time.deltaTime * 3;
			bgShake = backDir * shakeTest.Evaluate(count) * (1 + force * .1f);
			yield return null;
		}
	}
}


/*if (skew)
		{
		//	Skew	//
			Vector3 oldForward = currentRot * V3.forward;
					currentRot = camRot;
			Vector3 newForward = currentRot * V3.forward;
			bool    sideChange = !f.Same(Mathf.Sign(oldForward.z), Mathf.Sign(newForward.z));
			
			change = sideChange ? new Vector3(0, change.magnitude, 0) : Quaternion.FromToRotation(oldForward,newForward).eulerAngles;
			xLerp  = Mathf.Lerp(xLerp, change.x.Euler180() * .12f, Time.deltaTime * 50);
			yLerp  = Mathf.Lerp(yLerp, change.y.Euler180() * .12f, Time.deltaTime * 50);
		
			mainCam.NewSkew(new Vector2(yLerp, xLerp) * .2f, .5f);
			backCam.NewSkew(new Vector2(yLerp, xLerp), .1f);
		}*/


/*[System.Serializable]
	public class Cam
	{
		public Camera camera;

		
		public void SetFOV(float fov)
		{
			//camera.ResetProjectionMatrix();
			camera.fieldOfView = fov;
		}
		
		
		public void NewSkew(Vector2 dir, float counterMove = 0)
		{
			float dir_M = dir.magnitude;
			Matrix4x4 rotMatrix       = Matrix4x4.TRS(V3.zero, Quaternion.FromToRotation(V3.up, dir * (dir_M > 0? 1f / dir_M : 0)), V3.one);
			camera.projectionMatrix   = Matrix4x4.Inverse(rotMatrix) * (Matrix4x4.Translate(V3.down * dir_M * .5f * counterMove) * (Matrix4x4.Scale(new Vector3(1, 1 + dir_M, 1)) * (rotMatrix * camera.projectionMatrix)));
			//camera.projectionMatrix = Matrix4x4.Inverse(rotMatrix) * (Matrix4x4.Scale(new Vector3(1, 1 + dir.magnitude, 1)) * (rotMatrix * camera.projectionMatrix));
		}
	}*/
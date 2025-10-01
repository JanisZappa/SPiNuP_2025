using UnityEngine;


public class LightColorTest : Singleton<LightColorTest>
{
	public Color colorA;
	[Range(0,1)]
	public float colorALerp, colorALuminance, colorASaturation;
	
	[Space(20)] 
	public Color colorB;
	[Range(0,1)]
	public float colorBLerp, colorBLuminance, colorBSaturation;
	
	
	private static readonly int SunDir = Shader.PropertyToID("SunDir");
	private static readonly int ColorA = Shader.PropertyToID("ColorA");
	private static readonly int ColorB = Shader.PropertyToID("ColorB");

	private void Update () 
	{
		Shader.SetGlobalVector(SunDir, transform.up);
	}


	public static void UpdateMe(LightColorTestObject thing)
	{
		thing.mR.material.SetColor(ColorA, Color.Lerp(thing.color, Inst.colorA, Inst.colorALerp).ToHLS().SetLuminace(Inst.colorALuminance).SetSaturation(Inst.colorASaturation));
		thing.mR.material.SetColor(ColorB, Color.Lerp(thing.color, Inst.colorB, Inst.colorBLerp).ToHLS().SetLuminace(Inst.colorBLuminance).SetSaturation(Inst.colorBSaturation));

		return;
		thing.mR.material.SetColor(ColorA, thing.color.ToHLS().HueSlerp(Inst.colorA.ToHLS(), Inst.colorALerp).SetLuminace(Inst.colorALuminance).SetSaturation(Inst.colorASaturation));
		thing.mR.material.SetColor(ColorB, thing.color.ToHLS().HueSlerp(Inst.colorB.ToHLS(), Inst.colorBLerp).SetLuminace(Inst.colorBLuminance).SetSaturation(Inst.colorBSaturation));
	}
}

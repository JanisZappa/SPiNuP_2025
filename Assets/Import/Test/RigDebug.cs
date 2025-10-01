using LevelElements;
using UnityEngine;


public class RigDebug 
{
	private static readonly BoolSwitch showRotators = new("Char/Rig Rotators", false);
	
	
	private bool drawSticks, drawBones, drawBend;
	private readonly RigPoser poser;
	

	public RigDebug(RigPoser poser)
	{
		DRAW.Enabled = DRAW.EditorDraw = true;
		this.poser    = poser;
	}


	public void Update()
	{
		if(showRotators)
			ActualPivot();
		
		return;
		
		ShowRootBones();
		DrawLimbs();
		DrawBend();
		DrawBendChain();
	}


	private void ShowRootBones()
	{
		for (int i = 0; i < poser.rig.rigParts.Length; i++)
		DRAW.Circle(poser.transform.TransformPoint(poser.rig.rigParts[i].rootBone.turnPos), .25f, 20).Fill(1);
	}
	

	private void ActualPivot()
	{
		if (poser.transform.position.y < -100)
			return;

		
		Vector3 line = poser.transform.rotation * V3.forward * Level.PlaneOffset * .4f;
		DRAW.Vector(poser.transform.position + line, line * -2).SetColor(COLOR.red.tomato);
		DRAW.Vector(poser.HandRootPos.pos       + line, line * -2).SetColor(COLOR.red.tomato);
		DRAW.Vector(poser.FootRootPos.pos       + line, line * -2).SetColor(COLOR.red.tomato);
		
		
		for (int i = 0; i < 7; i++)
		{
			Vector3 offset = -line + line * 2 / 6f * i;
			DRAW.Circle(poser.HandRootPos.pos + offset, poser.handRadius, 12).SetColor(COLOR.red.tomato).Fill(1);
			DRAW.Circle(poser.FootRootPos.pos + offset, poser.footRadius, 12).SetColor(COLOR.red.tomato).Fill(1);
		}
		
		
		DRAW.Circle(poser.HandRootPos.pos, poser.handRadius + .15f, 12).SetColor(COLOR.red.tomato);
		DRAW.Circle(poser.FootRootPos.pos, poser.footRadius + .15f, 12).SetColor(COLOR.red.tomato);
		
		DRAW.Arrow (poser.HandRootPos.pos, poser.HandRootPos.rot * V3.up    *.6f, .2f).SetColor(COLOR.red.tomato).Fill(1);
		DRAW.Arrow (poser.FootRootPos.pos, poser.FootRootPos.rot * V3.down  *.6f, .2f).SetColor(COLOR.red.tomato).Fill(1);

		
		Vector3 bodyV = poser.transform.rotation * new Vector3(0, poser.height * .5f, 0);
		DRAW.Vector(poser.transform.position - bodyV, bodyV * 2).SetColor(COLOR.yellow.fresh);
	}


	private void DrawBendChain()
	{
		if (!GameManager.Running)
			return;

		for (int c = 0; c < poser.mainChains.Length; c++)
			for (int i = 0; i < poser.mainChains[c].boneCount - 1; i++)
			{
				Vector3 p1 = poser.transform.TransformPoint(poser.mainChains[c].bones[i].bendPos);
				Vector3 p2 = poser.transform.TransformPoint(poser.mainChains[c].bones[i + 1].bendPos);

				Color color = c == 0? COLOR.blue.cornflower : (c == 1? COLOR.red.tomato : COLOR.green.spring);
				
				DRAW.Vector(p1, p2 - p1).SetColor(color);
			}
		
		for (int c = 0; c < 2; c++)
			DRAW.Circle(poser.transform.TransformPoint(poser.mainChains[c].Tip.bendPos), Item.DefaultRadius + .2f, 12).SetColor(COLOR.orange.coral);
	}


	private void DrawLimbs()
	{
		drawBones = drawBones.KeySwitch(KeyCode.I);
		
		if(!drawBones)
			return;

		Vector3    bodyPos = poser.transform.position;
		Quaternion bodyRot = poser.transform.rotation;

		RigPart[] rigParts = poser.rig.rigParts;
		for (int i = 0; i < rigParts.Length; i++)
		{
			for (int e = 0; e < rigParts[i].boneCount - 1; e++)
			{
				Color c = Color.Lerp(COLOR.blue.cornflower, COLOR.purple.violet, e / (rigParts[i].boneCount - 2f));
				DRAW.DotVector(bodyPos + bodyRot * rigParts[i].bones[e].turnPos, bodyRot * (rigParts[i].bones[e + 1].turnPos - rigParts[i].bones[e].turnPos), .045f, .005f).SetColor(c);
			}
			
			if(i < 2)
				continue;
			
			DRAW.GapVector(bodyPos + bodyRot * rigParts[i].rootBone.turnPos, bodyRot * (rigParts[i].rootBone.bendRot * V3.up * rigParts[i].rootChainLength), 5).SetColor(COLOR.red.tomato);
		}
	}

	
	

	private const int pC = 100;
	private readonly Vector3[][][] lines =
	{
		new[] { new Vector3[pC], new Vector3[pC], new Vector3[pC], new Vector3[pC], new Vector3[pC]},
		new[] { new Vector3[pC], new Vector3[pC], new Vector3[pC], new Vector3[pC], new Vector3[pC]},
		new[] { new Vector3[pC], new Vector3[pC], new Vector3[pC], new Vector3[pC], new Vector3[pC]}
	};


	private void DrawBend()
	{
		drawBend = drawBend.KeySwitch(KeyCode.O);
		
		if(!drawBend)
			return;
		
		Vector3    bodyPos = poser.transform.position;
		Quaternion bodyRot = poser.transform.rotation;
		
		float lineLength = poser.height * 2;

		for (int i = 0; i < lines.Length; i++)
		{
			float lerp = i / (lines.Length - 1f);
			float z    = Mathf.Lerp(poser.size.z * -.5f, poser.size.z * .5f,  i / (lines.Length - 1f));
			Color c    = Color.Lerp(COLOR.green.lime, COLOR.yellow.fresh, lerp).A(.25f);
			
			for (int e = 0; e < lines[i].Length; e++)
			{
				float x = Mathf.Lerp(poser.size.x * -.5f, poser.size.x * .5f, e / (lines[i].Length - 1f));
				for (int j = 0; j < lines[i][e].Length; j++)
					lines[i][e][j] = bodyPos + bodyRot * poser.GetBendPlacement( new Vector3(x, -lineLength * .5f + j * (lineLength / (lines[i][e].Length - 1)), z).MultiBy(poser.squashScale) ).pos;
			
				DRAW.Line(lines[i][e]).SetColor(c);
			}
		}
	}
}
